namespace StockApp.Repositories
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.SqlClient;
    using StockApp.Database;
    using StockApp.Models;

    public class StockPageRepository : IStockPageRepository
    {
        private readonly string cnp;

        public StockPageUser User { get; private set; }

        public bool IsGuest { get; private set; }

        public StockPageRepository()
        {
            SqlCommand getCNP = new("SELECT * FROM HARDCODED_CNPS", DatabaseHelper.GetConnection());

            SqlDataReader reader = getCNP.ExecuteReader();
            reader.Read();

            this.cnp = reader["CNP"].ToString();
            Console.WriteLine("CNP: " + this.cnp);

            SqlCommand getUser = new("SELECT * FROM [USER] WHERE CNP = @cnp", DatabaseHelper.GetConnection());
            getUser.Parameters.AddWithValue("@cnp", this.cnp);

            SqlDataReader reader2 = getUser.ExecuteReader();
            reader2.Read();

            if (reader2.HasRows)
            {
                this.User = new StockPageUser(
                    reader2["CNP"].ToString(),
                    reader2["NAME"].ToString(),
                    Convert.ToInt32(reader2["GEM_BALANCE"]));

                this.IsGuest = false;
            }
            else
            {
                this.IsGuest = true;
            }
        }

        public void UpdateUserGems(int gems)
        {
            SqlCommand updateUser = new("UPDATE [USER] SET GEM_BALANCE = @gems WHERE CNP = @cnp", DatabaseHelper.GetConnection());
            updateUser.Parameters.AddWithValue("@gems", gems);
            updateUser.Parameters.AddWithValue("@cnp", cnp);
            updateUser.ExecuteNonQuery();
            this.User.GemBalance = gems;
        }

        public void AddOrUpdateUserStock(string stockName, int quantity)
        {
            SqlCommand addOrUpdateUserStock = new("IF EXISTS (SELECT * FROM USER_STOCK WHERE USER_CNP = @cnp AND STOCK_NAME = @name) " +
                "BEGIN UPDATE USER_STOCK SET QUANTITY = QUANTITY + @quantity WHERE USER_CNP = @cnp AND STOCK_NAME = @name END " +
                "ELSE BEGIN INSERT INTO USER_STOCK (USER_CNP, STOCK_NAME, QUANTITY) VALUES (@cnp, @name, @quantity) END", DatabaseHelper.GetConnection());

            addOrUpdateUserStock.Parameters.AddWithValue("@cnp", cnp);
            addOrUpdateUserStock.Parameters.AddWithValue("@name", stockName);
            addOrUpdateUserStock.Parameters.AddWithValue("@quantity", quantity);

            addOrUpdateUserStock.ExecuteNonQuery();
        }

        public void AddStockValue(string stockName, int price)
        {
            SqlCommand addStockValue = new("INSERT INTO STOCK_VALUE (STOCK_NAME, PRICE) VALUES (@name, @price)", DatabaseHelper.GetConnection());

            addStockValue.Parameters.AddWithValue("@name", stockName);
            addStockValue.Parameters.AddWithValue("@price", price);

            addStockValue.ExecuteNonQuery();
        }

        public StockPageStock GetStock(string stockName)
        {
            SqlCommand getStock = new("SELECT * FROM STOCK WHERE STOCK_NAME = @name", DatabaseHelper.GetConnection());
            getStock.Parameters.AddWithValue("@name", stockName);

            SqlDataReader reader = getStock.ExecuteReader();
            reader.Read();

            return new StockPageStock(
                reader["STOCK_NAME"].ToString(),
                reader["STOCK_SYMBOL"].ToString(),
                reader["AUTHOR_CNP"].ToString());
        }

        public IReadOnlyList<int> GetStockHistory(string stockName)
        {
            SqlCommand getStock = new("SELECT * FROM STOCK_VALUE WHERE STOCK_NAME = @name", DatabaseHelper.GetConnection());
            getStock.Parameters.AddWithValue("@name", stockName);

            SqlDataReader reader = getStock.ExecuteReader();

            List<int> stock_values = [];
            while (reader.Read())
            {
                stock_values.Add(Convert.ToInt32(reader["PRICE"]));
            }

            return stock_values;
        }

        public int GetOwnedStocks(string stockName)
        {
            SqlCommand getOwnedStock = new SqlCommand("SELECT * FROM USER_STOCK WHERE USER_CNP = @cnp AND STOCK_NAME = @name", DatabaseHelper.GetConnection());
            getOwnedStock.Parameters.AddWithValue("@cnp", cnp);
            getOwnedStock.Parameters.AddWithValue("@name", stockName);

            SqlDataReader reader = getOwnedStock.ExecuteReader();
            reader.Read();

            return reader.HasRows ? Convert.ToInt32(reader["QUANTITY"]) : 0;
        }

        public bool GetFavorite(string stockName)
        {
            SqlCommand getFavorite = new SqlCommand("SELECT * FROM FAVORITE_STOCK WHERE USER_CNP = @cnp AND STOCK_NAME = @name", DatabaseHelper.GetConnection());
            getFavorite.Parameters.AddWithValue("@cnp", cnp);
            getFavorite.Parameters.AddWithValue("@name", stockName);

            SqlDataReader reader = getFavorite.ExecuteReader();
            reader.Read();

            return reader.HasRows;
        }

        public void ToggleFavorite(string stockName, bool state)
        {
            if (state)
            {
                SqlCommand addFavorite = new("INSERT INTO FAVORITE_STOCK (USER_CNP, STOCK_NAME) VALUES (@cnp, @name)", DatabaseHelper.GetConnection());
                addFavorite.Parameters.AddWithValue("@cnp", cnp);
                addFavorite.Parameters.AddWithValue("@name", stockName);

                addFavorite.ExecuteNonQuery();
                return;
            }

            SqlCommand removeFavorite = new("DELETE FROM FAVORITE_STOCK WHERE USER_CNP = @cnp AND STOCK_NAME = @name", DatabaseHelper.GetConnection());
            removeFavorite.Parameters.AddWithValue("@cnp", cnp);
            removeFavorite.Parameters.AddWithValue("@name", stockName);
            removeFavorite.ExecuteNonQuery();
        }
    }
}
