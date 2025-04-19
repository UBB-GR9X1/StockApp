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
    /// Repository for managing transactions in the application.
    /// </summary>
    public class TransactionRepository : ITransactionRepository
    {
        /// <summary>
        /// Gets the in-memory list of transaction logs.
        /// </summary>
        // FIXME: The list initializer "[]" may not compile in C#. Consider using "new List<TransactionLogTransaction>()" instead.
        public List<TransactionLogTransaction> Transactions { get; private set; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionRepository"/> class
        /// and loads all existing transactions from the database.
        /// </summary>
        public TransactionRepository()
        {
            // Query to select transaction records along with their stock symbols
            string query = @"
            SELECT t.*, s.STOCK_SYMBOL
            FROM USERS_TRANSACTION t
            JOIN STOCK s ON t.STOCK_NAME = s.STOCK_NAME";

            using SqlConnection connection = DatabaseHelper.GetConnection();
            using SqlCommand command = new(query, connection);
            using SqlDataReader reader = command.ExecuteReader();

            // Read each record and map to TransactionLogTransaction objects
            while (reader.Read())
            {
                string stockName = reader["STOCK_NAME"].ToString();
                string stockSymbol = reader["STOCK_SYMBOL"].ToString();

                bool isBuy = Convert.ToBoolean(reader["TYPE"]);
                string stockType = isBuy ? "BUY" : "SELL";

                int amount = Convert.ToInt32(reader["QUANTITY"]);
                int pricePerStock = Convert.ToInt32(reader["PRICE"]);
                DateTime date = DateTime.Parse(reader["DATE"].ToString());
                string author = reader["USER_CNP"].ToString();

                // Add the mapped transaction to the in-memory list
                this.Transactions.Add(
                    new TransactionLogTransaction(
                        stockSymbol,
                        stockName,
                        stockType,
                        amount,
                        pricePerStock,
                        date,
                        author));
            }
        }

        /// <summary>
        /// Retrieves transactions that match the given filter criteria.
        /// </summary>
        /// <param name="criteria">Filtering options for stock name, type, values, and date range.</param>
        /// <returns>List of transactions matching the criteria.</returns>
        public List<TransactionLogTransaction> GetByFilterCriteria(TransactionFilterCriteria criteria)
        {
            // Use LINQ to apply all filter predicates in one query
            return [.. this.Transactions.Where(transaction =>
                (string.IsNullOrEmpty(criteria.StockName) || transaction.StockName.Equals(criteria.StockName)) &&
                (string.IsNullOrEmpty(criteria.Type) || transaction.Type.Equals(criteria.Type)) &&
                (!criteria.MinTotalValue.HasValue || transaction.TotalValue >= criteria.MinTotalValue) &&
                (!criteria.MaxTotalValue.HasValue || transaction.TotalValue <= criteria.MaxTotalValue) &&
                (!criteria.StartDate.HasValue || transaction.Date >= criteria.StartDate) &&
                (!criteria.EndDate.HasValue || transaction.Date <= criteria.EndDate))];
        }

        /// <summary>
        /// Adds a new transaction to both the database and the in-memory list.
        /// </summary>
        /// <param name="transaction">The transaction to add.</param>
        /// <exception cref="TransactionRepositoryException">
        /// Thrown if the referenced stock does not exist in the database.
        /// </exception>
        public void AddTransaction(TransactionLogTransaction transaction)
        {
            string connectionString = DatabaseHelper.GetConnection().ConnectionString;

            string insertQuery = @"
                INSERT INTO USERS_TRANSACTION (STOCK_NAME, TYPE, QUANTITY, PRICE, DATE, USER_CNP)
                VALUES (@stockName, @type, @quantity, @price, @date, @userCnp)";

            using SqlConnection connection = new(connectionString);
            connection.Open();

            // Ensure the stock exists before inserting the transaction
            string checkStockQuery = "SELECT COUNT(*) FROM STOCK WHERE STOCK_NAME = @stockName";
            using (SqlCommand checkCommand = new(checkStockQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@stockName", transaction.StockName);
                int stockExists = (int)checkCommand.ExecuteScalar();

                if (stockExists == 0)
                {
                    // Throw if stock not found
                    throw new TransactionRepositoryException(
                        $"Stock with name '{transaction.StockName}' does not exist.");
                }
            }

            // Insert the transaction record into the database
            using (SqlCommand command = new(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@stockName", transaction.StockName);

                // True if BUY, false if SELL
                command.Parameters.AddWithValue(
                    "@type",
                    transaction.Type.Equals("BUY", StringComparison.CurrentCultureIgnoreCase));
                command.Parameters.AddWithValue("@quantity", transaction.Amount);
                command.Parameters.AddWithValue("@price", transaction.PricePerStock);
                command.Parameters.AddWithValue("@date", transaction.Date);
                command.Parameters.AddWithValue("@userCnp", transaction.Author);

                command.ExecuteNonQuery();
            }

            // Add the new transaction to the in-memory list
            this.Transactions.Add(transaction);
        }
    }
}
