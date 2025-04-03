using StockApp.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;

namespace TransactionLog
{
    public class TransactionRepository
    {
        private readonly List<Transaction> transactions = [];

        public TransactionRepository()
        {
            string connectionString = DatabaseHelper.Instance.GetConnection().ConnectionString;
            string query = "SELECT * FROM USERS_TRANSACTION";
            string queryForSymbol = "SELECT STOCK_SYMBOL FROM STOCK WHERE STOCK_NAME = @stock";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string stockName = reader["STOCK_NAME"].ToString();

                        string stockSymbol = "";
                        using (SqlCommand command2 = new SqlCommand(queryForSymbol, connection))
                        {
                            command2.Parameters.AddWithValue("@stock", stockName);
                            using (SqlDataReader reader2 = command2.ExecuteReader())
                            {
                                if (reader2.Read())
                                {
                                    stockSymbol = reader2["STOCK_SYMBOL"].ToString();
                                }
                            }
                        }

                        string stockType = reader["TYPE"].ToString();
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
    }
}
