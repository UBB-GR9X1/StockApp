using StockApp.Database;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace TransactionLog
{
    public class TransactionRepository
    {
        private readonly List<Transaction> transactions = [];

        public TransactionRepository()
        {
            string connectionString = DatabaseHelper.Instance.GetConnection().ConnectionString;
            string query = @"
            SELECT t.*, s.STOCK_SYMBOL
            FROM USERS_TRANSACTION t
            JOIN STOCK s ON t.STOCK_NAME = s.STOCK_NAME";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
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

                        transactions.Add(new Transaction(stockSymbol, stockName, stockType, amount, pricePerStock, date, author));
                    }
                }
            }

        }

        public List<Transaction> GetAll()
        {
            return [.. transactions];
        }

        public List<Transaction> GetByFilterCriteria(TransactionFilterCriteria criteria)
        {
            return [.. transactions.Where(transaction =>
                (string.IsNullOrEmpty(criteria.StockName) || transaction.StockName.Equals(criteria.StockName)) &&
                (string.IsNullOrEmpty(criteria.Type) || transaction.Type.Equals(criteria.Type)) &&
                (!criteria.MinTotalValue.HasValue || transaction.TotalValue >= criteria.MinTotalValue) &&
                (!criteria.MaxTotalValue.HasValue || transaction.TotalValue <= criteria.MaxTotalValue) &&
                (!criteria.StartDate.HasValue || transaction.Date >= criteria.StartDate) &&
                (!criteria.EndDate.HasValue || transaction.Date <= criteria.EndDate)
            )];
        }

        public void AddTransaction(Transaction transaction)
        {
            string connectionString = DatabaseHelper.Instance.GetConnection().ConnectionString;

            string insertQuery = @"
                INSERT INTO USERS_TRANSACTION (STOCK_NAME, TYPE, QUANTITY, PRICE, DATE, USER_CNP)
                VALUES (@stockName, @type, @quantity, @price, @date, @userCnp)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Optional: Ensure stock exists
                string checkStockQuery = "SELECT COUNT(*) FROM STOCK WHERE STOCK_NAME = @stockName";
                using (SqlCommand checkCommand = new SqlCommand(checkStockQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@stockName", transaction.StockName);
                    int stockExists = (int)checkCommand.ExecuteScalar();
                    if (stockExists == 0)
                    {
                        throw new Exception($"Stock with name '{transaction.StockName}' does not exist.");
                    }
                }

                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@stockName", transaction.StockName);
                    command.Parameters.AddWithValue("@type", transaction.Type.ToUpper() == "BUY"); // true if BUY, false if SELL
                    command.Parameters.AddWithValue("@quantity", transaction.Amount);
                    command.Parameters.AddWithValue("@price", transaction.PricePerStock);
                    command.Parameters.AddWithValue("@date", transaction.Date);
                    command.Parameters.AddWithValue("@userCnp", transaction.Author);

                    command.ExecuteNonQuery();
                }

                // Add to in-memory list
                transactions.Add(transaction);
            }
        }
    }
}
