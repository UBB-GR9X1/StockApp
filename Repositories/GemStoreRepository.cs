namespace StockApp.Repositories
{
    using System;
    using Microsoft.Data.SqlClient;
    using StockApp.Database;

    internal class GemStoreRepository : IGemStoreRepository
    {
        private readonly IDbExecutor dbConnection;
        private readonly string cnp;

        public GemStoreRepository()
            : this(new SqlDbExecutor(DatabaseHelper.GetConnection()))
        {
            var userRepo = new UserRepository();
            this.cnp = userRepo.CurrentUserCNP;
        }

        internal GemStoreRepository(IDbExecutor executor)
        {
            dbConnection = executor ?? throw new ArgumentNullException(nameof(executor));

            var userRepo = new UserRepository();
            this.cnp = userRepo.CurrentUserCNP;
        }

        public string GetCnp()
        {
            return new UserRepository().CurrentUserCNP;
        }

        public int GetUserGemBalance(string userCnp)
        {
            string checkQuery = "SELECT GEM_BALANCE FROM [USER] WHERE CNP = @CNP";

            var result = this.dbConnection.ExecuteScalar(checkQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@CNP", userCnp);
            });

            return result != null ? Convert.ToInt32(result) : 0;
        }

        public void UpdateUserGemBalance(string userCnp, int newBalance)
        {
            string updateQuery = "UPDATE [USER] SET GEM_BALANCE = @NewBalance WHERE CNP = @CNP";

            this.dbConnection.ExecuteNonQuery(updateQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@NewBalance", newBalance);
                cmd.Parameters.AddWithValue("@CNP", userCnp);
            });
        }

        public bool IsGuest(string userCnp)
        {
            string checkQuery = "SELECT COUNT(*) FROM [USER] WHERE CNP = @CNP";

            var result = this.dbConnection.ExecuteScalar(checkQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@CNP", userCnp);
            });
            var count = result != null ? Convert.ToInt32(result) : 0;
            return count == 0;
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
}
