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
        private readonly string cnp;

        /// <summary>
        /// Initializes a new instance of the <see cref="GemStoreRepository"/> class with the default SQL executor.
        /// </summary>
        public GemStoreRepository()
            : this(new SqlDbExecutor(DatabaseHelper.GetConnection()))
        {
            var userRepo = new UserRepository();
            this.cnp = userRepo.CurrentUserCnp;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GemStoreRepository"/> class with the specified database executor.
        /// </summary>
        /// <param name="executor">Executor used to run SQL commands.</param>
        internal GemStoreRepository(IDbExecutor executor)
        {
            this.dbConnection = executor ?? throw new ArgumentNullException(nameof(executor));

            var userRepo = new UserRepository();
            this.cnp = userRepo.CurrentUserCnp;
        }

        /// <summary>
        /// Retrieves the CNP for the current user.
        /// </summary>
        /// <returns>The CNP string, or empty if not found.</returns>
        public string GetCnp()
        {
            return new UserRepository().CurrentUserCnp;
        }

        /// <summary>
        /// Retrieves the current gem balance for the specified user.
        /// </summary>
        /// <param name="userCnp">User identifier (CNP).</param>
        /// <returns>User's gem balance as an integer.</returns>
        public int GetUserGemBalance(string userCnp)
        {
            string checkQuery = "SELECT GEM_BALANCE FROM [USER] WHERE CNP = @CNP";

            var result = this.dbConnection.ExecuteScalar(checkQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@CNP", userCnp);
            });

            // FIXME: consider throwing an exception if the user is not found
            return result != null ? Convert.ToInt32(result) : 0;
        }

        /// <summary>
        /// Updates the gem balance for a given user.
        /// </summary>
        /// <param name="userCnp">User identifier (CNP).</param>
        /// <param name="newBalance">New gem balance to set.</param>
        public void UpdateUserGemBalance(string userCnp, int newBalance)
        {
            string updateQuery = "UPDATE [USER] SET GEM_BALANCE = @NewBalance WHERE CNP = @CNP";

            this.dbConnection.ExecuteNonQuery(updateQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@NewBalance", newBalance);
                cmd.Parameters.AddWithValue("@CNP", userCnp);
            });
        }

        /// <summary>
        /// Determines if the specified user is considered a guest (no record in the database).
        /// </summary>
        /// <param name="userCnp">User identifier (CNP).</param>
        /// <returns><c>true</c> if no user record exists; otherwise, <c>false</c>.</returns>
        public bool IsGuest(string userCnp)
        {
            string checkQuery = "SELECT COUNT(*) FROM [USER] WHERE CNP = @CNP";

            var result = this.dbConnection.ExecuteScalar(checkQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@CNP", userCnp);
            });

            // TODO: consider caching the result to avoid constant database hits
            var count = result != null ? Convert.ToInt32(result) : 0;
            return count == 0;
        }
    }
}
