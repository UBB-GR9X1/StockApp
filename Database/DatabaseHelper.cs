namespace StockApp.Database
{
    using System;
    using System.Data;
    using System.IO;
    using Microsoft.Data.SqlClient;

    internal class DatabaseHelper
    {
        private const string CreateTablesScriptPath = "SqlScripts/CreateTables.sql";
        private const string ResetDatabaseScriptPath = "SqlScripts/ResetDatabase.sql";
        private const string CheckTablesExistScriptPath = "SqlScripts/CheckTablesExist.sql";

        private static DatabaseHelper? instance;

        private DatabaseHelper()
        {
            InitializeDatabase();
        }

        public static DatabaseHelper Instance => instance ??= new();

        public static void InitializeDatabase()
        {
            if (string.IsNullOrWhiteSpace(App.ConnectionString))
            {
                throw new InvalidOperationException("Connection string is not initialized");
            }

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
                throw;
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
            SqlConnection connection = new(App.ConnectionString);

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
            try
            {
                using SqlConnection connection = new(App.ConnectionString);
                connection.Open();

                string sql = LoadSqlScript(CheckTablesExistScriptPath);

                using SqlCommand command = new(sql, connection);
                return Convert.ToBoolean(command.ExecuteScalar() ?? false);
            }
            catch (FileNotFoundException ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL script file not found: {ex.Message}");
                throw;
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking table existence: {ex.Message}");
                throw;
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
                using SqlConnection connection = new(masterConnection);
                connection.Open();

                string sql = LoadSqlScript(ResetDatabaseScriptPath);

                using SqlCommand command = new(sql, connection);
                command.ExecuteNonQuery();
            }
            catch (FileNotFoundException ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL script file not found: {ex.Message}");
                throw;
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error resetting database: {ex.Message}");
                throw;
            }
        }

        private static void CreateDatabaseTables()
        {
            try
            {
                using SqlConnection connection = new(App.ConnectionString);
                connection.Open();

                string sql = LoadSqlScript(CreateTablesScriptPath);

                using SqlCommand command = new(sql, connection);
                command.ExecuteNonQuery();
            }
            catch (FileNotFoundException ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL script file not found: {ex.Message}");
                throw;
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating tables: {ex.Message}");
                throw;
            }
        }

        private static string LoadSqlScript(string relativePath)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(baseDirectory, relativePath);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"SQL script file not found: {fullPath}");
            }

            return File.ReadAllText(fullPath);
        }
    }
}
