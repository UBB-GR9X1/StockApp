using StockApp.Database;
using System;
using System.Data.SQLite;

namespace GemStore.Repositories
{
    internal class GemStoreRepository
    {
        private SQLiteConnection dbConnection = DatabaseHelper.Instance.GetConnection();

        public void PopulateHardcodedCnps()
        {
            string[] cnps = new string[] { "1234567890123", "1234567890124", "1234567890125" };
            string insertQuery = "INSERT INTO HARDCODED_CNPS (CNP) VALUES (@CNP)";
            using (var insertCommand = new SQLiteCommand(insertQuery, dbConnection))
            {
                foreach (var cnp in cnps)
                {
                    insertCommand.Parameters.AddWithValue("@CNP", cnp);
                    insertCommand.ExecuteNonQuery();
                }
            }
        }

        public void PopulateUserTable()
        {
            var users = new[]
            {
                new { CNP = "1234567890123", Name = "Emma Popescu", GemBalance = 1000 },
                new { CNP = "1234567890124", Name = "Diana Ionescu", GemBalance = 1500 },
                new { CNP = "1234567890125", Name = "Oana Georgescu", GemBalance = 200 }
            };

            string insertQuery = "INSERT INTO USER (CNP, NAME, GEM_BALANCE) VALUES (@CNP, @Name, @GemBalance)";
            using (var insertCommand = new SQLiteCommand(insertQuery, dbConnection))
            {
                foreach (var user in users)
                {
                    insertCommand.Parameters.AddWithValue("@CNP", user.CNP);
                    insertCommand.Parameters.AddWithValue("@Name", user.Name);
                    insertCommand.Parameters.AddWithValue("@GemBalance", user.GemBalance);
                    insertCommand.ExecuteNonQuery();
                }
            }
        }


        public int GetUserGemBalance(string cnp)
        {
            string checkQuery = "SELECT GEM_BALANCE FROM USER WHERE CNP = @CNP";
            using (var checkCommand = new SQLiteCommand(checkQuery, dbConnection))
            {
                checkCommand.Parameters.AddWithValue("@CNP", cnp);
                var result = checkCommand.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        public void UpdateUserGemBalance(string cnp, int newBalance)
        {
            string checkQuery = "UPDATE USER SET GEM_BALANCE = @NewBalance WHERE CNP = @CNP";
            using (var checkCommand = new SQLiteCommand(checkQuery, dbConnection))
            {
                checkCommand.Parameters.AddWithValue("@NewBalance", newBalance);
                checkCommand.Parameters.AddWithValue("@CNP", cnp);
                checkCommand.ExecuteNonQuery();
            }
        }

        public bool IsGuest(string cnp)
        {
            string checkQuery = "SELECT COUNT(*) FROM HARDCODED_CNPS WHERE CNP = @CNP";
            using (var checkCommand = new SQLiteCommand(checkQuery, dbConnection))
            {
                checkCommand.Parameters.AddWithValue("@CNP", cnp);
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                return count == 0; // not found = guest
            }
        }
    }
}
