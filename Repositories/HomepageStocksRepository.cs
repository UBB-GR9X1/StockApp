using System;
using System.Collections.Generic;
using System.Data.SQLite;
using StocksHomepage.Model;
using StockApp.Database;

namespace StocksHomepage.Repositories
{
    internal class HomepageStocksRepository
    {
        private SQLiteConnection dbConnection = DatabaseHelper.Instance.GetConnection();
        private string userCNP;

        public HomepageStocksRepository()
        {
            this.userCNP = GetUserCNP();
            //RemoveValuesFromDB();
            //PopulateDatabase();
            LoadStocks();
        }
            public string GetUserCNP()
                {
                string insertQuery = "INSERT OR IGNORE INTO HARDCODED_CNPS (CNP) VALUES ('1234567890')";
                using (var insertCommand = new SQLiteCommand(insertQuery, dbConnection))
                {
                    insertCommand.ExecuteNonQuery();
                }
                string query = "SELECT CNP FROM HARDCODED_CNPS";

                using (var command = new SQLiteCommand(query, dbConnection))
                {
                    command.ExecuteNonQuery();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return reader["CNP"].ToString();
                        }
                    }
                }
                return null;
              }
        public void RemoveValuesFromDB()
        {
            using (var command = new SQLiteCommand(dbConnection))
            {
                command.CommandText = "DELETE FROM STOCK";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM STOCK_VALUE";
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM FAVORITE_STOCK";
                command.ExecuteNonQuery();
            }
        }
        public void PopulateDatabase()
        {
            using (var command = new SQLiteCommand(dbConnection))
            {
                var stocks = new List<(string Symbol, string Name, double Price, bool isFavorite)>
        {
            ("AAPL", "Apple Inc.", 175.00, true),
            ("MSFT", "Microsoft Corp.", 320.00, true),
            ("NVDA", "NVIDIA Corporation", 600.00, true),
            ("TSM", "Taiwan Semiconductor", 110.00, true),
            ("GOOGL", "Alphabet Inc.", 2800.00, false),
            ("AMZN", "Amazon.com Inc.", 3500.00, false),
            ("TSLA", "Tesla Inc.", 700.00, false),
            ("META", "Meta Platforms, Inc.", 340.00, false),
            ("DIS", "The Walt Disney Company", 190.00, false),
            ("NFLX", "Netflix, Inc.", 500.00, false),
            ("INTC", "Intel Corporation", 50.00, false),
            ("CSCO", "Cisco Systems, Inc.", 55.00, false),
            ("QCOM", "QUALCOMM Incorporated", 150.00, false),
            ("IBM", "IBM Corporation", 120.00, false),
            ("ORCL", "Oracle Corporation", 80.00, false),
            ("ADBE", "Adobe Inc.", 600.00, false),
            ("CRM", "Salesforce.com, Inc.", 250.00, false),
            ("NOW", "ServiceNow, Inc.", 500.00, false),
            ("SAP", "SAP SE", 150.00, false),
            ("UBER", "Uber Technologies, Inc.", 40.00, false),
            ("LYFT", "Lyft, Inc.", 50.00, false),
            ("ZM", "Zoom Video Communications, Inc.", 200.00, false),
            ("DOCU", "DocuSign, Inc.", 150.00, false)
        };

                foreach (var stock in stocks)
                {
                    // Insert stock if not exists
                    command.CommandText = "INSERT OR IGNORE INTO STOCK (STOCK_NAME, STOCK_SYMBOL, AUTHOR_CNP) VALUES (@Name, @Symbol, @AuthorCNP)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Name", stock.Name);
                    command.Parameters.AddWithValue("@Symbol", stock.Symbol);
                    command.Parameters.AddWithValue("@AuthorCNP", userCNP);
                    command.ExecuteNonQuery();

                    // Ensure only latest stock price exists
                    command.CommandText = "DELETE FROM STOCK_VALUE WHERE STOCK_NAME = @Name";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Name", stock.Name);
                    command.ExecuteNonQuery();

                    // Insert latest stock price
                    command.CommandText = "INSERT INTO STOCK_VALUE (STOCK_NAME, PRICE) VALUES (@Name, @Price)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Name", stock.Name);
                    command.Parameters.AddWithValue("@Price", stock.Price);
                    command.ExecuteNonQuery();

                    // Insert or update favorite stocks
                    command.CommandText = "INSERT OR REPLACE INTO FAVORITE_STOCK (USER_CNP, STOCK_NAME, IS_FAVORITE) VALUES (@UserCNP, @Name, @IsFavorite)";
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@UserCNP", userCNP);
                    command.Parameters.AddWithValue("@Name", stock.Name);
                    command.Parameters.AddWithValue("@IsFavorite", stock.isFavorite ? 1 : 0);
                    command.ExecuteNonQuery();
                }
            }
        }
        public List<HomepageStock> LoadStocks()
        {
            List<HomepageStock> stocks = new List<HomepageStock>();

            string query = @"
                            SELECT s.STOCK_NAME, s.STOCK_SYMBOL, 
                                   COALESCE(v.PRICE, 0) AS PRICE,
                                   COALESCE(f.IS_FAVORITE, 0) AS IS_FAVORITE,
                                   (COALESCE(v.PRICE, 0) - COALESCE(prev.PRICE, 0)) AS CHANGE_VALUE
                            FROM STOCK s
                            LEFT JOIN STOCK_VALUE v 
                                ON s.STOCK_NAME = v.STOCK_NAME
                                AND v.ROWID = (SELECT MAX(ROWID) FROM STOCK_VALUE WHERE STOCK_NAME = s.STOCK_NAME)
                            LEFT JOIN STOCK_VALUE prev 
                                ON s.STOCK_NAME = prev.STOCK_NAME
                                AND prev.ROWID = (SELECT MAX(ROWID) FROM STOCK_VALUE WHERE STOCK_NAME = s.STOCK_NAME AND ROWID < v.ROWID)
                            LEFT JOIN FAVORITE_STOCK f 
                                ON s.STOCK_NAME = f.STOCK_NAME 
                                AND f.USER_CNP = @UserCNP";

            using (var command = new SQLiteCommand(query, dbConnection))
            {
                command.Parameters.AddWithValue("@UserCNP", userCNP);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int changeValue = Convert.ToInt32(reader["CHANGE_VALUE"]);
                        var stockName = reader["STOCK_NAME"]?.ToString();
                        var stockSymbol = reader["STOCK_SYMBOL"]?.ToString();
                        var stockPrice = Convert.ToInt32(reader["PRICE"]);
                        var stockChange = changeValue >= 0 ? "+" + changeValue.ToString() : changeValue.ToString();
                        var isFavorite = Convert.ToInt32(reader["IS_FAVORITE"]) == 1;
                        var stock = new HomepageStock
                        {
                            Symbol = stockSymbol,
                            Name = stockName,
                            Price = stockPrice,
                            Change = stockChange,
                            isFavorite = isFavorite
                        };
                        stocks.Add(stock);
                    }
                }
            }
            return stocks;
        }

        public void AddToFavorites(HomepageStock stock)
        {
            using (var command = new SQLiteCommand(dbConnection))
            {
                command.CommandText = "INSERT OR REPLACE INTO FAVORITE_STOCK (USER_CNP, STOCK_NAME, IS_FAVORITE) VALUES (@UserCNP, @Name, 1)";
                command.Parameters.AddWithValue("@UserCNP", userCNP);
                command.Parameters.AddWithValue("@Name", stock.Name);
                command.ExecuteNonQuery();
            }
        }

        public void RemoveFromFavorites(HomepageStock stock)
        {
            using (var command = new SQLiteCommand(dbConnection))
            {
                command.CommandText = "DELETE FROM FAVORITE_STOCK WHERE USER_CNP = @UserCNP AND STOCK_NAME = @Name";
                command.Parameters.AddWithValue("@UserCNP", userCNP);
                command.Parameters.AddWithValue("@Name", stock.Name);
                command.ExecuteNonQuery();
            }
        }

    }
}
