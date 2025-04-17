namespace StockApp.Repository
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.SqlClient;
    using StockApp.Models;

    public class StockPageRepository
    {
        private Database.DatabaseHelper databaseHelper;
        private string cnp;
        private StockPageUser user = null;
        private bool isGuest;

        public StockPageRepository()
        {
            databaseHelper = Database.DatabaseHelper.Instance;

            SqlCommand getCNP = new SqlCommand("SELECT * FROM HARDCODED_CNPS", databaseHelper.GetConnection());
            SqlDataReader reader = getCNP.ExecuteReader();
            reader.Read();
            cnp = reader["CNP"].ToString();
            Console.WriteLine("CNP: " + cnp);

            SqlCommand getUser = new SqlCommand("SELECT * FROM [USER] WHERE CNP = @cnp", databaseHelper.GetConnection());
            getUser.Parameters.AddWithValue("@cnp", cnp);
            SqlDataReader reader2 = getUser.ExecuteReader();
            reader2.Read();
            if (reader2.HasRows)
            {
                user = new StockPageUser(reader2["CNP"].ToString(), reader2["NAME"].ToString(), Convert.ToInt32(reader2["GEM_BALANCE"]));
                isGuest = false;
            }
            else
            {
                isGuest = true;
            }
        }
        public bool IsGuest()
        {
            return isGuest;
        }

        public StockPageUser GetUser()
        {
            return user;
        }

        public void UpdateUserGems(int gems)
        {
            SqlCommand updateUser = new SqlCommand("UPDATE [USER] SET GEM_BALANCE = @gems WHERE CNP = @cnp", databaseHelper.GetConnection());
            updateUser.Parameters.AddWithValue("@gems", gems);
            updateUser.Parameters.AddWithValue("@cnp", cnp);
            updateUser.ExecuteNonQuery();
            user.GemBalance = gems;
        }

        public void AddOrUpdateUserStock(string stockName, int quantity)
        {
            SqlCommand addOrUpdateUserStock = new SqlCommand("IF EXISTS (SELECT * FROM USER_STOCK WHERE USER_CNP = @cnp AND STOCK_NAME = @name) " +
                "BEGIN UPDATE USER_STOCK SET QUANTITY = QUANTITY + @quantity WHERE USER_CNP = @cnp AND STOCK_NAME = @name END " +
                "ELSE BEGIN INSERT INTO USER_STOCK (USER_CNP, STOCK_NAME, QUANTITY) VALUES (@cnp, @name, @quantity) END", databaseHelper.GetConnection());
            addOrUpdateUserStock.Parameters.AddWithValue("@cnp", cnp);
            addOrUpdateUserStock.Parameters.AddWithValue("@name", stockName);
            addOrUpdateUserStock.Parameters.AddWithValue("@quantity", quantity);
            addOrUpdateUserStock.ExecuteNonQuery();
        }

        public void AddStockValue(string stockName, int price)
        {
            SqlCommand addStockValue = new SqlCommand("INSERT INTO STOCK_VALUE (STOCK_NAME, PRICE) VALUES (@name, @price)", databaseHelper.GetConnection());
            addStockValue.Parameters.AddWithValue("@name", stockName);
            addStockValue.Parameters.AddWithValue("@price", price);
            addStockValue.ExecuteNonQuery();
        }

        public StockPageStock GetStock(string stockName)
        {
            SqlCommand getStock = new SqlCommand("SELECT * FROM STOCK WHERE STOCK_NAME = @name", databaseHelper.GetConnection());
            getStock.Parameters.AddWithValue("@name", stockName);
            SqlDataReader reader = getStock.ExecuteReader();
            reader.Read();
            return new StockPageStock(reader["STOCK_NAME"].ToString(), reader["STOCK_SYMBOL"].ToString(), reader["AUTHOR_CNP"].ToString());
        }

        public List<int> GetStockHistory(string stockName)
        {
            SqlCommand getStock = new SqlCommand("SELECT * FROM STOCK_VALUE WHERE STOCK_NAME = @name", databaseHelper.GetConnection());
            getStock.Parameters.AddWithValue("@name", stockName);
            SqlDataReader reader = getStock.ExecuteReader();
            List<int> stock_values = new List<int>();
            while (reader.Read())
            {
                stock_values.Add(Convert.ToInt32(reader["PRICE"]));
            }
            return stock_values;
        }

        public int GetOwnedStocks(string stockName)
        {
            SqlCommand getOwnedStock = new SqlCommand("SELECT * FROM USER_STOCK WHERE USER_CNP = @cnp AND STOCK_NAME = @name", databaseHelper.GetConnection());
            getOwnedStock.Parameters.AddWithValue("@cnp", cnp);
            getOwnedStock.Parameters.AddWithValue("@name", stockName);
            SqlDataReader reader = getOwnedStock.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                return Convert.ToInt32(reader["QUANTITY"]);
            }
            else
            {
                return 0;
            }
        }

        public bool GetFavorite(string stockName)
        {
            SqlCommand getFavorite = new SqlCommand("SELECT * FROM FAVORITE_STOCK WHERE USER_CNP = @cnp AND STOCK_NAME = @name", databaseHelper.GetConnection());
            getFavorite.Parameters.AddWithValue("@cnp", cnp);
            getFavorite.Parameters.AddWithValue("@name", stockName);
            SqlDataReader reader = getFavorite.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ToggleFavorite(string stockName, bool state)
        {
            if (state)
            {
                SqlCommand addFavorite = new SqlCommand("INSERT INTO FAVORITE_STOCK (USER_CNP, STOCK_NAME) VALUES (@cnp, @name)", databaseHelper.GetConnection());
                addFavorite.Parameters.AddWithValue("@cnp", cnp);
                addFavorite.Parameters.AddWithValue("@name", stockName);
                addFavorite.ExecuteNonQuery();
            }
            else
            {
                SqlCommand removeFavorite = new SqlCommand("DELETE FROM FAVORITE_STOCK WHERE USER_CNP = @cnp AND STOCK_NAME = @name", databaseHelper.GetConnection());
                removeFavorite.Parameters.AddWithValue("@cnp", cnp);
                removeFavorite.Parameters.AddWithValue("@name", stockName);
                removeFavorite.ExecuteNonQuery();
            }
        }
    }
}
