namespace StockApp.Database
{
    using System;
    using System.Data;
    using System.IO;
    using Microsoft.Data.SqlClient;
    using StockApp.Exceptions;

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
                throw new InvalidOperationException("Connection string is not initialized.");
            }

            try
            {
                bool tablesExist = CheckIfTablesExist();
                if (!tablesExist)
                {
                    CreateDatabaseTables();
                }
            }
            catch (SqlScriptMissingException ex)
            {
                throw new DatabaseInitializationException("Missing required SQL script during initialization.", ex);
            }
            catch (SqlException ex)
            {
                throw new DatabaseInitializationException("SQL error during database initialization.", ex);
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
                throw new DatabaseInitializationException("Failed to open SQL connection.", ex);
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
                throw new SqlScriptMissingException($"SQL script file '{CheckTablesExistScriptPath}' was not found.", ex);
            }
            catch (SqlException ex)
            {
                throw new DatabaseInitializationException("An error occurred while checking if database tables exist.", ex);
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
                throw new SqlScriptMissingException(ResetDatabaseScriptPath, ex);
            }
            catch (SqlException ex)
            {
                throw new DatabaseInitializationException("Failed to reset the database.", ex);
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
                throw new SqlScriptMissingException(CreateTablesScriptPath, ex);
            }
            catch (SqlException ex)
            {
                throw new DatabaseInitializationException("Failed to create database tables.", ex);
            }
        }


        private static string LoadSqlScript(string relativePath)
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

            if (!File.Exists(fullPath))
            {
                throw new SqlScriptMissingException(fullPath);
            }

            return File.ReadAllText(fullPath);
        }

    }
}
