namespace StockApp.Repositories
{
    using System;
    using Microsoft.Data.SqlClient;
    using StockApp.Database;

    internal class GemStoreRepository : IGemStoreRepository
    {
        private readonly IDbExecutor dbConnection;

        public GemStoreRepository()
           : this(new SqlDbExecutor(DatabaseHelper.GetConnection()))
        {
        }

        internal GemStoreRepository(IDbExecutor executor)
        {
            dbConnection = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public string GetCnp()
        {
            string cnpQuery = "SELECT CNP FROM HARDCODED_CNPS";
            var result = dbConnection.ExecuteScalar(cnpQuery, cmd => { });
            return result?.ToString() ?? string.Empty;
        }

        public int GetUserGemBalance(string cnp)
        {
            string checkQuery = "SELECT GEM_BALANCE FROM [USER] WHERE CNP = @CNP";

            var result = dbConnection.ExecuteScalar(checkQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@CNP", cnp);
            });

            return result != null ? Convert.ToInt32(result) : 0;
        }

        public void UpdateUserGemBalance(string cnp, int newBalance)
        {
            string updateQuery = "UPDATE [USER] SET GEM_BALANCE = @NewBalance WHERE CNP = @CNP";

            dbConnection.ExecuteNonQuery(updateQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@NewBalance", newBalance);
                cmd.Parameters.AddWithValue("@CNP", cnp);
            });
        }

        public bool IsGuest(string cnp)
        {
            string checkQuery = "SELECT COUNT(*) FROM [USER] WHERE CNP = @CNP";

            var result = dbConnection.ExecuteScalar(checkQuery, cmd =>
            {
                cmd.Parameters.AddWithValue("@CNP", cnp);
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
