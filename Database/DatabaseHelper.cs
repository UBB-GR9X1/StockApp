using System;
using System.Data.SQLite;
using System.IO;

namespace StockApp.Database
{
    internal class DatabaseHelper
    {
        private static string connectionString = @"Data Source=C:\Users\Johnnyboy\Desktop\Faculty\ANU 2\sem2\ISS\StockApp\Database\StockApp_DB.db;Version=3;";

        public static void InitializeDatabase()
        {
            if (!File.Exists(@"C:\Users\Johnnyboy\Desktop\Faculty\ANU 2\sem2\ISS\StockApp\Database\StockApp_DB.db"))
            {
                SQLiteConnection.CreateFile(@"C:\Users\Johnnyboy\Desktop\Faculty\ANU 2\sem2\ISS\StockApp\Database\StockApp_DB.db");

                using (var connectionTOBD = new SQLiteConnection(connectionString))
                {
                    connectionTOBD.Open();

                    string createUserTableQuery =
                        "CREATE TABLE USER (" +
                        "CNP TEXT PRIMARY KEY," +
                        " NAME TEXT," +
                        " DESCRIPTION TEXT," +
                        " IS_HIDDEN INTEGER," +
                        " IS_ADMIN INTEGER," +
                        " PROFILE_PICTURE TEXT," +
                        " GEM_BALANCE INTEGER)";

                    string createStockTableQuery =
                        "CREATE TABLE STOCK (" +
                        "STOCK_NAME TEXT PRIMARY KEY," +
                        " STOCK_SYMBOL TEXT," +
                        " AUTHOR_CNP TEXT," +
                        " FOREIGN KEY (AUTHOR_CNP) REFERENCES USER(CNP))";

                    string createStockValueTableQuery =
                        "CREATE TABLE STOCK_VALUE (" +
                        "STOCK_NAME TEXT," +
                        " PRICE INTEGER," +
                        " FOREIGN KEY (STOCK_NAME) REFERENCES STOCK(STOCK_NAME))";

                    string createUserStockTableQuery =
                        "CREATE TABLE USER_STOCK (" +
                        "USER_CNP TEXT," +
                        " STOCK_NAME TEXT," +
                        " QUANTITY INTEGER," +
                        " FOREIGN KEY (USER_CNP) REFERENCES USER(CNP)," +
                        " FOREIGN KEY (STOCK_NAME) REFERENCES STOCK(STOCK_NAME))";

                    string createFavoriteStockTableQuery =
                        "CREATE TABLE FAVORITE_STOCK (" +
                        "USER_CNP TEXT," +
                        " STOCK_NAME TEXT," +
                        " IS_FAVORITE INTEGER," +
                        " FOREIGN KEY (USER_CNP) REFERENCES USER(CNP)," +
                        " FOREIGN KEY (STOCK_NAME) REFERENCES STOCK(STOCK_NAME))";

                    string createTransactionTableQuery =
                        "CREATE TABLE USERS_TRANSACTION (" +
                        "TRANSACTION_ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                        " USER_CNP TEXT," +
                        " STOCK_NAME TEXT," +
                        " TYPE INTEGER," +
                        " QUANTITY INTEGER," +
                        " PRICE INTEGER," +
                        " DATE TEXT," +
                        " FOREIGN KEY (USER_CNP) REFERENCES USER(CNP)," +
                        " FOREIGN KEY (STOCK_NAME) REFERENCES STOCK(STOCK_NAME))";

                    string createAlertsTableQuery =
                        "CREATE TABLE ALERTS (" +
                        "ALERT_ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                        " STOCK_NAME TEXT," +
                        " NAME TEXT," +
                        " LOWER_BOUND INTEGER," +
                        " UPPER_BOUND INTEGER," +
                        " TOGGLE INTEGER," +
                        " FOREIGN KEY (STOCK_NAME) REFERENCES STOCK(STOCK_NAME))";

                    using (var command = new SQLiteCommand(connectionTOBD))
                    {
                        command.CommandText = createUserTableQuery;
                        command.ExecuteNonQuery();
                        command.CommandText = createStockTableQuery;
                        command.ExecuteNonQuery();
                        command.CommandText = createStockValueTableQuery;
                        command.ExecuteNonQuery();
                        command.CommandText = createUserStockTableQuery;
                        command.ExecuteNonQuery();
                        command.CommandText = createFavoriteStockTableQuery;
                        command.ExecuteNonQuery();
                        command.CommandText = createTransactionTableQuery;
                        command.ExecuteNonQuery();
                        command.CommandText = createAlertsTableQuery;
                        command.ExecuteNonQuery();
                    }


                    string insertUsersQuery =
                       "INSERT INTO USER (CNP, NAME, DESCRIPTION, IS_HIDDEN, IS_ADMIN, PROFILE_PICTURE, GEM_BALANCE) VALUES " +
                       "('1234567890123', 'Admin', 'System Administrator', 0, 1, 'C:/Users/Johnnyboy/Desktop/ProfilePic.png', 1000), " +
                       "('9876543210987', 'John Doe', 'Regular User', 0, 0, 'C:/Users/Johnnyboy/Desktop/UserPic.png', 500);";

                    string insertStocksQuery =
                        "INSERT INTO STOCK (STOCK_NAME, STOCK_SYMBOL, AUTHOR_CNP) VALUES " +
                        "('Tesla Inc.', 'TSLA', '1234567890123'), " +
                        "('Apple Inc.', 'AAPL', '9876543210987');";

                    using (var command = new SQLiteCommand(connectionTOBD))
                    {
                        command.CommandText = insertUsersQuery;
                        command.ExecuteNonQuery();
                        command.CommandText = insertStocksQuery;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public static string getConnectionString()
        {
            return connectionString;
        }

    }
}
