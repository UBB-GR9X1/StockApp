namespace StockApp.Repository
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

        public void LoadAlerts()
        {
            this.alerts.Clear();

            string query = "SELECT * FROM ALERTS";
            using SqlCommand command = new (query, this.dbConnection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                Alert alert = new ()
                {
                    AlertId = reader.GetInt32(0),
                    StockName = reader.GetString(1),
                    Name = reader.GetString(2),
                    LowerBound = reader.GetDecimal(3),
                    UpperBound = reader.GetDecimal(4),
                    ToggleOnOff = reader.GetBoolean(5),
                };

                this.alerts.Add(alert);
            }
        }

        public List<Alert> GetAllAlerts()
        {
            return this.alerts;
        }

        public Alert GetAlertById(int alertId)
        {
            return this.alerts.FirstOrDefault(a => a.AlertId == alertId)
                ?? throw new KeyNotFoundException($"Alert with ID {alertId} not found.");
        }

        public void AddAlert(Alert alert)
        {
            string insertQuery = @"
                INSERT INTO ALERTS (STOCK_NAME, NAME, LOWER_BOUND, UPPER_BOUND, TOGGLE) 
                VALUES (@StockName, @Name, @LowerBound, @UpperBound, @ToggleOnOff);
                SELECT SCOPE_IDENTITY();";

            using var command = new SqlCommand(insertQuery, this.dbConnection);
            command.Parameters.AddWithValue("@StockName", alert.StockName);
            command.Parameters.AddWithValue("@Name", alert.Name);
            command.Parameters.AddWithValue("@LowerBound", alert.LowerBound);
            command.Parameters.AddWithValue("@UpperBound", alert.UpperBound);
            command.Parameters.AddWithValue("@ToggleOnOff", alert.ToggleOnOff);

            var newAlertId = Convert.ToInt32(command.ExecuteScalar());
            alert.AlertId = newAlertId;
            this.alerts.Add(alert);
        }

        public void UpdateAlert(Alert alert)
        {
            string updateQuery = @"
                UPDATE ALERTS
                SET STOCK_NAME = @StockName, 
                    NAME = @Name, 
                    LOWER_BOUND = @LowerBound, 
                    UPPER_BOUND = @UpperBound, 
                    TOGGLE = @ToggleOnOff 
                WHERE ALERT_ID = @AlertId";

            using var command = new SqlCommand(updateQuery, this.dbConnection);
            command.Parameters.AddWithValue("@AlertId", alert.AlertId);
            command.Parameters.AddWithValue("@StockName", alert.StockName);
            command.Parameters.AddWithValue("@Name", alert.Name);
            command.Parameters.AddWithValue("@LowerBound", alert.LowerBound);
            command.Parameters.AddWithValue("@UpperBound", alert.UpperBound);
            command.Parameters.AddWithValue("@ToggleOnOff", alert.ToggleOnOff);

            command.ExecuteNonQuery();

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

            using SqlCommand command = new (deleteQuery, this.dbConnection);
            command.Parameters.AddWithValue("@AlertId", alertId);

            command.ExecuteNonQuery();

            this.alerts.RemoveAll(a => a.AlertId == alertId);
        }

        public bool IsAlertTriggered(string stockName, decimal currentPrice)
        {
            var alert = this.alerts.FirstOrDefault(a => a.StockName == stockName);

            if (alert == null)
            {
               return false;
            }

            return alert.ToggleOnOff
                && (currentPrice >= alert.UpperBound || currentPrice <= alert.LowerBound);
        }

        public void TriggerAlert(string stockName, decimal currentPrice)
        {
            var alert = this.alerts.FirstOrDefault(a => a.StockName == stockName);

            if (alert == null || !this.IsAlertTriggered(stockName, currentPrice))
            {
                return;
            }

            string message = $"⚠ Alert triggered for {stockName}:" +
                    $" Price = {currentPrice}, " +
                    $"Bounds: [{alert.LowerBound} - {alert.UpperBound}]";

            TriggeredAlert triggeredAlert = new ()
            {
                StockName = stockName,
                Message = message,
            };

            this.TriggeredAlerts.Add(triggeredAlert);
        }

        public List<TriggeredAlert> GetTriggeredAlerts()
        {
            return this.TriggeredAlerts;
        }

        public void ClearTriggeredAlerts()
        {
            this.TriggeredAlerts.Clear();
        }
    }
}
