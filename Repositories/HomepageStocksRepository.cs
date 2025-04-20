using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("StockApp.Service.Tests")]

namespace StockApp.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Data.SqlClient;
    using Microsoft.IdentityModel.Tokens;
    using StockApp.Database;
    using StockApp.Models;

    /// <summary>
    /// Repository for retrieving homepage stock information and managing user favorites.
    /// </summary>
    internal class HomepageStocksRepository : IHomepageStocksRepository
    {
        private string userCnp;

        /// <summary>
        /// Gets or sets the current user's CNP. Setting this triggers loading of stock data.
        /// </summary>
        private string UserCnp
        {
            get => this.userCnp;
            set
            {
                this.userCnp = value;
                if (string.IsNullOrEmpty(this.userCnp))
                {
                    throw new ArgumentNullException(nameof(this.UserCnp), "CNP cannot be null or empty.");
                }

                // Inline: load stocks whenever UserCNP is assigned
                this.LoadStocks();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomepageStocksRepository"/> class
        /// by retrieving a hardcoded CNP and loading initial stock data.
        /// </summary>
        public HomepageStocksRepository()
        {
            var userRepository = new UserRepository();
            this.userCnp = userRepository.CurrentUserCNP;
        }

        /// <summary>
        /// Retrieves the historical prices for a given stock.
        /// </summary>
        /// <param name="stockName">Name of the stock.</param>
        /// <returns>List of historical price values.</returns>
        public List<int> GetStockHistory(string stockName)
        {
            const string query = "SELECT PRICE FROM STOCK_VALUE WHERE STOCK_NAME = @name ORDER BY STOCK_VALUE_ID";

            // Inline: execute reader mapping PRICE column to int
            return ExecuteReader(query,
                command => command.Parameters.AddWithValue("@name", stockName),
                reader => Convert.ToInt32(reader["PRICE"]));
        }

        /// <summary>
        /// Determines if the specified user is a guest (i.e., not in the USER table).
        /// </summary>
        /// <param name="userCNP">User identifier (CNP).</param>
        /// <returns>True if user is guest; otherwise false.</returns>
        public bool IsGuestUser(string userCNP)
        {
            if (userCNP.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(userCNP), "CNP cannot be null or empty.");
            }

            const string query = "SELECT COUNT(*) FROM [USER] WHERE CNP = @UserCNP";
            int count = ExecuteScalar<int>(query,
                command => command.Parameters.AddWithValue("@UserCNP", userCNP));
            return count == 0;
        }

        /// <summary>
        /// Retrieves the current user's CNP from the hardcoded table.
        /// </summary>
        /// <returns>The latest CNP string.</returns>
        public string GetUserCnp()
        {
            return new UserRepository().CurrentUserCNP;
        }

        /// <summary>
        /// Loads stock summaries including current price, change percentage, and favorite status.
        /// </summary>
        /// <returns>List of <see cref="HomepageStock"/> objects for display.</returns>
        public List<HomepageStock> LoadStocks()
        {
            // Inline: first gather all price histories
            const string historyQuery = "SELECT STOCK_NAME, PRICE FROM STOCK_VALUE ORDER BY STOCK_NAME, STOCK_VALUE_ID";
            var allStockHistories = new Dictionary<string, List<int>>();

            var historyResults = ExecuteReader(historyQuery, null, reader => new
            {
                StockName = reader["STOCK_NAME"].ToString(),
                Price = Convert.ToInt32(reader["PRICE"]),
            });

            foreach (var group in historyResults.GroupBy(h => h.StockName))
            {
                allStockHistories[group.Key] = [.. group.Select(g => g.Price)];
            }

            // Inline: query stock info and favorites
            const string stocksQuery = @"
                SELECT 
                    s.STOCK_NAME,
                    s.STOCK_SYMBOL,
                    s.AUTHOR_CNP,
                    MAX(COALESCE(f.IS_FAVORITE, 0)) AS IS_FAVORITE
                FROM STOCK s
                LEFT JOIN FAVORITE_STOCK f ON s.STOCK_NAME = f.STOCK_NAME AND f.USER_CNP = @UserCNP
                GROUP BY s.STOCK_NAME, s.STOCK_SYMBOL, s.AUTHOR_CNP";

            return ExecuteReader(
                stocksQuery,
                command => command.Parameters.AddWithValue("@UserCNP", this.UserCnp),
                reader =>
                {
                    var stockName = reader["STOCK_NAME"]?.ToString() ?? throw new InvalidOperationException("Stock name cannot be null.");
                    var stockSymbol = reader["STOCK_SYMBOL"]?.ToString() ?? throw new InvalidOperationException("Stock symbol cannot be null.");
                    var isFavorite = Convert.ToInt32(reader["IS_FAVORITE"]) == 1;

                    var stockHistory = allStockHistories.TryGetValue(stockName, out var history) ? history : [];
                    var currentPrice = stockHistory.LastOrDefault();
                    var previousPrice = stockHistory.Count > 1 ? stockHistory[^2] : 0;

                    // Inline: calculate percent change or default to 0%
                    var changePercentage = previousPrice > 0
                        ? $"{((currentPrice - previousPrice) * 100) / previousPrice:+0;-0}%"
                        : "0%";

                    Stock stock = new(
                                    symbol: stockSymbol,
                                    name: stockName,
                                    authorCNP: reader["AUTHOR_CNP"]?.ToString() ?? throw new InvalidOperationException("Author CNP cannot be null."),
                                    price: currentPrice,
                                    quantity: 0);

                    return new HomepageStock
                    {
                        StockDetails = stock,
                        Change = changePercentage,
                        IsFavorite = isFavorite,
                    };
                });
        }

        /// <summary>
        /// Adds the specified stock to the user's favorites.
        /// </summary>
        /// <param name="stock">The <see cref="HomepageStock"/> to favorite.</param>
        public void AddToFavorites(HomepageStock stock)
        {
            const string query = "INSERT INTO FAVORITE_STOCK (USER_CNP, STOCK_NAME, IS_FAVORITE) VALUES (@UserCNP, @Name, 1)";
            ExecuteSql(query, command =>
            {
                command.Parameters.AddWithValue("@UserCNP", this.UserCnp);
                command.Parameters.AddWithValue("@Name", stock.StockDetails.Name);
            });
        }

        /// <summary>
        /// Removes the specified stock from the user's favorites.
        /// </summary>
        /// <param name="stock">The <see cref="HomepageStock"/> to unfavorite.</param>
        public void RemoveFromFavorites(HomepageStock stock)
        {
            const string query = "DELETE FROM FAVORITE_STOCK WHERE USER_CNP = @UserCNP AND STOCK_NAME = @Name";
            ExecuteSql(query, command =>
            {
                command.Parameters.AddWithValue("@UserCNP", this.UserCnp);
                command.Parameters.AddWithValue("@Name", stock.StockDetails.Name);
            });
        }

        /// <summary>
        /// Creates a new user profile with random username and default values.
        /// </summary>
        public void CreateUserProfile()
        {
            // Inline: generate a random username
            var names = new List<string>
            {
                "storm", "shadow", "blaze", "nova", "ember", "frost", "zephyr", "luna", "onyx", "raven",
                "viper", "echo", "skye", "falcon", "titan", "phoenix", "cobra", "ghost", "venom", "dusk",
                "wraith", "flare", "night", "rogue", "drift", "glitch", "shade", "pulse", "crimson",
                "hazard", "orbit", "quake", "rune", "saber", "thorn", "vortex", "zodiac", "howl", "jett",
            };

            var random = new Random();
            var randomUsername = $"{names[random.Next(names.Count)]}_{random.Next(1000, 10000)}";

            const string query = @"
                INSERT INTO [USER] 
                (CNP, NAME, DESCRIPTION, IS_HIDDEN, IS_ADMIN, PROFILE_PICTURE, GEM_BALANCE)
                VALUES 
                (@CNP, @Name, @Description, @IsHidden, @IsAdmin, @ProfilePicture, @GemBalance)";

            ExecuteSql(query, command =>
            {
                command.Parameters.AddWithValue("@CNP", this.UserCnp);
                command.Parameters.AddWithValue("@Name", randomUsername);
                command.Parameters.AddWithValue("@Description", "Default User Description");
                command.Parameters.AddWithValue("@IsHidden", false);
                command.Parameters.AddWithValue("@IsAdmin", false);
                command.Parameters.AddWithValue("@ProfilePicture", "https://cdn.discordapp.com/attachments/1309495559085756436/1358378808440389854/defaultProfilePicture.png");
                command.Parameters.AddWithValue("@GemBalance", 0);
            });
        }

        // Shared helper: execute a non-query SQL command
        private static void ExecuteSql(string query, Action<SqlCommand> parameterize)
        {
            using var command = new SqlCommand(query, DatabaseHelper.GetConnection());
            parameterize?.Invoke(command);
            command.ExecuteNonQuery();
        }

        // Shared helper: execute a scalar query and return typed result
        private static T ExecuteScalar<T>(string query, Action<SqlCommand> parameterize)
        {
            using var command = new SqlCommand(query, DatabaseHelper.GetConnection());
            parameterize?.Invoke(command);
            return (T)Convert.ChangeType(command.ExecuteScalar(), typeof(T));
        }

        // Shared helper: execute reader and map each row
        private static List<T> ExecuteReader<T>(string query, Action<SqlCommand> parameterize, Func<SqlDataReader, T> map)
        {
            using SqlCommand command = new(query, DatabaseHelper.GetConnection());
            parameterize?.Invoke(command);

            using var reader = command.ExecuteReader();
            var results = new List<T>();

            // Inline: read each row and apply mapping function
            while (reader.Read())
            {
                results.Add(map(reader));
            }

            return results;
        }
    }
}
