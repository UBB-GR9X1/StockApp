using StockApp.Database;
using StockApp.Model;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace StockApp.Repositories
{
    internal class BaseStocksRepository
    {
        private List<BaseStock> stocks = new List<BaseStock>();
        private SqlConnection dbConnection = DatabaseHelper.Instance.GetConnection();

        public BaseStocksRepository()
        {
            LoadStocks();
        }

        public void AddStock(BaseStock stock)
        {
            string checkQuery = "SELECT COUNT(*) FROM STOCK WHERE STOCK_NAME = @StockName";
            using (var checkCommand = new SqlCommand(checkQuery, dbConnection))
            {
                checkCommand.Parameters.AddWithValue("@StockName", stock.Name);
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (count > 0)
                {
                    throw new Exception("A stock with this name already exists!");
                }
            }

            string query = "INSERT INTO STOCK (STOCK_NAME, STOCK_SYMBOL, AUTHOR_CNP) VALUES (@StockName, @StockSymbol, @AuthorCNP)";
            using (var command = new SqlCommand(query, dbConnection))
            {
                command.Parameters.AddWithValue("@StockName", stock.Name);
                command.Parameters.AddWithValue("@StockSymbol", stock.Symbol);
                command.Parameters.AddWithValue("@AuthorCNP", stock.AuthorCNP);
                command.ExecuteNonQuery();
            }

            stocks.Add(stock);
        }

        public void LoadStocks()
        {
            string query = "SELECT STOCK_NAME, STOCK_SYMBOL, AUTHOR_CNP FROM STOCK";
            using (var command = new SqlCommand(query, dbConnection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var stockName = reader["STOCK_NAME"]?.ToString() ?? string.Empty;
                        var stockSymbol = reader["STOCK_SYMBOL"]?.ToString() ?? string.Empty;
                        var authorCnp = reader["AUTHOR_CNP"]?.ToString() ?? string.Empty;

                        var stock = new BaseStock(stockName, stockSymbol, authorCnp);
                        stocks.Add(stock);
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
