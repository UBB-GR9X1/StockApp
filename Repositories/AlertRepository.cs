namespace StockApp.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Data.SqlClient;
    using StockApp.Database;
    using StockApp.Models;

    public class AlertRepository
    {
        private readonly List<Alert> alerts = [];
        private readonly SqlConnection dbConnection = DatabaseHelper.GetConnection();

        public AlertRepository()
        {
            this.LoadAlerts();
        }

        public List<TriggeredAlert> TriggeredAlerts { get; private set; } = [];

        // Shared Method for SQL Command Execution
        private void ExecuteSql(string query, Action<SqlCommand> parameterize)
        {
            using var command = new SqlCommand(query, this.dbConnection);
            parameterize?.Invoke(command);
            command.ExecuteNonQuery();
        }

        // Shared Method for SQL Reader
        private List<T> ExecuteReader<T>(string query, Func<SqlDataReader, T> map)
        {
            using var command = new SqlCommand(query, this.dbConnection);
            using var reader = command.ExecuteReader();
            List<T> results = [];

            while (reader.Read())
            {
                results.Add(map(reader));
            }

            return results;
        }

        public void LoadAlerts()
        {
            string query = "SELECT * FROM ALERTS";
            this.alerts.Clear();
            this.alerts.AddRange(this.ExecuteReader(query, reader => new Alert
            {
                AlertId = reader.GetInt32(0),
                StockName = reader.GetString(1),
                Name = reader.GetString(2),
                LowerBound = reader.GetInt32(3),
                UpperBound = reader.GetInt32(4),
                ToggleOnOff = reader.GetBoolean(5),
            }));
        }

        public List<Alert> GetAllAlerts() => alerts;

        public Alert GetAlertById(int alertId)
        => alerts.FirstOrDefault(a => a.AlertId == alertId)
           ?? throw new KeyNotFoundException($"Alert {alertId} not found.");

        public void AddAlert(Alert alert)
        {
            string insertQuery = @"INSERT INTO ALERTS (STOCK_NAME, NAME, LOWER_BOUND, UPPER_BOUND, TOGGLE) 
                                   VALUES (@StockName, @Name, @LowerBound, @UpperBound, @ToggleOnOff);
                                   SELECT SCOPE_IDENTITY();";

            this.ExecuteSql(insertQuery, command =>
            {
                command.Parameters.AddWithValue("@StockName", alert.StockName);
                command.Parameters.AddWithValue("@Name", alert.Name);
                command.Parameters.AddWithValue("@LowerBound", alert.LowerBound);
                command.Parameters.AddWithValue("@UpperBound", alert.UpperBound);
                command.Parameters.AddWithValue("@ToggleOnOff", alert.ToggleOnOff);
                alert.AlertId = Convert.ToInt32(command.ExecuteScalar());
            });

            this.alerts.Add(alert);
        }

        public void UpdateAlert(Alert alert)
        {
            string updateQuery = @"UPDATE ALERTS
                                   SET STOCK_NAME = @StockName, NAME = @Name, LOWER_BOUND = @LowerBound, 
                                       UPPER_BOUND = @UpperBound, TOGGLE = @ToggleOnOff
                                   WHERE ALERT_ID = @AlertId";

            this.ExecuteSql(updateQuery, command =>
            {
                command.Parameters.AddWithValue("@AlertId", alert.AlertId);
                command.Parameters.AddWithValue("@StockName", alert.StockName);
                command.Parameters.AddWithValue("@Name", alert.Name);
                command.Parameters.AddWithValue("@LowerBound", alert.LowerBound);
                command.Parameters.AddWithValue("@UpperBound", alert.UpperBound);
                command.Parameters.AddWithValue("@ToggleOnOff", alert.ToggleOnOff);
            });

            var existingAlert = this.alerts.FirstOrDefault(a => a.AlertId == alert.AlertId);
            if (existingAlert != null)
            {
                existingAlert.StockName = alert.StockName;
                existingAlert.Name = alert.Name;
                existingAlert.LowerBound = alert.LowerBound;
                existingAlert.UpperBound = alert.UpperBound;
                existingAlert.ToggleOnOff = alert.ToggleOnOff;
            }
        }

        public void DeleteAlert(int alertId)
        {
            string deleteQuery = "DELETE FROM ALERTS WHERE ALERT_ID = @AlertId";

            this.ExecuteSql(deleteQuery, command =>
            {
                command.Parameters.AddWithValue("@AlertId", alertId);
            });

            this.alerts.RemoveAll(a => a.AlertId == alertId);
        }

        public bool IsAlertTriggered(string stockName, decimal currentPrice)
        {
            var alert = this.alerts.FirstOrDefault(a => a.StockName == stockName);
            return alert != null && alert.ToggleOnOff &&
                   (currentPrice >= alert.UpperBound || currentPrice <= alert.LowerBound);
        }

        public void TriggerAlert(string stockName, decimal currentPrice)
        {
            if (!this.IsAlertTriggered(stockName, currentPrice))
            {
                return;
            }

            var alert = this.alerts.First(a => a.StockName == stockName);
            var message = $"⚠ Alert triggered for {stockName}: Price = {currentPrice}, Bounds: [{alert.LowerBound} - {alert.UpperBound}]";

            this.TriggeredAlerts.Add(new TriggeredAlert
            {
                StockName = stockName,
                Message = message,
            });
        }

        public List<TriggeredAlert> GetTriggeredAlerts() => TriggeredAlerts;

        public void ClearTriggeredAlerts() => this.TriggeredAlerts.Clear();
    }
}