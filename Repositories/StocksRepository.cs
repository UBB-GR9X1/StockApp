using StockApp.Database;
using StockApp.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace StockApp.Repositories
{
    internal class StocksRepository
    {
        private List<BaseStock> stocks = new List<BaseStock>();
        private string connectionString = DatabaseHelper.getConnectionString();

        public StocksRepository() {
            LoadStocks();
        }

        public void AddStock(BaseStock stock)
        {
            stocks.Add(stock);
        }

        public void LoadStocks()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT STOCK_NAME, STOCK_SYMBOL, AUTHOR_CNP FROM STOCK";

                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var stockName = reader["STOCK_NAME"]?.ToString() ?? string.Empty;
                            var stockSymbol = reader["STOCK_SYMBOL"]?.ToString() ?? string.Empty;
                            var authorCnp = reader["AUTHOR_CNP"]?.ToString() ?? string.Empty;

                            var stock = new BaseStock(stockName, stockSymbol, authorCnp);
                            AddStock(stock);
                        }
                    }
                }
            }
        }

        public List<BaseStock> GetAllStocks()
        {
            return stocks;
        }

    }
}
