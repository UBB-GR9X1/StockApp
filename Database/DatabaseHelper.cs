using System;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;

namespace StockApp.Database
{
    internal class DatabaseHelper
    {
        private static string databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StockApp_DB.db");
        private static string connectionString = "Data Source=" + databasePath + ";Version=3;";
        private static SQLiteConnection _connection;
        private static DatabaseHelper _instance;

        public static DatabaseHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DatabaseHelper();
                }
                return _instance;
            }
        }

        private  DatabaseHelper()
        {
            EnsureDatabaseExists();
            OpenConnection();
        }

        

        public static void InitializeDatabase()
        {
            if (!File.Exists(databasePath))
            {
                SQLiteConnection.CreateFile(databasePath);

                using (var connectionTOBD = new SQLiteConnection(connectionString))
                {
                    connectionTOBD.Open();

                    string createUserTableQuery =
                        "CREATE TABLE USER (" +
                        " CNP TEXT PRIMARY KEY," +
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
                        " USER_CNP TEXT," +
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

                    string createNewsArticleTableQuery =
                        "CREATE TABLE NEWS_ARTICLE (" +
                        "ARTICLE_ID TEXT PRIMRY KEY," +
                        " TITLE TEXT NOT NULL," +
                        " SUMMARY TEXT," +
                        " CONTEXT TEXT NOT NULL," +
                        " SOURCE TEXT," +
                        " PUBLISH_DATE TEXT NOT NULL," +
                        " IS_READ INTEGER," +
                        " IS_WATCHLIST_RELATED INTEGER," +
                        " CATEGORY TEXT)";
                    string createUserArticleTableQuery =
                        "CREATE TABLE USER_ARTICLE (" +
                        " ARTICLE_ID TEXT PRIMARY KEY," +
                        " TITLE TEXT NOT NULL," +
                        " CONTENT TEXT NOT NULL," +
                        " AUTHOR_CNP TEXT," +
                        " SUBMISSTION_DATE TEXT," +
                        " STATUS TEXT," +
                        " TOPIC TEXT," +
                        " FOREIGN KEY (AUTHOR_CNP) REFERENCES USER(CNP))";
                    string createRelatedStocksTableQuery =
                        "CREATE TABLE RELATED_STOCKS (" +
                        " SERIAL_ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                        " STOCK_NAME TEXT NOT NULL," +
                        " ARTICLE_ID TEXT NOT NULL," +
                        " FOREIGN KEY (ARTICLE_ID) REFERENCES NEWS_ARTICLE(ARTICLE_ID)," +
                        " FOREIGN KEY (STOCK_NAME) REFERENCES STOCK(STOCK_NAME))";
                   
                    string createHardcodedCNPsTableQuery =
                        "CREATE TABLE HARDCODED_CNPS (" +
                        " CNP TEXT PRIMARY KEY)";

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
                        command.CommandText = createNewsArticleTableQuery;
                        command.ExecuteNonQuery();
                        command.CommandText = createUserArticleTableQuery;
                        command.ExecuteNonQuery();
                        command.CommandText = createRelatedStocksTableQuery;
                        command.ExecuteNonQuery();
                        command.CommandText = createHardcodedCNPsTableQuery;
                        command.ExecuteNonQuery();
                    }

                }
            }
        }
        public static string getConnectionString()
        {
            return connectionString;
        }

        public void CloseConnection()
        {
            if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        public SQLiteConnection GetConnection()
        {
            if (_connection == null || _connection.State == System.Data.ConnectionState.Closed)
            {
                OpenConnection();
            }
            return _connection;
        }

        private void OpenConnection()
        {
            if (_connection == null)
            {
                _connection = new SQLiteConnection(connectionString);
                _connection.Open();
            }
        }

        private static void EnsureDatabaseExists()
        {
            if (!File.Exists(databasePath))
            {
                SQLiteConnection.CreateFile(databasePath);
                InitializeDatabase();
            }
        }


    }
}
