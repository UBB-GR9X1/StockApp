namespace StockApp.Repository
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.SqlClient;
    using StockApp.Database;
    using StockApp.Models;

    public class ProfileRepository : IProfileRepository
    {
        private readonly SqlConnection dbConnection = DatabaseHelper.GetConnection();
        private readonly string cnp;
        private readonly string loggedInUserCNP;

        public ProfileRepository(string authorCNP)
        {
            // Get logged-in user CNP
            this.loggedInUserCNP =
                this.ExecuteScalar<string>(
                    "SELECT TOP 1 CNP FROM HARDCODED_CNPS ORDER BY CNP DESC",
                    null)
                ?? throw new Exception("No CNP found in HARDCODED_CNPS table.");

            // Assign the working user CNP
            this.cnp = authorCNP;
        }

        public IUser CurrentUser()
        {
            const string query = @"
                SELECT CNP, NAME, PROFILE_PICTURE, DESCRIPTION, IS_HIDDEN
                FROM [USER]
                WHERE CNP = @CNP";

            using SqlCommand command = new(query, this.dbConnection);
            command.Parameters.AddWithValue("@CNP", this.cnp);

            using SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User(
                    cnp: reader["CNP"]?.ToString(),
                    username: reader["NAME"]?.ToString(),
                    description: reader["DESCRIPTION"]?.ToString(),
                    isModerator: false, // Assuming this is not part of the query
                    image: reader["PROFILE_PICTURE"]?.ToString(),
                    isHidden: reader["IS_HIDDEN"] != DBNull.Value && (bool)reader["IS_HIDDEN"]
                );
            }

            throw new Exception("User not found.");
        }

        // Helper: Execute a SQL query and return a scalar value
        private T? ExecuteScalar<T>(string query, Action<SqlCommand> parameterize)
        {
            using SqlCommand command = new(query, this.dbConnection);
            parameterize?.Invoke(command);

            var result = command.ExecuteScalar();
            return result == null || result == DBNull.Value
                ? default
                : (T)Convert.ChangeType(result, typeof(T));
        }

        // Helper: Execute a non-query SQL command
        private void ExecuteNonQuery(string query, Action<SqlCommand> parameterize)
        {
            using SqlCommand command = new(query, this.dbConnection);
            parameterize?.Invoke(command);
            command.ExecuteNonQuery();
        }

        // Generate random usernames
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

        // Get current user profile
        public IUser GetUserProfile(string authorCNP)
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

        // Update admin status
        public void UpdateRepoIsAdmin(bool isAdmin)
        {
            this.ExecuteNonQuery("UPDATE [USER] SET IS_ADMIN = @IsAdmin WHERE CNP = @CNP", command =>
            {
                command.Parameters.AddWithValue("@IsAdmin", isAdmin ? 1 : 0);
                command.Parameters.AddWithValue("@CNP", this.cnp);
            });
        }

        // Update user profile
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
                    command.Parameters.AddWithValue("@CNP", this.cnp);
                }
            );
        }

        // Get user-owned stocks
        public IReadOnlyList<string> UserStocks()
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

            var stocks = new List<string>();
            using var command = new SqlCommand(query, this.dbConnection);
            command.Parameters.AddWithValue("@UserCNP", this.cnp);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var stockString = $"{reader["STOCK_SYMBOL"]} | {reader["STOCK_NAME"]} | " +
                                  $"Quantity: {reader["QUANTITY"]} | Price: {reader["PRICE"]}";
                stocks.Add(stockString);
            }

            return stocks;
        }

        // Get logged-in user CNP
        public string GetLoggedInUserCNP() => this.loggedInUserCNP;
    }
}