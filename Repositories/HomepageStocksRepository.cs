using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;
using StocksHomepage.Model;
using StockApp.Database;
using Microsoft.Data.SqlClient;

namespace StocksHomepage.Repositories
{
    internal class HomepageStocksRepository
    {
        private SqlConnection dbConnection = DatabaseHelper.Instance.GetConnection();
        private string userCNP;

        public string getCNP()
        {
            return this.userCNP;
        }

        public HomepageStocksRepository()
        {
            //this.userCNP = GetUserCNP();
            this.userCNP = "5050225";
            Console.WriteLine("User CNP: " + userCNP);
            Console.WriteLine("IsGuestUser: " + IsGuestUser(userCNP));
            LoadStocks();
        }
        public bool IsGuestUser(string userCNP)
        {
            string query = "SELECT COUNT(*) FROM HARDCODED_CNPS WHERE CNP = @UserCNP";
            using (var command = new SqlCommand(query, dbConnection))
            {
                command.Parameters.AddWithValue("@UserCNP", userCNP);
                int count = (int)command.ExecuteScalar();
                Console.WriteLine("Count: " + count);
                return count == 0; 
            }
        }
        
        public string GetUserCNP()
                {
                string query = "SELECT CNP FROM HARDCODED_CNPS";

                using (var command = new SqlCommand(query, dbConnection))
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
        
        public List<HomepageStock> LoadStocks()
        {
            List<HomepageStock> stocks = new List<HomepageStock>();
            string query = @"WITH LatestStockValue AS (
                                SELECT 
                                    STOCK_NAME, 
                                    PRICE
                                FROM STOCK_VALUE sv1
                                WHERE PRICE = (SELECT MAX(PRICE) FROM STOCK_VALUE sv2 WHERE sv1.STOCK_NAME = sv2.STOCK_NAME)
                            ),
                            PreviousStockValue AS (
                                SELECT 
                                    STOCK_NAME, 
                                    PRICE
                                FROM STOCK_VALUE sv1
                                WHERE PRICE = (SELECT MAX(PRICE) 
                                               FROM STOCK_VALUE sv2 
                                               WHERE sv1.STOCK_NAME = sv2.STOCK_NAME 
                                               AND sv2.PRICE < (SELECT MAX(PRICE) FROM STOCK_VALUE sv3 WHERE sv3.STOCK_NAME = sv2.STOCK_NAME))
                            )
                            SELECT 
                                s.STOCK_NAME, 
                                s.STOCK_SYMBOL, 
                                COALESCE(lsv.PRICE, 0) AS PRICE,
                                COALESCE(f.IS_FAVORITE, 0) AS IS_FAVORITE,
                                (COALESCE(lsv.PRICE, 0) - COALESCE(psv.PRICE, 0)) AS CHANGE_VALUE
                            FROM STOCK s
                            LEFT JOIN LatestStockValue lsv ON s.STOCK_NAME = lsv.STOCK_NAME
                            LEFT JOIN PreviousStockValue psv ON s.STOCK_NAME = psv.STOCK_NAME
                            LEFT JOIN FAVORITE_STOCK f ON s.STOCK_NAME = f.STOCK_NAME AND f.USER_CNP = @UserCNP;
                            ";

            using (var command = new SqlCommand(query, dbConnection))
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
            var query = "INSERT INTO FAVORITE_STOCK (USER_CNP, STOCK_NAME, IS_FAVORITE) VALUES (@UserCNP, @Name, 1)";
            using (var command = new SqlCommand(query, dbConnection))
            {
                command.Parameters.AddWithValue("@UserCNP", userCNP);
                command.Parameters.AddWithValue("@Name", stock.Name);
                command.ExecuteNonQuery();
            }
        }

        public void RemoveFromFavorites(HomepageStock stock)
        {
            var query = "DELETE FROM FAVORITE_STOCK WHERE USER_CNP = @UserCNP AND STOCK_NAME = @Name";
            using (var command = new SqlCommand(query, dbConnection))
            {
                command.Parameters.AddWithValue("@UserCNP", userCNP);
                command.Parameters.AddWithValue("@Name", stock.Name);
                command.ExecuteNonQuery();
            }
        }

        public void CreateUserProfile()
        {
            
            string currentCNP = userCNP;

            string userQuery = "INSERT INTO [USER] (CNP, NAME, DESCRIPTION, IS_HIDDEN, IS_ADMIN, PROFILE_PICTURE, GEM_BALANCE) VALUES(@CNP, @Name, @Description, @IsHidden, @IsAdmin, @ProfilePicture, @GemBalance)";

            using (var command = new SqlCommand(userQuery, dbConnection))
            {
                command.Parameters.AddWithValue("@CNP", currentCNP);
                command.Parameters.AddWithValue("@Name", "New User");  
                command.Parameters.AddWithValue("@Description", "Default User Description");
                command.Parameters.AddWithValue("@IsHidden", false);  
                command.Parameters.AddWithValue("@IsAdmin", false);   
                command.Parameters.AddWithValue("@ProfilePicture", "default.jpg");
                command.Parameters.AddWithValue("@GemBalance", 0);  
                command.ExecuteNonQuery();
            }

            string query = "INSERT INTO HARDCODED_CNPS (CNP) VALUES (@CNP)";

            using (var command = new SqlCommand(query, dbConnection))
            {
                command.Parameters.AddWithValue("@CNP", currentCNP);
                command.ExecuteNonQuery();
            }
            
        }

    }
}
