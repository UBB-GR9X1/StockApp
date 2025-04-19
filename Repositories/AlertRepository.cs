namespace StockApp.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Data.SqlClient;
    using StockApp.Database;
    using StockApp.Exceptions;
    using StockApp.Models;

    /// <summary>
    /// Repository for managing <see cref="Alert"/> entities and their triggered instances in the database.
    /// </summary>
    public class AlertRepository
    {
        // Connection pulled from helper for executing commands
        private readonly SqlConnection dbConnection = DatabaseHelper.GetConnection();

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertRepository"/> class and loads alerts into memory.
        /// </summary>
        public AlertRepository()
        {
            this.LoadAlerts();
        }

        /// <summary>
        /// Gets the list of all configured alerts.
        /// </summary>
        public List<Alert> Alerts { get; } = [];

        /// <summary>
        /// Gets the collection of alerts that have been triggered since load or last clear.
        /// </summary>
        public List<TriggeredAlert> TriggeredAlerts { get; private set; } = [];

        /// <summary>
        /// Clears the in-memory list of triggered alerts.
        /// </summary>
        public void ClearTriggeredAlerts() => this.TriggeredAlerts.Clear();

        // Shared Method for SQL Command Execution
        private void ExecuteSql(string query, Action<SqlCommand> parameterize)
        {
            using var command = new SqlCommand(query, this.dbConnection);
            // Inline: apply parameters via provided lambda
            parameterize?.Invoke(command);
            command.ExecuteNonQuery();
        }

        // Shared Method for SQL Reader
        private List<T> ExecuteReader<T>(string query, Func<SqlDataReader, T> map)
        {
            using var command = new SqlCommand(query, this.dbConnection);
            using var reader = command.ExecuteReader();
            List<T> results = [];

            // Inline: iterate rows and map to objects
            while (reader.Read())
            {
                results.Add(map(reader));
            }

            return results;
        }

        /// <summary>
        /// Loads all alerts from the database into the <see cref="Alerts"/> list.
        /// </summary>
        public void LoadAlerts()
        {
            const string query = "SELECT * FROM ALERTS";
            this.Alerts.Clear();
            // Inline: map each row to an Alert instance
            this.Alerts.AddRange(
                this.ExecuteReader(query, reader => new Alert
                {
                    AlertId = reader.GetInt32(0),
                    StockName = reader.GetString(1),
                    Name = reader.GetString(2),
                    LowerBound = reader.GetInt32(3),
                    UpperBound = reader.GetInt32(4),
                    ToggleOnOff = reader.GetBoolean(5),
                })
            );
        }

        /// <summary>
        /// Retrieves a single alert by its identifier.
        /// </summary>
        /// <param name="alertId">The unique ID of the alert.</param>
        /// <returns>The matching <see cref="Alert"/>.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no alert with the given ID exists.</exception>
        public Alert GetAlertById(int alertId)
        {
            return this.Alerts.FirstOrDefault(a => a.AlertId == alertId)
                ?? throw new KeyNotFoundException($"Alert with ID {alertId} not found.");
        }

        /// <summary>
        /// Adds a new alert to the database and in-memory collection.
        /// </summary>
        /// <param name="stockName">Name of the stock to monitor.</param>
        /// <param name="name">Descriptive name of the alert.</param>
        /// <param name="upperBound">Upper price bound for triggering.</param>
        /// <param name="lowerBound">Lower price bound for triggering.</param>
        /// <param name="toggleOnOff">Whether the alert is active.</param>
        /// <returns>The newly created <see cref="Alert"/> with assigned ID.</returns>
        public Alert AddAlert(string stockName, string name, decimal upperBound, decimal lowerBound, bool toggleOnOff)
        {
            const string insertQuery = @"
                INSERT INTO ALERTS 
                    (STOCK_NAME, NAME, LOWER_BOUND, UPPER_BOUND, TOGGLE) 
                VALUES 
                    (@stockName, @name, @lowerBound, @upperBound, @toggleOnOff);
                
                SELECT SCOPE_IDENTITY();";

            int alertId = -1;

            // Inline: execute insert and retrieve new ID
            this.ExecuteSql(insertQuery, command =>
            {
                command.Parameters.AddWithValue("@stockName", stockName);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@lowerBound", lowerBound);
                command.Parameters.AddWithValue("@upperBound", upperBound);
                command.Parameters.AddWithValue("@toggleOnOff", toggleOnOff);
                alertId = Convert.ToInt32(command.ExecuteScalar());
            });

            var alert = new Alert
            {
                AlertId = alertId,
                StockName = stockName,
                Name = name,
                LowerBound = lowerBound,
                UpperBound = upperBound,
                ToggleOnOff = toggleOnOff,
            };

            this.Alerts.Add(alert);
            return alert;
        }

        /// <summary>
        /// Updates an existing alert's properties.
        /// </summary>
        /// <param name="alertId">ID of the alert to update.</param>
        /// <param name="stockName">New stock name.</param>
        /// <param name="name">New alert name.</param>
        /// <param name="upperBound">New upper bound.</param>
        /// <param name="lowerBound">New lower bound.</param>
        /// <param name="toggleOnOff">New toggle state.</param>
        /// <exception cref="AlertRepositoryException">Thrown if the database update fails.</exception>
        public void UpdateAlert(int alertId, string stockName, string name, decimal upperBound, decimal lowerBound, bool toggleOnOff)
        {
            string updateQuery = @"
        UPDATE ALERTS SET 
            STOCK_NAME = @stockName, 
            NAME = @name, 
            LOWER_BOUND = @lowerBound, 
            UPPER_BOUND = @upperBound, 
            TOGGLE = @toggleOnOff
        WHERE ALERT_ID = @alertId";

            try
            {
                this.ExecuteSql(updateQuery, command =>
                {
                    command.Parameters.AddWithValue("@alertId", alertId);
                    command.Parameters.AddWithValue("@stockName", stockName);
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@lowerBound", lowerBound);
                    command.Parameters.AddWithValue("@upperBound", upperBound);
                    command.Parameters.AddWithValue("@toggleOnOff", toggleOnOff);
                });

                // Inline: update in-memory object
                var existingAlert = this.Alerts.FirstOrDefault(a => a.AlertId == alertId);
                if (existingAlert != null)
                {
                    existingAlert.StockName = stockName;
                    existingAlert.Name = name;
                    existingAlert.LowerBound = lowerBound;
                    existingAlert.UpperBound = upperBound;
                    existingAlert.ToggleOnOff = toggleOnOff;
                }
            }
            catch (SqlException ex)
            {
                throw new AlertRepositoryException($"Failed to update alert with ID {alertId}.", ex);
            }
        }

        /// <summary>
        /// Deletes an alert from the database and removes it from memory.
        /// </summary>
        /// <param name="alertId">ID of the alert to delete.</param>
        public void DeleteAlert(int alertId)
        {
            string deleteQuery = "DELETE FROM ALERTS WHERE ALERT_ID = @alertId";

            this.ExecuteSql(deleteQuery, command =>
            {
                command.Parameters.AddWithValue("@alertId", alertId);
            });

            // Inline: remove from in-memory list
            this.Alerts.RemoveAll(a => a.AlertId == alertId);
        }

        /// <summary>
        /// Checks whether the specified stock price triggers any alert.
        /// </summary>
        /// <param name="stockName">Name of the stock.</param>
        /// <param name="currentPrice">Current price of the stock.</param>
        /// <returns><c>true</c> if an enabled alert is triggered; otherwise, <c>false</c>.</returns>
        public bool IsAlertTriggered(string stockName, decimal currentPrice)
        {
            var alert = this.Alerts.FirstOrDefault(a => a.StockName == stockName);
            return alert != null
                   && alert.ToggleOnOff
                   && (currentPrice >= alert.UpperBound || currentPrice <= alert.LowerBound);
        }

        /// <summary>
        /// Adds a <see cref="TriggeredAlert"/> entry if the given stock price triggers the alert.
        /// </summary>
        /// <param name="stockName">Name of the stock.</param>
        /// <param name="currentPrice">Current price of the stock.</param>
        public void TriggerAlert(string stockName, decimal currentPrice)
        {
            if (!this.IsAlertTriggered(stockName, currentPrice))
            {
                return; // Inline: do nothing if not triggered
            }

            var alert = this.Alerts.First(a => a.StockName == stockName);
            string message = $"Alert triggered for {stockName}: Price = {currentPrice}, Bounds: [{alert.LowerBound} - {alert.UpperBound}]";

            this.TriggeredAlerts.Add(new TriggeredAlert
            {
                StockName = stockName,
                Message = message,
            });
        }
    }
}
