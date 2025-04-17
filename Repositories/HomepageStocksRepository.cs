namespace StockApp.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Data.SqlClient;
    using Microsoft.IdentityModel.Tokens;
    using StockApp.Database;
    using StockApp.Models;

    internal class HomepageStocksRepository : IHomepageStocksRepository
    {
        private string _UserCNP;

        private string UserCNP
        {
            get => _UserCNP;
            set
            {
                _UserCNP = value;
                if (string.IsNullOrEmpty(_UserCNP))
                {
                    throw new ArgumentNullException(nameof(UserCNP), "CNP cannot be null or empty.");
                }
                this.LoadStocks();
            }
        }

        public HomepageStocksRepository()
        {
            // TODO: don't just load a mock user
            string query = "SELECT TOP 1 CNP FROM HARDCODED_CNPS ORDER BY CNP DESC";

            using (var command = new SqlCommand(query, DatabaseHelper.GetConnection()))
            {
                command.ExecuteNonQuery();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        this.UserCNP = reader["CNP"].ToString() ?? throw new Exception("No CNP found in HARDCODED_CNPS table.");
                        return;
                    }
                }
            }

            throw new InvalidOperationException("No hardcoded CNP found in the database");
        }

        public IReadOnlyList<int> GetStockHistory(string stockName)
        {
            const string query = "SELECT PRICE FROM STOCK_VALUE WHERE STOCK_NAME = @name ORDER BY STOCK_VALUE_ID";
            return this.ExecuteReader(query, command => command.Parameters.AddWithValue("@name", stockName),
                reader => Convert.ToInt32(reader["PRICE"]));
        }

        public bool IsGuestUser(string userCNP)
        {
            if (userCNP.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(userCNP), "CNP cannot be null or empty.");
            const string query = "SELECT COUNT(*) FROM [USER] WHERE CNP = @UserCNP";
            return this.ExecuteScalar<int>(query, command => command.Parameters.AddWithValue("@UserCNP", userCNP)) == 0;
        }

        public string GetUserCnp()
        {
            const string query = "SELECT TOP 1 CNP FROM HARDCODED_CNPS ORDER BY CNP DESC";
            var cnps = this.ExecuteReader(query, null, reader => reader["CNP"]?.ToString());
            return cnps.FirstOrDefault() ?? throw new Exception("No CNP found in HARDCODED_CNPS table.");
        }

        public IReadOnlyList<IHomepageStock> LoadStocks()
        {
            // Fetch stock histories
            const string historyQuery = "SELECT STOCK_NAME, PRICE FROM STOCK_VALUE ORDER BY STOCK_NAME, STOCK_VALUE_ID";
            var allStockHistories = new Dictionary<string, List<int>>();

            var historyResults = this.ExecuteReader(historyQuery, null, reader => new
            {
                StockName = reader["STOCK_NAME"].ToString(),
                Price = Convert.ToInt32(reader["PRICE"])
            });

            foreach (var group in historyResults.GroupBy(h => h.StockName))
            {
                allStockHistories[group.Key] = group.Select(g => g.Price).ToList();
            }

            // Fetch stock info
            const string stocksQuery = @"
                SELECT 
                    s.STOCK_NAME,
                    s.STOCK_SYMBOL,
                    MAX(COALESCE(f.IS_FAVORITE, 0)) AS IS_FAVORITE
                FROM STOCK s
                LEFT JOIN FAVORITE_STOCK f ON s.STOCK_NAME = f.STOCK_NAME AND f.USER_CNP = @UserCNP
                GROUP BY s.STOCK_NAME, s.STOCK_SYMBOL";

            return this.ExecuteReader(
                stocksQuery,
                command => command.Parameters.AddWithValue("@UserCNP", this.UserCNP),
                reader =>
                {
                    var stockName = reader["STOCK_NAME"]?.ToString();
                    var stockSymbol = reader["STOCK_SYMBOL"]?.ToString();
                    var isFavorite = Convert.ToInt32(reader["IS_FAVORITE"]) == 1;

                    var stockHistory = allStockHistories.TryGetValue(stockName, out var history) ? history : [];
                    var currentPrice = stockHistory.LastOrDefault();
                    var previousPrice = stockHistory.Count > 1 ? stockHistory[stockHistory.Count - 2] : 0;
                    var changePercentage = previousPrice > 0
                        ? $"{((currentPrice - previousPrice) * 100) / previousPrice:+0;-0}%"
                        : "0%";

                    return new HomepageStock
                    {
                        Symbol = stockSymbol,
                        Name = stockName,
                        Price = currentPrice,
                        Change = changePercentage,
                        IsFavorite = isFavorite
                    };
                });
        }

        public void AddToFavorites(IHomepageStock stock)
        {
            const string query = "INSERT INTO FAVORITE_STOCK (USER_CNP, STOCK_NAME, IS_FAVORITE) VALUES (@UserCNP, @Name, 1)";
            this.ExecuteSql(query, command =>
            {
                command.Parameters.AddWithValue("@UserCNP", this.UserCNP);
                command.Parameters.AddWithValue("@Name", stock.Name);
            });
        }

        public void RemoveFromFavorites(IHomepageStock stock)
        {
            const string query = "DELETE FROM FAVORITE_STOCK WHERE USER_CNP = @UserCNP AND STOCK_NAME = @Name";
            this.ExecuteSql(query, command =>
            {
                command.Parameters.AddWithValue("@UserCNP", this.UserCNP);
                command.Parameters.AddWithValue("@Name", stock.Name);
            });
        }

        public void CreateUserProfile()
        {
            var names = new List<string>
            {
                "storm", "shadow", "blaze", "nova", "ember", "frost", "zephyr", "luna", "onyx", "raven",
                "viper", "echo", "skye", "falcon", "titan", "phoenix", "cobra", "ghost", "venom", "dusk",
                "wraith", "flare", "night", "rogue", "drift", "glitch", "shade", "pulse", "crimson",
                "hazard", "orbit", "quake", "rune", "saber", "thorn", "vortex", "zodiac", "howl", "jett"
            };

            var random = new Random();
            var randomUsername = $"{names[random.Next(names.Count)]}_{random.Next(1000, 10000)}";

            const string query = @"
                INSERT INTO [USER] 
                (CNP, NAME, DESCRIPTION, IS_HIDDEN, IS_ADMIN, PROFILE_PICTURE, GEM_BALANCE)
                VALUES 
                (@CNP, @Name, @Description, @IsHidden, @IsAdmin, @ProfilePicture, @GemBalance)";

            this.ExecuteSql(query, command =>
            {
                command.Parameters.AddWithValue("@CNP", this.UserCNP);
                command.Parameters.AddWithValue("@Name", randomUsername);
                command.Parameters.AddWithValue("@Description", "Default User Description");
                command.Parameters.AddWithValue("@IsHidden", false);
                command.Parameters.AddWithValue("@IsAdmin", false);
                command.Parameters.AddWithValue("@ProfilePicture", "https://cdn.discordapp.com/attachments/1309495559085756436/1358378808440389854/defaultProfilePicture.png");
                command.Parameters.AddWithValue("@GemBalance", 0);
            });
        }

        // Helper: Execute a SQL query with parameters
        private void ExecuteSql(string query, Action<SqlCommand> parameterize)
        {
            using var command = new SqlCommand(query, DatabaseHelper.GetConnection());
            parameterize?.Invoke(command);
            command.ExecuteNonQuery();
        }

        // Helper: Execute a query and return a scalar value
        private T ExecuteScalar<T>(string query, Action<SqlCommand> parameterize)
        {
            using var command = new SqlCommand(query, DatabaseHelper.GetConnection());
            parameterize?.Invoke(command);
            return (T)Convert.ChangeType(command.ExecuteScalar(), typeof(T));
        }

        // Helper: Execute and map SQL data reader results
        private List<T> ExecuteReader<T>(string query, Action<SqlCommand> parameterize, Func<SqlDataReader, T> map)
        {
            using SqlCommand command = new(query, DatabaseHelper.GetConnection());
            parameterize?.Invoke(command);

            using var reader = command.ExecuteReader();
            List<T> results = [];

            while (reader.Read())
            {
                results.Add(map(reader));
            }

            return results;
        }
    }
}