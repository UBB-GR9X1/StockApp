namespace StockApp.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Microsoft.Data.SqlClient;

    internal class DatabaseHelper
    {
        private static readonly string ConnectionString = @"
            Server=(localdb)\\MSSQLLocalDB;
            Database=StockApp_DB;   
            Trusted_Connection=True;
            TrustServerCertificate=True;";

        private static DatabaseHelper? instance;

        public static DatabaseHelper Instance => instance ??= new ();

        private DatabaseHelper()
        {
            InitializeDatabase();
        }

        public static void InitializeDatabase()
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string is not initialized");

            bool databaseExists = false;
            bool tablesExist = false;

            try
            {
                tablesExist = CheckIfTablesExist();
                databaseExists = true;
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking database existence: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex.Message}");
            }

            if (!databaseExists)
            {
                ResetDatabase();
                tablesExist = false; // new database, tables don't exist
            }

            if (!tablesExist)
            {
                CreateDatabaseTables();
            }
        }

        public static void CloseConnection(SqlConnection connection)
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public static SqlConnection GetConnection()
        {
            SqlConnection connection = new (ConnectionString);

            try
            {
                connection.Open();
                return connection;
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening SQL Server connection: {ex.Message}");
                throw;
            }
        }

        private static bool CheckIfTablesExist()
        {
            const string checkTableQuery = @"
                SELECT CASE WHEN EXISTS (
                    SELECT 1
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_NAME = 'STOCK'
                ) THEN 1 ELSE 0 END";

            try
            {
                using SqlConnection connection = new (ConnectionString);
                connection.Open();

                using SqlCommand command = new (checkTableQuery, connection);
                return Convert.ToBoolean(command.ExecuteScalar() ?? false);
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking table existence: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex.Message}");
                return false;
            }
        }

        private static void ResetDatabase()
        {
            const string masterConnection = @"
                Data Source=VM;
                Initial Catalog=StockApp_DB;
                Integrated Security=True;
                Trust Server Certificate=True";

            try
            {
                using SqlConnection connection = new (masterConnection);
                connection.Open();

                using SqlCommand command = new (null, connection);

                const string dropDbQuery = @"
                    IF EXISTS (
                        SELECT name 
                        FROM sys.databases 
                        WHERE name = 'StockApp_DB'
                    ) DROP DATABASE StockApp_DB";
                command.CommandText = dropDbQuery;
                command.ExecuteNonQuery();

                const string createDbQuery = "CREATE DATABASE StockApp_DB";
                command.CommandText = createDbQuery;
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error resetting database: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex.Message}");
            }
        }

        private static void CreateDatabaseTables()
        {
            /*
            There is also this table:

            string createTriggeredAlertsTableQuery = @"
               CREATE TABLE TRIGGERED_ALERTS (
                  SOCK_NAME NVARCHAR(100),
                  NAME NVARCHAR(100),";

            But whoever wrote this code didn't finish it, so I won't either.
            */

            List<string> tableCreationQueries =
            [
               @"
                   CREATE TABLE [USER] (
                       CNP NVARCHAR(50) PRIMARY KEY,
                       NAME NVARCHAR(100),
                       DESCRIPTION NVARCHAR(MAX),
                       IS_HIDDEN BIT,
                       IS_ADMIN BIT,
                       PROFILE_PICTURE NVARCHAR(MAX),
                       GEM_BALANCE INT
                   )",
               @"
                   CREATE TABLE STOCK (
                       STOCK_NAME NVARCHAR(100) PRIMARY KEY,
                       STOCK_SYMBOL NVARCHAR(20),
                       AUTHOR_CNP NVARCHAR(50),
                       FOREIGN KEY (AUTHOR_CNP) REFERENCES [USER](CNP)
                   )",
               @"
                   CREATE TABLE STOCK_VALUE (
                       STOCK_VALUE_ID INT IDENTITY(1,1) PRIMARY KEY,
                       STOCK_NAME NVARCHAR(100),
                       PRICE INT,
                       FOREIGN KEY (STOCK_NAME) REFERENCES STOCK(STOCK_NAME)
                   )",
               @"
                   CREATE TABLE USER_STOCK (
                       USER_CNP NVARCHAR(50),
                       STOCK_NAME NVARCHAR(100),
                       QUANTITY INT,
                       FOREIGN KEY (USER_CNP) REFERENCES [USER](CNP),
                       FOREIGN KEY (STOCK_NAME) REFERENCES STOCK(STOCK_NAME)
                   )",
               @"
                   CREATE TABLE FAVORITE_STOCK (
                       USER_CNP NVARCHAR(50),
                       STOCK_NAME NVARCHAR(100),
                       IS_FAVORITE BIT,
                       FOREIGN KEY (USER_CNP) REFERENCES [USER](CNP),
                       FOREIGN KEY (STOCK_NAME) REFERENCES STOCK(STOCK_NAME)
                   )",
               @"
                   CREATE TABLE USERS_TRANSACTION (
                       TRANSACTION_ID INT IDENTITY(1,1) PRIMARY KEY,
                       USER_CNP NVARCHAR(50),
                       STOCK_NAME NVARCHAR(100),
                       TYPE INT,
                       QUANTITY INT,
                       PRICE INT,
                       DATE NVARCHAR(50),
                       FOREIGN KEY (USER_CNP) REFERENCES [USER](CNP),
                       FOREIGN KEY (STOCK_NAME) REFERENCES STOCK(STOCK_NAME)
                   )",
               @"
                   CREATE TABLE ALERTS (
                       ALERT_ID INT IDENTITY(1,1) PRIMARY KEY,
                       STOCK_NAME NVARCHAR(100),
                       NAME NVARCHAR(100),
                       LOWER_BOUND INT,
                       UPPER_BOUND INT,
                       TOGGLE BIT,
                       FOREIGN KEY (STOCK_NAME) REFERENCES STOCK(STOCK_NAME)
                   )",
               @"
                   CREATE TABLE NEWS_ARTICLE (
                       ARTICLE_ID NVARCHAR(100) PRIMARY KEY,
                       TITLE NVARCHAR(200) NOT NULL,
                       SUMMARY NVARCHAR(MAX),
                       CONTENT NVARCHAR(MAX) NOT NULL,
                       SOURCE NVARCHAR(100),
                       PUBLISH_DATE NVARCHAR(50) NOT NULL,
                       IS_READ BIT,
                       IS_WATCHLIST_RELATED BIT,
                       CATEGORY NVARCHAR(50)
                   )",
               @"
                   CREATE TABLE USER_ARTICLE (
                       ARTICLE_ID NVARCHAR(100) PRIMARY KEY,
                       TITLE NVARCHAR(200) NOT NULL,
                       SUMMARY NVARCHAR(MAX),
                       CONTENT NVARCHAR(MAX) NOT NULL,
                       AUTHOR_CNP NVARCHAR(50),
                       SUBMISSION_DATE NVARCHAR(50),
                       STATUS NVARCHAR(50),
                       TOPIC NVARCHAR(50),
                       FOREIGN KEY (AUTHOR_CNP) REFERENCES [USER](CNP)
                   )",
               @"
                   CREATE TABLE RELATED_STOCKS (
                       SERIAL_ID INT IDENTITY(1,1) PRIMARY KEY,
                       STOCK_NAME NVARCHAR(100) NOT NULL,
                       ARTICLE_ID NVARCHAR(100) NOT NULL,
                       FOREIGN KEY (ARTICLE_ID) REFERENCES NEWS_ARTICLE(ARTICLE_ID),
                       FOREIGN KEY (STOCK_NAME) REFERENCES STOCK(STOCK_NAME)
                   )",
               @"
                   CREATE TABLE HARDCODED_CNPS (
                       CNP NVARCHAR(50) PRIMARY KEY
                   )"
            ];

            try
            {
                using SqlConnection connection = new (ConnectionString);
                connection.Open();

                using SqlCommand command = new (null, connection);

                foreach (string query in tableCreationQueries)
                {
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating tables: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex.Message}");
                throw;
            }
        }
    }
}