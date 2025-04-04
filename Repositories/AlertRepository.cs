using System.Collections.Generic;
using System.Linq;
using Models;
using StockApp.Database;
using System;
using Microsoft.Data.SqlClient;

namespace Repository
{
    public class AlertRepository
    {
        private readonly List<Alert> _alerts = new List<Alert>();
        private SqlConnection dbConnection = DatabaseHelper.Instance.GetConnection();

        public AlertRepository()
        {
            LoadAlerts();
        }

        public void LoadAlerts()
        {
            _alerts.Clear(); 
            using (var connection = dbConnection)
            {
                string query = "SELECT * FROM ALERTS"; 
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _alerts.Add(new Alert
                            {
                                AlertId = reader.GetInt32(0),
                                StockName = reader.GetString(1),
                                Name = reader.GetString(2),
                                LowerBound = reader.GetInt32(3),
                                UpperBound = reader.GetInt32(4),
                                ToggleOnOff = reader.GetBoolean(5)
                            });
                        }
                    }
                }
            }
        }

        public List<Alert> GetAllAlerts()
        {
            return _alerts;
        }

        public Alert GetAlertById(int alertId)
        {
            return _alerts.FirstOrDefault(a => a.AlertId == alertId);
        }

        public void AddAlert(Alert alert)
        {
            string insertQuery = @"
                INSERT INTO ALERTS (STOCK_NAME, NAME, LOWER_BOUND, UPPER_BOUND, TOGGLE) 
                VALUES (@StockName, @Name, @LowerBound, @UpperBound, @ToggleOnOff);
                SELECT SCOPE_IDENTITY();";

            using (var connection = dbConnection)
            {
                using (var command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@StockName", alert.StockName);
                    command.Parameters.AddWithValue("@Name", alert.Name);
                    command.Parameters.AddWithValue("@LowerBound", alert.LowerBound);
                    command.Parameters.AddWithValue("@UpperBound", alert.UpperBound);
                    command.Parameters.AddWithValue("@ToggleOnOff", alert.ToggleOnOff);

                    connection.Open();
                    var newAlertId = Convert.ToInt32(command.ExecuteScalar());
                    alert.AlertId = newAlertId; 
                    _alerts.Add(alert);  
                }
            }
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

            using (var connection = dbConnection)
            {
                using (var command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@AlertId", alert.AlertId);
                    command.Parameters.AddWithValue("@StockName", alert.StockName);
                    command.Parameters.AddWithValue("@Name", alert.Name);
                    command.Parameters.AddWithValue("@LowerBound", alert.LowerBound);
                    command.Parameters.AddWithValue("@UpperBound", alert.UpperBound);
                    command.Parameters.AddWithValue("@ToggleOnOff", alert.ToggleOnOff);

                    connection.Open();
                    command.ExecuteNonQuery();

                    var existingAlert = _alerts.FirstOrDefault(a => a.AlertId == alert.AlertId);
                    if (existingAlert != null)
                    {
                        existingAlert.StockName = alert.StockName;
                        existingAlert.Name = alert.Name;
                        existingAlert.LowerBound = alert.LowerBound;
                        existingAlert.UpperBound = alert.UpperBound;
                        existingAlert.ToggleOnOff = alert.ToggleOnOff;
                    }
                }
            }
        }
        public void DeleteAlert(int alertId)
        {
            string deleteQuery = "DELETE FROM ALERTS WHERE ALERT_ID = @AlertId";

            using (var connection = dbConnection)
            {
                using (var command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@AlertId", alertId);

                    connection.Open();
                    command.ExecuteNonQuery();
                    _alerts.RemoveAll(a => a.AlertId == alertId);
                }
            }
        }

        public bool IsAlertTriggered(string stockName, decimal currentPrice)
        {
            var alert = _alerts.FirstOrDefault(a => a.StockName == stockName);
            if (alert != null)
            {
                return (currentPrice >= alert.UpperBound || currentPrice <= alert.LowerBound) && alert.ToggleOnOff;
            }
            return false;
        }

        public void TriggerAlert(string stockName, decimal currentPrice)  // notify the user (list with alerts)
        {
            var alert = _alerts.FirstOrDefault(a => a.StockName == stockName);
            if (alert != null && IsAlertTriggered(stockName, currentPrice))
            {
               
                Console.WriteLine($"Alert triggered for {stockName}: Current Price = {currentPrice}");
            }
        }


    }
}
