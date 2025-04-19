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

    /// <summary>
    /// Repository for managing <see cref="BaseStock"/> entities, including addition and retrieval from the database.
    /// </summary>
    internal class BaseStocksRepository : IBaseStocksRepository
    {
        private readonly List<BaseStock> stocks = [];
        private readonly SqlConnection dbConnection = DatabaseHelper.GetConnection();

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseStocksRepository"/> class and loads all stocks into memory.
        /// </summary>
        public BaseStocksRepository()
        {
            this.LoadStocks();
        }

        /// <summary>
        /// Executes a non-query SQL command with optional parameterization and transaction.
        /// </summary>
        /// <param name="query">The SQL command text to execute.</param>
        /// <param name="parameterize">Action to add parameters to the <see cref="SqlCommand"/>.</param>
        /// <param name="transaction">Optional <see cref="SqlTransaction"/> for transactional execution.</param>
        private void ExecuteSql(string query, Action<SqlCommand> parameterize, SqlTransaction? transaction = null)
        {
            try
            {
                using var command = new SqlCommand(query, this.dbConnection, transaction);

                // Inline: apply parameters before execution
                parameterize?.Invoke(command);
                command.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                // FIXME: consider more granular error handling based on SQL error codes
                throw new StockRepositoryException("Database error while executing a stock-related SQL command.", sqlEx);
            }
        }

        /// <summary>
        /// Executes a SQL scalar query and converts the result to the specified type.
        /// </summary>
        /// <typeparam name="T">The expected return type.</param>
        /// <param name="query">The SQL query text to execute.</param>
        /// <param name="parameterize">Action to add parameters to the <see cref="SqlCommand"/>.</param>
        /// <param name="transaction">Optional <see cref="SqlTransaction"/> for transactional execution.</param>
        /// <returns>The scalar result converted to type <typeparamref name="T"/>.</returns>
        private T ExecuteScalar<T>(string query, Action<SqlCommand> parameterize, SqlTransaction? transaction = null)
        {
            try
            {
                using var command = new SqlCommand(query, this.dbConnection, transaction);

                // Inline: apply parameters before execution
                parameterize?.Invoke(command);
                return (T)Convert.ChangeType(command.ExecuteScalar(), typeof(T));
            }
            catch (SqlException sqlEx)
            {
                throw new StockRepositoryException("Database error while executing scalar query.", sqlEx);
            }
            catch (InvalidCastException castEx)
            {
                // FIXME: handle null or unexpected types gracefully
                throw new StockRepositoryException("Failed to cast result from scalar query.", castEx);
            }
        }

        /// <summary>
        /// Adds a new stock to the database and in-memory collection.
        /// </summary>
        /// <param name="stock">The <see cref="BaseStock"/> to add.</param>
        /// <param name="initialPrice">Initial price for the stock.</param>
        public void AddStock(BaseStock stock, int initialPrice = 100)
        {
            using var transaction = this.dbConnection.BeginTransaction();

            try
            {
                // Inline: check for duplicate stock name before insertion
                const string checkQuery = "SELECT COUNT(*) FROM STOCK WHERE STOCK_NAME = @StockName";
                int count = this.ExecuteScalar<int>(checkQuery, cmd =>
                {
                    cmd.Parameters.AddWithValue("@stockName", stock.Name);
                }, transaction);

                if (count > 0)
                {
                    // Inline: rollback if duplicate found
                    throw new DuplicateStockException(stock.Name);
                }

                string insertStock = "INSERT INTO STOCK (STOCK_NAME, STOCK_SYMBOL, AUTHOR_CNP) VALUES (@stockName, @stockSymbol, @authorCnp)";
                this.ExecuteSql(insertStock, cmd =>
                {
                    cmd.Parameters.AddWithValue("@stockName", stock.Name);
                    cmd.Parameters.AddWithValue("@stockSymbol", stock.Symbol);
                    cmd.Parameters.AddWithValue("@authorCnp", stock.AuthorCNP);
                }, transaction);

                string insertValue = "INSERT INTO STOCK_VALUE (STOCK_NAME, PRICE) VALUES (@stockName, @price)";
                this.ExecuteSql(insertValue, cmd =>
                {
                    cmd.Parameters.AddWithValue("@stockName", stock.Name);
                    cmd.Parameters.AddWithValue("@price", initialPrice);
                }, transaction);

                transaction.Commit();

                // Inline: update in-memory collection after commit
                this.stocks.Add(stock);
            }
            catch (DuplicateStockException)
            {
                transaction.Rollback();
                throw;
            }
            catch (Exception ex)
            {
                // Inline: rollback on any unexpected exception
                transaction.Rollback();
                throw new StockRepositoryException("Failed to add stock to repository.", ex);
            }
        }

        /// <summary>
        /// Loads all stocks from the database into the in-memory list.
        /// </summary>
        public void LoadStocks()
        {
            const string query = "SELECT STOCK_NAME, STOCK_SYMBOL, AUTHOR_CNP FROM STOCK";

            try
            {
                using SqlCommand command = new(query, this.dbConnection);
                using var reader = command.ExecuteReader();
                this.stocks.Clear();

                // Inline: map each row to a BaseStock object
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
                // FIXME: consider retry logic or fallback caching
                throw new StockRepositoryException("Failed to load stocks from database.", ex);
            }
        }

        /// <summary>
        /// Retrieves all stocks from the in-memory collection.
        /// </summary>
        /// <returns>A list of all <see cref="BaseStock"/> objects.</returns>
        public List<BaseStock> GetAllStocks() => [.. this.stocks];
    }
}
