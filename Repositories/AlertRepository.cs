using System.Collections.Generic;
using System.Linq;
using Models;
using System.Data.SQLite;
using StockApp.Database;
using System;
using Models;

namespace Alerts.Repository
{
    public class AlertRepository
    {
        private readonly List<Alert> _alerts = new List<Alert>();
        private readonly SQLiteConnection _dbConnection = DatabaseHelper.Instance.GetConnection();

        public AlertRepository()
        {
            InitializeDatabase();
            LoadAlerts();
            if (!_alerts.Any())
            {
                InitializeDefaultAlerts();
            }
        }

        private void InitializeDatabase()
        {
            try
            {
                // Check if table exists
                string checkTableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name='ALERTS'";

                using (var checkCommand = new SQLiteCommand(checkTableQuery, _dbConnection))
                {
                    var tableExists = checkCommand.ExecuteScalar() != null;

                    if (!tableExists)
                    {
                        CreateTable();
                    }
                    else
                    {
                        CheckAndAddMissingColumns();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle database initialization errors
                throw new Exception("Failed to initialize database", ex);
            }
        }

        private void CreateTable()
        {
            string createTableQuery = @"
                CREATE TABLE ALERTS (
                    ALERT_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    STOCK_NAME TEXT NOT NULL,
                    NAME TEXT NOT NULL,
                    UPPER_BOUND INTEGER NOT NULL,
                    LOWER_BOUND INTEGER NOT NULL,
                    TOGGLE_ON_OFF INTEGER NOT NULL DEFAULT 1
                )";

            using (var command = new SQLiteCommand(createTableQuery, _dbConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        private void CheckAndAddMissingColumns()
        {
            var requiredColumns = new Dictionary<string, string>
            {
                {"STOCK_NAME", "TEXT NOT NULL"},
                {"NAME", "TEXT NOT NULL"},
                {"UPPER_BOUND", "INTEGER NOT NULL"},
                {"LOWER_BOUND", "INTEGER NOT NULL"},
                {"TOGGLE_ON_OFF", "INTEGER NOT NULL DEFAULT 1"}
            };

            foreach (var column in requiredColumns)
            {
                string checkColumnQuery = $@"
                    SELECT COUNT(*) FROM pragma_table_info('ALERTS') 
                    WHERE name = '{column.Key}'";

                using (var checkCommand = new SQLiteCommand(checkColumnQuery, _dbConnection))
                {
                    var columnExists = Convert.ToInt64(checkCommand.ExecuteScalar()) > 0;
                    if (!columnExists)
                    {
                        string addColumnQuery = $"ALTER TABLE ALERTS ADD COLUMN {column.Key} {column.Value}";
                        using (var alterCommand = new SQLiteCommand(addColumnQuery, _dbConnection))
                        {
                            alterCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public List<Alert> GetAllAlerts() => _alerts;

        private void LoadAlerts()
        {
            _alerts.Clear();
            string query = "SELECT * FROM ALERTS";

            using (var command = new SQLiteCommand(query, _dbConnection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    try
                    {
                        _alerts.Add(new Alert
                        {
                            AlertId = Convert.ToInt32(reader["ALERT_ID"]),
                            StockName = reader["STOCK_NAME"].ToString(),
                            Name = reader["NAME"].ToString(),
                            UpperBound = Convert.ToInt32(reader["UPPER_BOUND"]),
                            LowerBound = Convert.ToInt32(reader["LOWER_BOUND"]),
                            ToggleOnOff = Convert.ToInt32(reader["TOGGLE_ON_OFF"]) == 1
                        });
                    }
                    catch (Exception ex)
                    {
                        // Handle individual row parsing errors
                        throw new Exception($"Failed to parse alert data: {ex.Message}", ex);
                    }
                }
            }
        }

        private void InitializeDefaultAlerts()
        {
            var defaultAlerts = new List<Alert>
            {
                new Alert {
                    StockName = "Tesla",
                    Name = "Price Drop Alert",
                    UpperBound = 200,
                    LowerBound = 150,
                    ToggleOnOff = true
                },
                new Alert {
                    StockName = "Apple",
                    Name = "Volatility Alert",
                    UpperBound = 180,
                    LowerBound = 160,
                    ToggleOnOff = true
                }
            };

            foreach (var alert in defaultAlerts)
            {
                try
                {
                    AddAlert(alert);
                }
                catch (Exception ex)
                {
                    // Handle alert initialization errors
                    throw new Exception($"Failed to initialize default alert: {ex.Message}", ex);
                }
            }
        }

        public void AddAlert(Alert alert)
        {
            string query = @"
                INSERT INTO ALERTS 
                (STOCK_NAME, NAME, UPPER_BOUND, LOWER_BOUND, TOGGLE_ON_OFF) 
                VALUES (@StockName, @Name, @UpperBound, @LowerBound, @ToggleOnOff)";

            using (var command = new SQLiteCommand(query, _dbConnection))
            {
                command.Parameters.AddWithValue("@StockName", alert.StockName ?? string.Empty);
                command.Parameters.AddWithValue("@Name", alert.Name ?? string.Empty);
                command.Parameters.AddWithValue("@UpperBound", alert.UpperBound);
                command.Parameters.AddWithValue("@LowerBound", alert.LowerBound);
                command.Parameters.AddWithValue("@ToggleOnOff", alert.ToggleOnOff ? 1 : 0);
                command.ExecuteNonQuery();

                alert.AlertId = Convert.ToInt32(_dbConnection.LastInsertRowId);
                _alerts.Add(alert);
            }
        }

        public void DeleteAlert(int alertId)
        {
            string query = "DELETE FROM ALERTS WHERE ALERT_ID = @AlertId";
            using (var command = new SQLiteCommand(query, _dbConnection))
            {
                command.Parameters.AddWithValue("@AlertId", alertId);
                command.ExecuteNonQuery();
            }
            _alerts.RemoveAll(a => a.AlertId == alertId);
        }

        public void UpdateAlert(Alert alert)
        {
            string query = @"
                UPDATE ALERTS SET 
                STOCK_NAME = @StockName,
                NAME = @Name,
                UPPER_BOUND = @UpperBound,
                LOWER_BOUND = @LowerBound,
                TOGGLE_ON_OFF = @ToggleOnOff
                WHERE ALERT_ID = @AlertId";

            using (var command = new SQLiteCommand(query, _dbConnection))
            {
                command.Parameters.AddWithValue("@StockName", alert.StockName ?? string.Empty);
                command.Parameters.AddWithValue("@Name", alert.Name ?? string.Empty);
                command.Parameters.AddWithValue("@UpperBound", alert.UpperBound);
                command.Parameters.AddWithValue("@LowerBound", alert.LowerBound);
                command.Parameters.AddWithValue("@ToggleOnOff", alert.ToggleOnOff ? 1 : 0);
                command.Parameters.AddWithValue("@AlertId", alert.AlertId);
                command.ExecuteNonQuery();
            }
        }
    }
}