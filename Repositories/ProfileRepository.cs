namespace StockApp.Repositories
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.SqlClient;
    using StockApp.Database;
    using StockApp.Models;

    /// <summary>
    /// Repository for managing user profiles and related operations.
    /// </summary>
    public class ProfileRepository
    {
        private readonly SqlConnection dbConnection = DatabaseHelper.GetConnection();
        private readonly string loggedInUserCNP;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileRepository"/> class.
        /// </summary>
        /// <param name="authorCNP">The CNP of the user whose profile is being managed.</param>
        public ProfileRepository(string authorCNP)
        {
            // Assign the working user CNP
            this.loggedInUserCNP = authorCNP;
        }

        /// <summary>
        /// Gets the current user's profile.
        /// </summary>
        /// <returns>A <see cref="User"/> object representing the current user.</returns>
        public User CurrentUser()
        {
            const string query = @"
                    SELECT CNP, NAME, PROFILE_PICTURE, DESCRIPTION, IS_HIDDEN, GEM_BALANCE
                    FROM [USER]
                    WHERE CNP = @CNP";

            using SqlCommand command = new(query, this.dbConnection);
            command.Parameters.AddWithValue("@CNP", this.loggedInUserCNP);

            using SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User(
                    cnp: reader["CNP"]?.ToString(),
                    username: reader["NAME"]?.ToString(),
                    description: reader["DESCRIPTION"]?.ToString(),
                    isModerator: false, // Assuming this is not part of the query
                    image: reader["PROFILE_PICTURE"]?.ToString(),
                    isHidden: reader["IS_HIDDEN"] != DBNull.Value && (bool)reader["IS_HIDDEN"],
                    gem_balance: reader["GEM_BALANCE"] != DBNull.Value ? (int)reader["GEM_BALANCE"] : 0
                );
            }

            throw new Exception("User not found.");
        }


        /// <summary>
        /// Generates a random username from a predefined list.
        /// </summary>
        /// <returns>A randomly selected username.</returns>
        public string GenerateUsername()
        {
            List<string> randomUsernames =
            [
                "macaroane_cu_branza", "ecler_cu_fistic", "franzela_", "username1",
                    "snitel_cu_piure", "ceai_de_musetel", "vita_de_vie", "paine_cu_pateu",
                    "floare_de_tei", "cirese_si_visine", "inghetata_roz", "tort_de_afine",
                    "paste_carbonara", "amandina", "orez_cu_lapte"
            ];

            Random random = new();
            return randomUsernames[random.Next(randomUsernames.Count)];
        }

        /// <summary>
        /// Gets the profile of a user by their CNP.
        /// </summary>
        /// <param name="authorCNP">The CNP of the user whose profile is being retrieved.</param>
        /// <returns>A <see cref="User"/> object representing the user.</returns>
        public User GetUserProfile(string authorCNP)
        {
            const string query = @"
                    SELECT CNP, NAME, PROFILE_PICTURE, DESCRIPTION, IS_HIDDEN
                    FROM [USER]
                    WHERE CNP = @CNP";
            return this.ExecuteScalar<User>(query, command =>
            {
                command.Parameters.AddWithValue("@CNP", authorCNP);
            }) ?? throw new Exception("User not found.");
        }

        /// <summary>
        /// Updates the admin status of the current user.
        /// </summary>
        /// <param name="isAdmin">A value indicating whether the user should be an admin.</param>
        public void UpdateRepoIsAdmin(bool isAdmin)
        {
            this.ExecuteNonQuery("UPDATE [USER] SET IS_ADMIN = @IsAdmin WHERE CNP = @CNP", command =>
            {
                command.Parameters.AddWithValue("@IsAdmin", isAdmin ? 1 : 0);
                command.Parameters.AddWithValue("@CNP", this.loggedInUserCNP);
            });
        }

        /// <summary>
        /// Updates the profile of the current user.
        /// </summary>
        /// <param name="newUsername">The new username.</param>
        /// <param name="newImage">The new profile picture URL.</param>
        /// <param name="newDescription">The new description.</param>
        /// <param name="newHidden">A value indicating whether the profile should be hidden.</param>
        public void UpdateMyUser(string newUsername, string newImage, string newDescription, bool newHidden)
        {
            this.ExecuteNonQuery(
                @"UPDATE [USER]
                      SET NAME = @NewName, PROFILE_PICTURE = @NewProfilePicture, 
                          DESCRIPTION = @NewDescription, IS_HIDDEN = @NewIsHidden 
                      WHERE CNP = @CNP",
                command =>
                {
                    command.Parameters.AddWithValue("@NewName", newUsername);
                    command.Parameters.AddWithValue("@NewProfilePicture", newImage);
                    command.Parameters.AddWithValue("@NewDescription", newDescription);
                    command.Parameters.AddWithValue("@NewIsHidden", newHidden ? 1 : 0);
                    command.Parameters.AddWithValue("@CNP", this.loggedInUserCNP);
                }
            );
        }

        /// <summary>
        /// Gets the list of stocks owned by the current user.
        /// </summary>
        /// <returns>A list of the user's stocks.</returns>
        public List<Stock> UserStocks()
        {
            const string query = @"
                    WITH UserStocks AS (
                        SELECT STOCK_NAME
                        FROM USER_STOCK
                        WHERE USER_CNP = @UserCNP
                    ),
                    LatestStockValue AS (
                        SELECT sv1.STOCK_NAME, sv1.PRICE
                        FROM STOCK_VALUE sv1
                        WHERE sv1.STOCK_NAME IN (SELECT STOCK_NAME FROM UserStocks)
                          AND sv1.PRICE = (
                              SELECT MAX(sv2.PRICE)
                              FROM STOCK_VALUE sv2
                              WHERE sv2.STOCK_NAME = sv1.STOCK_NAME
                          )
                    )
                    SELECT 
                        s.STOCK_SYMBOL,
                        us.STOCK_NAME,
                        us.QUANTITY,
                        COALESCE(lsv.PRICE, 0) AS PRICE
                    FROM USER_STOCK us
                    JOIN STOCK s ON us.STOCK_NAME = s.STOCK_NAME
                    LEFT JOIN LatestStockValue lsv ON s.STOCK_NAME = lsv.STOCK_NAME
                    WHERE us.USER_CNP = @UserCNP";

            using var command = new SqlCommand(query, DatabaseHelper.GetConnection());
            command.Parameters.AddWithValue("@UserCNP", this.loggedInUserCNP);
            using var reader = command.ExecuteReader();
            List<Stock> stocks = new();
            while (reader.Read())
            {
                stocks.Add(new Stock(
                    symbol: reader["STOCK_SYMBOL"]?.ToString() ?? throw new Exception("Stock symbol not found."),
                    name: reader["STOCK_NAME"]?.ToString() ?? throw new Exception("Stock name not found."),
                    quantity: reader["QUANTITY"] != DBNull.Value ? (int)reader["QUANTITY"] : throw new Exception("Stock quantity not found."),
                    price: reader["PRICE"] != DBNull.Value ? (int)reader["PRICE"] : throw new Exception("Stock price not found."),
                    authorCNP: this.loggedInUserCNP
                ));
            }
            return stocks;
        }

        /// <summary>
        /// Executes a SQL query and returns a scalar value.
        /// </summary>
        /// <typeparam name="T">The type of the scalar value to return.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameterize">An action to parameterize the SQL command.</param>
        /// <returns>The scalar value of type <typeparamref name="T"/>.</returns>
        private T? ExecuteScalar<T>(string query, Action<SqlCommand> parameterize)
        {
            using SqlCommand command = new(query, this.dbConnection);
            parameterize?.Invoke(command);

            var result = command.ExecuteScalar();
            return result == null || result == DBNull.Value
                ? default
                : (T)Convert.ChangeType(result, typeof(T));
        }

        /// <summary>
        /// Executes a non-query SQL command.
        /// </summary>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameterize">An action to parameterize the SQL command.</param>
        private void ExecuteNonQuery(string query, Action<SqlCommand> parameterize)
        {
            using SqlCommand command = new(query, this.dbConnection);
            parameterize?.Invoke(command);
            command.ExecuteNonQuery();
        }
    }
}