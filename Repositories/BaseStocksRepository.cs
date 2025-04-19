using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("StockApp.Repository.Tests")]
namespace StockApp.Repositories
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.SqlClient;
    using StockApp.Database;
    using StockApp.Exceptions;
    using StockApp.Models;

    internal class BaseStocksRepository : IBaseStocksRepository
    {
        private readonly List<BaseStock> stocks = [];
        private readonly SqlConnection dbConnection = DatabaseHelper.GetConnection();

        public BaseStocksRepository()
        {
            this.LoadStocks();
        }

        private void ExecuteSql(string query, Action<SqlCommand> parameterize, SqlTransaction? transaction = null)
        {
            try
            {
                using var command = new SqlCommand(query, this.dbConnection, transaction);
                parameterize?.Invoke(command);
                command.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                throw new StockRepositoryException("Database error while executing a stock-related SQL command.", sqlEx);
            }
        }

        private T ExecuteScalar<T>(string query, Action<SqlCommand> parameterize, SqlTransaction? transaction = null)
        {
            try
            {
                using var command = new SqlCommand(query, this.dbConnection, transaction);
                parameterize?.Invoke(command);
                return (T)Convert.ChangeType(command.ExecuteScalar(), typeof(T));
            }
            catch (SqlException sqlEx)
            {
                throw new StockRepositoryException("Database error while executing scalar query.", sqlEx);
            }
            catch (InvalidCastException castEx)
            {
                throw new StockRepositoryException("Failed to cast result from scalar query.", castEx);
            }
        }

        public void AddStock(BaseStock stock, int initialPrice = 100)
        {
            using var transaction = this.dbConnection.BeginTransaction();

            try
            {
                string checkQuery = "SELECT COUNT(*) FROM STOCK WHERE STOCK_NAME = @StockName";
                int count = this.ExecuteScalar<int>(checkQuery, cmd =>
                {
                    cmd.Parameters.AddWithValue("@StockName", stock.Name);
                }, transaction);

                if (count > 0)
                {
                    throw new DuplicateStockException(stock.Name);
                }

                string insertStock = "INSERT INTO STOCK (STOCK_NAME, STOCK_SYMBOL, AUTHOR_CNP) VALUES (@StockName, @StockSymbol, @AuthorCNP)";
                this.ExecuteSql(insertStock, cmd =>
                {
                    cmd.Parameters.AddWithValue("@StockName", stock.Name);
                    cmd.Parameters.AddWithValue("@StockSymbol", stock.Symbol);
                    cmd.Parameters.AddWithValue("@AuthorCNP", stock.AuthorCnp);
                }, transaction);

                string insertValue = "INSERT INTO STOCK_VALUE (STOCK_NAME, PRICE) VALUES (@StockName, @Price)";
                this.ExecuteSql(insertValue, cmd =>
                {
                    cmd.Parameters.AddWithValue("@StockName", stock.Name);
                    cmd.Parameters.AddWithValue("@Price", initialPrice);
                }, transaction);

                transaction.Commit();
                this.stocks.Add(stock);
            }
            catch (DuplicateStockException)
            {
                transaction.Rollback();
                throw;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new StockRepositoryException("Failed to add stock to repository.", ex);
            }
        }

        public void LoadStocks()
        {
            string query = "SELECT STOCK_NAME, STOCK_SYMBOL, AUTHOR_CNP FROM STOCK";

            try
            {
                using SqlCommand command = new(query, this.dbConnection);
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
            catch (SqlException ex)
            {
                throw new StockRepositoryException("Failed to load stocks from database.", ex);
            }
        }

        public List<BaseStock> GetAllStocks() => [.. this.stocks];
    }
}
