namespace StockApp.Repository
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.SqlClient;
    using StockApp.Database;
    using StockApp.Models;

    internal class BaseStocksRepository : IBaseStocksRepository
    {
        private readonly List<IBaseStock> stocks = [];
        private readonly SqlConnection dbConnection = DatabaseHelper.GetConnection();

        public BaseStocksRepository()
        {
            this.LoadStocks();
        }

        // Helper: Execute a SQL query with parameters
        private void ExecuteSql(string query, Action<SqlCommand> parameterize, SqlTransaction? transaction = null)
        {
            using var command = new SqlCommand(query, this.dbConnection, transaction);
            parameterize?.Invoke(command);
            command.ExecuteNonQuery();
        }

        // Helper: Execute a query and return a scalar value
        private T ExecuteScalar<T>(string query, Action<SqlCommand> parameterize, SqlTransaction? transaction = null)
        {
            using var command = new SqlCommand(query, this.dbConnection, transaction);
            parameterize?.Invoke(command);
            return (T)Convert.ChangeType(command.ExecuteScalar(), typeof(T));
        }

        public void AddStock(IBaseStock stock, int initialPrice = 100)
        {
            using var transaction = this.dbConnection.BeginTransaction();

            try
            {
                // Check for duplicate stock name
                string checkQuery = "SELECT COUNT(*) FROM STOCK WHERE STOCK_NAME = @StockName";
                int count = this.ExecuteScalar<int>(checkQuery, command =>
                {
                    command.Parameters.AddWithValue("@StockName", stock.Name);
                }, transaction);

                if (count > 0)
                {
                    throw new Exception("A stock with this name already exists!");
                }

                // Insert the stock
                string stockQuery = "INSERT INTO STOCK (STOCK_NAME, STOCK_SYMBOL, AUTHOR_CNP) VALUES (@StockName, @StockSymbol, @AuthorCNP)";
                this.ExecuteSql(stockQuery, command =>
                {
                    command.Parameters.AddWithValue("@StockName", stock.Name);
                    command.Parameters.AddWithValue("@StockSymbol", stock.Symbol);
                    command.Parameters.AddWithValue("@AuthorCNP", stock.AuthorCnp);
                }, transaction);

                // Insert the initial stock value
                string valueQuery = "INSERT INTO STOCK_VALUE (STOCK_NAME, PRICE) VALUES (@StockName, @Price)";
                this.ExecuteSql(valueQuery, command =>
                {
                    command.Parameters.AddWithValue("@StockName", stock.Name);
                    command.Parameters.AddWithValue("@Price", initialPrice);
                }, transaction);

                // Commit the transaction and update in-memory list
                transaction.Commit();
                this.stocks.Add(stock);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception($"Failed to add stock: {ex.Message}");
            }
        }

        public void LoadStocks()
        {
            string query = "SELECT STOCK_NAME, STOCK_SYMBOL, AUTHOR_CNP FROM STOCK";

            using SqlCommand command = new (query, this.dbConnection);
            using var reader = command.ExecuteReader();
            this.stocks.Clear();

            while (reader.Read())
            {
                var stock = new BaseStock(
                    reader["STOCK_NAME"]?.ToString() ?? string.Empty,
                    reader["STOCK_SYMBOL"]?.ToString() ?? string.Empty,
                    reader["AUTHOR_CNP"]?.ToString() ?? string.Empty
                );
                this.stocks.Add(stock);
            }
        }

        public IReadOnlyList<IBaseStock> GetAllStocks() => [.. this.stocks];
    }
}