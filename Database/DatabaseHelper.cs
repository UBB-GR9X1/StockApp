using System;
using System.Data.Common;
using System.IO;
using Microsoft.Data.SqlClient;

namespace StockApp.Database
{
    internal class DatabaseHelper
    {
        private static string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=StockApp_DB;Trusted_Connection=True;TrustServerCertificate=True;";
        private static DatabaseHelper _instance;

        public static DatabaseHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DatabaseHelper();
                }
                return _instance;
            }
        }

        private DatabaseHelper()
        {
            InitializeDatabase();
        }

        public static void InitializeDatabase()
        {
            bool databaseExists = false;
            bool tablesExist = false;

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    databaseExists = true;

                    string checkTableQuery = "SELECT CASE WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'STOCK') THEN 1 ELSE 0 END";
                    using (var command = new SqlCommand(checkTableQuery, connection))
                    {
                        tablesExist = Convert.ToBoolean(command.ExecuteScalar());
                    }

                    connection.Close();
                }
            }
            catch
            {
                databaseExists = false;
                tablesExist = false;
            }

            if (!databaseExists)
            {
                string masterConnection = "Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";
                using (var connection = new SqlConnection(masterConnection))
                {
                    connection.Open();
                    string dropDbQuery = "IF EXISTS (SELECT name FROM sys.databases WHERE name = 'StockApp_DB') DROP DATABASE StockApp_DB";
                    using (var command = new SqlCommand(dropDbQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    string createDbQuery = "CREATE DATABASE StockApp_DB";
                    using (var command = new SqlCommand(createDbQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
                tablesExist = false; // new database, tables don't exist
            }

            // tables if they don't exist
            if (!tablesExist)
            {
                using (var connectionTOBD = new SqlConnection(connectionString))
                {
                    connectionTOBD.Open();

                    string createUserTableQuery =
                        "CREATE TABLE [USER] (" +
                        " CNP NVARCHAR(50) PRIMARY KEY," +
                        " NAME NVARCHAR(100)," +
                        " DESCRIPTION NVARCHAR(MAX)," +
                        " IS_HIDDEN BIT," +
                        " IS_ADMIN BIT," +
                        " PROFILE_PICTURE NVARCHAR(MAX)," +
                        " GEM_BALANCE INT)";

                    string createStockTableQuery =
                        "CREATE TABLE STOCK (" +
                        "STOCK_NAME NVARCHAR(100) PRIMARY KEY," +
                        " STOCK_SYMBOL NVARCHAR(20)," +
                        " AUTHOR_CNP NVARCHAR(50)," +
                        " FOREIGN KEY (AUTHOR_CNP) REFERENCES [USER](CNP))";

                    string createStockValueTableQuery =
                        "CREATE TABLE STOCK_VALUE (" +
                        "STOCK_VALUE_ID INT IDENTITY(1,1) PRIMARY KEY," +
                        "STOCK_NAME NVARCHAR(100)," +
                        " PRICE INT," +
                        " FOREIGN KEY (STOCK_NAME) REFERENCES STOCK(STOCK_NAME))";

                    string createUserStockTableQuery =
                        "CREATE TABLE USER_STOCK (" +
                        " USER_CNP NVARCHAR(50)," +
                        " STOCK_NAME NVARCHAR(100)," +
                        " QUANTITY INT," +
                        " FOREIGN KEY (USER_CNP) REFERENCES [USER](CNP)," +
                        " FOREIGN KEY (STOCK_NAME) REFERENCES STOCK(STOCK_NAME))";

                    string createFavoriteStockTableQuery =
                        "CREATE TABLE FAVORITE_STOCK (" +
                        "USER_CNP NVARCHAR(50)," +
                        " STOCK_NAME NVARCHAR(100)," +
                        " IS_FAVORITE BIT," +
                        " FOREIGN KEY (USER_CNP) REFERENCES [USER](CNP)," +
                        " FOREIGN KEY (STOCK_NAME) REFERENCES STOCK(STOCK_NAME))";

                    string createTransactionTableQuery =
                        "CREATE TABLE USERS_TRANSACTION (" +
                        "TRANSACTION_ID INT IDENTITY(1,1) PRIMARY KEY," +
                        " USER_CNP NVARCHAR(50)," +
                        " STOCK_NAME NVARCHAR(100)," +
                        " TYPE INT," +
                        " QUANTITY INT," +
                        " PRICE INT," +
                        " DATE NVARCHAR(50)," +
                        " FOREIGN KEY (USER_CNP) REFERENCES [USER](CNP)," +
                        " FOREIGN KEY (STOCK_NAME) REFERENCES STOCK(STOCK_NAME))";

                    string createAlertsTableQuery =
                        "CREATE TABLE ALERTS (" +
                        "ALERT_ID INT IDENTITY(1,1) PRIMARY KEY," +
                        " STOCK_NAME NVARCHAR(100)," +
                        " NAME NVARCHAR(100)," +
                        " LOWER_BOUND INT," +
                        " UPPER_BOUND INT," +
                        " TOGGLE BIT," +
                        " FOREIGN KEY (STOCK_NAME) REFERENCES STOCK(STOCK_NAME))";
                    string createTriggeredAlertsTableQuery =
                        "CREATE TABLE TRIGGERED_ALERTS (" +
                        "SOCK_NAME NVARCHAR(100)," +
                        " NAME NVARCHAR(100),";

                    string createNewsArticleTableQuery =
                        "CREATE TABLE NEWS_ARTICLE (" +
                        "ARTICLE_ID NVARCHAR(100) PRIMARY KEY," +
                        " TITLE NVARCHAR(200) NOT NULL," +
                        " SUMMARY NVARCHAR(MAX)," +
                        " CONTENT NVARCHAR(MAX) NOT NULL," +
                        " SOURCE NVARCHAR(100)," +
                        " PUBLISH_DATE NVARCHAR(50) NOT NULL," +
                        " IS_READ BIT," +
                        " IS_WATCHLIST_RELATED BIT," +
                        " CATEGORY NVARCHAR(50))";

                    string createUserArticleTableQuery =
                        "CREATE TABLE USER_ARTICLE (" +
                        " ARTICLE_ID NVARCHAR(100) PRIMARY KEY," +
                        " TITLE NVARCHAR(200) NOT NULL," +
                        " SUMMARY NVARCHAR(MAX)," +
                        " CONTENT NVARCHAR(MAX) NOT NULL," +
                        " AUTHOR_CNP NVARCHAR(50)," +
                        " SUBMISSION_DATE NVARCHAR(50)," +
                        " STATUS NVARCHAR(50)," +
                        " TOPIC NVARCHAR(50)," +
                        " FOREIGN KEY (AUTHOR_CNP) REFERENCES [USER](CNP))";

                    string createRelatedStocksTableQuery =
                        "CREATE TABLE RELATED_STOCKS (" +
                        " SERIAL_ID INT IDENTITY(1,1) PRIMARY KEY," +
                        " STOCK_NAME NVARCHAR(100) NOT NULL," +
                        " ARTICLE_ID NVARCHAR(100) NOT NULL," +
                        " FOREIGN KEY (ARTICLE_ID) REFERENCES NEWS_ARTICLE(ARTICLE_ID)," +
                        " FOREIGN KEY (STOCK_NAME) REFERENCES STOCK(STOCK_NAME))";

                    string createHardcodedCNPsTableQuery =
                        "CREATE TABLE HARDCODED_CNPS (" +
                        " CNP NVARCHAR(50) PRIMARY KEY)";

                    try
                    {
                        using (var command = new SqlCommand(null, connectionTOBD))
                        {
                            command.CommandText = createUserTableQuery;
                            command.ExecuteNonQuery();
                            command.CommandText = createStockTableQuery;
                            command.ExecuteNonQuery();
                            command.CommandText = createStockValueTableQuery;
                            command.ExecuteNonQuery();
                            command.CommandText = createUserStockTableQuery;
                            command.ExecuteNonQuery();
                            command.CommandText = createFavoriteStockTableQuery;
                            command.ExecuteNonQuery();
                            command.CommandText = createTransactionTableQuery;
                            command.ExecuteNonQuery();
                            command.CommandText = createAlertsTableQuery;
                            command.ExecuteNonQuery();
                            command.CommandText = createTriggeredAlertsTableQuery;
                            command.CommandText = createNewsArticleTableQuery;
                            command.ExecuteNonQuery();
                            command.CommandText = createUserArticleTableQuery;
                            command.ExecuteNonQuery();
                            command.CommandText = createRelatedStocksTableQuery;
                            command.ExecuteNonQuery();
                            command.CommandText = createHardcodedCNPsTableQuery;
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error creating tables: {ex.Message}");
                        throw;
                    }

                    SqlCommand addUsers = new SqlCommand(@"
INSERT INTO [USER] (CNP, NAME, DESCRIPTION, IS_HIDDEN, IS_ADMIN, PROFILE_PICTURE, GEM_BALANCE) VALUES 
('1234567890123', 'John Doe 1', 'I am a user 1', 0, 0, 'https://images.unsplash.com/photo-1547481887-a26e2cacb5b2?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D', 1000), 
('1234567890124', 'John Doe 2', 'I am a user 2', 0, 0, 'https://images.unsplash.com/photo-1594583388647-364ea6532257?q=80&w=1974&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D', 1000), 
('1234567890125', 'John Doe 3', 'I am a user 3', 0, 0, 'https://images.unsplash.com/photo-1530747656683-c940eb6472d0?q=80&w=1990&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D', 1000),
('6666666666666', 'admin', 'I am the admin', 0, 1, 'https://images.unsplash.com/photo-1530747656683-c940eb6472d0?q=80&w=1990&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D', 1000)", 
                        connectionTOBD);
                    addUsers.ExecuteNonQuery();

                    SqlCommand addUSER = new SqlCommand("INSERT INTO HARDCODED_CNPS (CNP) VALUES ('1234567890124')", connectionTOBD);
                    addUSER.ExecuteNonQuery();

                    SqlCommand addStocks = new SqlCommand(@"
INSERT INTO STOCK (STOCK_NAME, STOCK_SYMBOL, AUTHOR_CNP) VALUES 
('Tesla', 'TSLA', '1234567890123'), 
('Besla', 'BSLA', '1234567890123'), 
('Cesla', 'CSLA', '1234567890124')",
connectionTOBD);
                    addStocks.ExecuteNonQuery();


                    SqlCommand addAlert = new SqlCommand(@"
INSERT INTO ALERTS (STOCK_NAME, NAME, LOWER_BOUND, UPPER_BOUND, TOGGLE) VALUES
('Tesla', 'Tesla Alert', 120, 150, 1),
('Besla', 'Besla Alert', 200, 250, 1),
('Cesla', 'Cesla Alert', 300, 350, 1)",
                        connectionTOBD);
                    addAlert.ExecuteNonQuery();

                    

                    SqlCommand addStockValues = new SqlCommand(@"
INSERT INTO STOCK_VALUE (STOCK_NAME, PRICE) VALUES
('Tesla', 133),
('Tesla', 140),
('Tesla', 150),
('Tesla', 120),
('Besla', 200),
('Besla', 250),
('Besla', 220),
('Besla', 210),
('Cesla', 300)",
connectionTOBD);
                    addStockValues.ExecuteNonQuery();

                    connectionTOBD.Close();
                }
            }
        }

        public void CloseConnection(SqlConnection connection)
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public SqlConnection GetConnection()
        {
            var connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening SQL Server connection: {ex.Message}");
                throw;
            }
        }
    }
}