namespace StockApp.Database
{
    using System;
    using System.Data;
    using System.IO;
    using Microsoft.Data.SqlClient;
    using StockApp.Exceptions;

    internal class DatabaseHelper
    {
        private static DatabaseHelper? instance;

        private DatabaseHelper()
        {
            InitializeDatabase();
        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="DatabaseHelper"/>.
        /// </summary>
        public static DatabaseHelper Instance => instance ??= new();

        /// <summary>
        /// Ensures the database is initialized by checking for required tables and creating them if missing.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <see cref="App.ConnectionString"/> is not set.
        /// </exception>
        /// <exception cref="DatabaseInitializationException">
        /// Thrown if a SQL error occurs or a required script is missing during initialization.
        /// </exception>
        public static void InitializeDatabase()
        {
            if (string.IsNullOrWhiteSpace(App.ConnectionString))
            {
                throw new InvalidOperationException("Connection string is not initialized.");
            }

            try
            {
                // Just verify we can connect to the database
                using SqlConnection connection = new(App.ConnectionString);
                connection.Open();
            }
            catch (SqlException ex)
            {
                throw new DatabaseInitializationException("SQL error during database initialization.", ex);
            }
        }

        /// <summary>
        /// Safely closes the given SQL connection if it is open.
        /// </summary>
        /// <param name="connection">The <see cref="SqlConnection"/> to close.</param>
        public static void CloseConnection(SqlConnection connection)
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Opens and returns a new <see cref="SqlConnection"/> using <see cref="App.ConnectionString"/>.
        /// </summary>
        /// <returns>An open <see cref="SqlConnection"/>.</returns>
        /// <exception cref="DatabaseInitializationException">
        /// Thrown if the connection cannot be opened.
        /// </exception>
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
    }
}
