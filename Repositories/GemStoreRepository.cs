namespace StockApp.Repositories
{
    using System;
    using Microsoft.Data.SqlClient;
    using StockApp.Database;

    /// <summary>
    /// Repository for retrieving and updating user gem balances and CNP values.
    /// </summary>
    internal class GemStoreRepository : IGemStoreRepository
    {
        private readonly IDbExecutor dbConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="GemStoreRepository"/> class with the default SQL executor.
        /// </summary>
        public GemStoreRepository()
            : this(new SqlDbExecutor(DatabaseHelper.GetConnection()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GemStoreRepository"/> class with the specified database executor.
        /// </summary>
        /// <param name="executor">Executor used to run SQL commands.</param>
        internal GemStoreRepository(IDbExecutor executor)
        {
            dbConnection = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        /// <summary>
        /// Retrieves the CNP for the current user.
        /// </summary>
        /// <returns>The CNP string, or empty if not found.</returns>
        public string GetCnp()
        {
            const string cnpQuery = "SELECT CNP FROM HARDCODED_CNPS";
            // Inline: execute scalar query to fetch CNP
            var result = dbConnection.ExecuteScalar(cnpQuery, cmd => { });
            // TODO: handle multiple results or empty table case more robustly
            return result?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the current gem balance for the specified user.
        /// </summary>
        /// <param name="cnp">User identifier (CNP).</param>
        /// <returns>User's gem balance as an integer.</returns>
        public int GetUserGemBalance(string cnp)
        {
            const string checkQuery = "SELECT GEM_BALANCE FROM [USER] WHERE CNP = @CNP";
            // Inline: pass CNP parameter and execute scalar
            var result = dbConnection.ExecuteScalar(checkQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@CNP", cnp);
            });
            // FIXME: consider throwing an exception if the user is not found
            return result != null ? Convert.ToInt32(result) : 0;
        }

        /// <summary>
        /// Updates the gem balance for a given user.
        /// </summary>
        /// <param name="cnp">User identifier (CNP).</param>
        /// <param name="newBalance">New gem balance to set.</param>
        public void UpdateUserGemBalance(string cnp, int newBalance)
        {
            const string updateQuery = "UPDATE [USER] SET GEM_BALANCE = @NewBalance WHERE CNP = @CNP";
            // Inline: parameterize update and execute non-query
            dbConnection.ExecuteNonQuery(updateQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@NewBalance", newBalance);
                cmd.Parameters.AddWithValue("@CNP", cnp);
            });
        }

        /// <summary>
        /// Determines if the specified user is considered a guest (no record in the database).
        /// </summary>
        /// <param name="cnp">User identifier (CNP).</param>
        /// <returns><c>true</c> if no user record exists; otherwise, <c>false</c>.</returns>
        public bool IsGuest(string cnp)
        {
            const string checkQuery = "SELECT COUNT(*) FROM [USER] WHERE CNP = @CNP";
            // Inline: count matching CNP entries
            var result = dbConnection.ExecuteScalar(checkQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@CNP", cnp);
            });
            // TODO: consider caching the result to avoid constant database hits
            var count = result != null ? Convert.ToInt32(result) : 0;
            return count == 0;
        }
    }
}

    //public void PopulateHardcodedCnps()
    //{
    //    string[] cnps = new string[] { "1234567890123"};
    //    string insertQuery = "INSERT INTO HARDCODED_CNPS (CNP) VALUES (@CNP)";
    //    using (var insertCommand = new SqlCommand(insertQuery, dbConnection))
    //    {
    //        foreach (var cnp in cnps)
    //        {
    //            insertCommand.Parameters.Clear();
    //            insertCommand.Parameters.AddWithValue("@CNP", cnp);
    //            insertCommand.ExecuteNonQuery();
    //        }
    //    }
    //}

    //public void PopulateUserTable()
    //{
    //    var users = new[]
    //    {
    //        new { CNP = "1234567890123", Name = "Emma Popescu", GemBalance = 1000 },
    //        new { CNP = "1234567890124", Name = "Diana Ionescu", GemBalance = 1500 },
    //        new { CNP = "1234567890125", Name = "Oana Georgescu", GemBalance = 200 }
    //    };

    //    string insertQuery = "INSERT INTO [USER] (CNP, NAME, GEM_BALANCE) VALUES (@CNP, @Name, @GemBalance)";
    //    using (var insertCommand = new SqlCommand(insertQuery, dbConnection))
    //    {
    //        foreach (var user in users)
    //        {
    //            insertCommand.Parameters.Clear();
    //            insertCommand.Parameters.AddWithValue("@CNP", user.CNP);
    //            insertCommand.Parameters.AddWithValue("@Name", user.Name);
    //            insertCommand.Parameters.AddWithValue("@GemBalance", user.GemBalance);
    //            insertCommand.ExecuteNonQuery();
    //        }
    //    }
    //}