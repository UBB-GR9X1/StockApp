namespace StockApp.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Data.SqlClient;
    using StockApp.Database;
    using StockApp.Models;

    internal class HomepageStocksRepository
    {
        private readonly SqlConnection dbConnection = DatabaseHelper.GetConnection();
        private readonly string userCNP;

        public HomepageStocksRepository()
        {
            this.userCNP = this.GetUserCNP();

            Console.WriteLine("User CNP: " + this.userCNP);
            Console.WriteLine("IsGuestUser: " + this.IsGuestUser(this.userCNP));

            this.LoadStocks();
        }

        public string GetCNP()
        {
            return this.userCNP;
        }

        public List<int> GetStockHistory(string stockName)
        {
            const string query = "SELECT PRICE FROM STOCK_VALUE WHERE STOCK_NAME = @name ORDER BY STOCK_VALUE_ID";

            using SqlCommand getStock = new (query, this.dbConnection);
            getStock.Parameters.AddWithValue("@name", stockName);

            using SqlDataReader reader = getStock.ExecuteReader();
            List<int> stock_values = [];

            while (reader.Read())
            {
                int price = Convert.ToInt32(reader["PRICE"]);
                stock_values.Add(price);
            }

            return stock_values;
        }

        public bool IsGuestUser(string userCNP)
        {
            string query = "SELECT COUNT(*) FROM [USER] WHERE CNP = @UserCNP";

            using SqlCommand command = new (query, this.dbConnection);
            command.Parameters.AddWithValue("@UserCNP", userCNP);

            int count = (int)command.ExecuteScalar();
            Console.WriteLine("Count: " + count);

            return count == 0;
        }

        public string GetUserCnp()
        {
            string query = "SELECT TOP 1 CNP FROM HARDCODED_CNPS ORDER BY CNP DESC";

            using SqlCommand command = new (query, this.dbConnection);
            command.ExecuteNonQuery();

            using var reader = command.ExecuteReader();

            while (!reader.Read())
            {
                throw new Exception("No CNP found in HARDCODED_CNPS table.");
            }

            return reader["CNP"].ToString()
                ?? throw new Exception("CNP is null in HARDCODED_CNPS table.");
        }

        public List<HomepageStock> LoadStocks()
        {
            List<HomepageStock> stocks = [];
            Dictionary<string, List<int>> allStockHistories = [];

            // First, get all stock histories in one go
            string historyQuery = "SELECT STOCK_NAME, PRICE FROM STOCK_VALUE ORDER BY STOCK_NAME, STOCK_VALUE_ID";

            using (SqlCommand historyCommand = new (historyQuery, this.dbConnection))
            {
                using var historyReader = historyCommand.ExecuteReader();
                string? currentStock = null;
                List<int>? currentPrices = null;

                while (historyReader.Read())
                {
                    string stockName = historyReader["STOCK_NAME"].ToString();
                    int price = Convert.ToInt32(historyReader["PRICE"]);

                    if (currentStock != stockName)
                    {
                        if (currentStock != null)
                        {
                            allStockHistories[currentStock] = currentPrices;
                        }

                        currentStock = stockName;
                        currentPrices = [];
                    }

                    currentPrices.Add(price);
                }

                // Add the last stock
                if (currentStock != null)
                {
                    allStockHistories[currentStock] = currentPrices;
                }
            }

            // Now get all stocks info
            string stocksQuery = @"
                SELECT 
                    s.STOCK_NAME,
                    s.STOCK_SYMBOL,
                    MAX(COALESCE(f.IS_FAVORITE, 0)) AS IS_FAVORITE
                FROM STOCK s
                LEFT JOIN FAVORITE_STOCK f ON s.STOCK_NAME = f.STOCK_NAME AND f.USER_CNP = @UserCNP
                GROUP BY s.STOCK_NAME, s.STOCK_SYMBOL";

            using (var command = new SqlCommand(stocksQuery, this.dbConnection))
            {
                command.Parameters.AddWithValue("@UserCNP", this.userCNP);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var stockName = reader["STOCK_NAME"]?.ToString();
                    var stockSymbol = reader["STOCK_SYMBOL"]?.ToString();
                    var isFavorite = Convert.ToInt32(reader["IS_FAVORITE"]) == 1;

                    // Get stock history from dict
                    List<int> stockHistory = allStockHistories.TryGetValue(stockName, out List<int>? value) ? value : [];

                    // Calculate price and change percentage
                    int currentPrice = 0;
                    string changePercentage = "0%";

                    if (stockHistory.Count > 0)
                    {
                        currentPrice = stockHistory.Last();

                        if (stockHistory.Count > 1)
                        {
                            int previousPrice = stockHistory[stockHistory.Count - 2];
                            if (previousPrice > 0)
                            {
                                int increasePerc = ((currentPrice - previousPrice) * 100) / previousPrice;
                                changePercentage = (increasePerc >= 0 ? "+" : string.Empty) + increasePerc.ToString() + "%";
                            }
                        }
                    }

                    HomepageStock stock = new ()
                    {
                        Symbol = stockSymbol,
                        Name = stockName,
                        Price = currentPrice,
                        Change = changePercentage,
                        IsFavorite = isFavorite,
                    };

                    stocks.Add(stock);
                }
            }

            return stocks;
        }

        public void AddToFavorites(HomepageStock stock)
        {
            var query = "INSERT INTO FAVORITE_STOCK (USER_CNP, STOCK_NAME, IS_FAVORITE) VALUES (@UserCNP, @Name, 1)";

            using SqlCommand command = new (query, this.dbConnection);
            command.Parameters.AddWithValue("@UserCNP", this.userCNP);
            command.Parameters.AddWithValue("@Name", stock.Name);

            command.ExecuteNonQuery();
        }

        public void RemoveFromFavorites(HomepageStock stock)
        {
            var query = "DELETE FROM FAVORITE_STOCK WHERE USER_CNP = @UserCNP AND STOCK_NAME = @Name";

            using SqlCommand command = new (query, this.dbConnection);
            command.Parameters.AddWithValue("@UserCNP", this.userCNP);
            command.Parameters.AddWithValue("@Name", stock.Name);

            command.ExecuteNonQuery();
        }

        public void CreateUserProfile()
        {
            List<string> names =
            [
                "storm", "shadow", "blaze", "nova", "ember", "frost", "zephyr", "luna", "onyx", "raven",
                "viper", "echo", "skye", "falcon", "titan", "phoenix", "cobra", "ghost", "venom", "dusk",
                "wraith", "flare", "night", "rogue", "drift", "glitch", "shade", "pulse", "crimson",
                "hazard", "orbit", "quake", "rune", "saber", "thorn", "vortex", "zodiac", "howl", "jett"
            ];

            Random random = new ();
            string name = names[random.Next(names.Count)];
            int number = random.Next(1000, 10000);
            string randomUsername = $"{name}_{number}";

            string userQuery =
                "INSERT INTO [USER] " +
                "(CNP, NAME, DESCRIPTION, IS_HIDDEN, IS_ADMIN, PROFILE_PICTURE, GEM_BALANCE) " +
                "VALUES " +
                "(@CNP, @Name, @Description, @IsHidden, @IsAdmin, @ProfilePicture, @GemBalance)";

            using SqlCommand command = new (userQuery, this.dbConnection);
            command.Parameters.AddWithValue("@CNP", this.userCNP);
            command.Parameters.AddWithValue("@Name", randomUsername);
            command.Parameters.AddWithValue("@Description", "Default User Description");
            command.Parameters.AddWithValue("@IsHidden", false);
            command.Parameters.AddWithValue("@IsAdmin", false);
            command.Parameters.AddWithValue("@ProfilePicture", "https://cdn.discordapp.com/attachments/1309495559085756436/1358378808440389854/defaultProfilePicture.png?ex=67f3a059&is=67f24ed9&hm=674641524bcc24a5fadfde6b087bf550b147c9ec9d81f81e4b0447f69624cb55&");
            command.Parameters.AddWithValue("@GemBalance", 0);

            command.ExecuteNonQuery();
        }
    }
}
