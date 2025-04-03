using StockApp.StockPage.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace StockApp.StockPage
{
    class StockPageRepository
    {
        private Database.DatabaseHelper databaseHelper;
        private string cnp;

        public StockPageRepository()
        {
            databaseHelper = Database.DatabaseHelper.Instance;

            SqlCommand getCNP = new SqlCommand("SELECT * FROM HARDCODED_CNPS", databaseHelper.GetConnection());
            SqlDataReader reader = getCNP.ExecuteReader();
            reader.Read();
            this.cnp = reader["CNP"].ToString();
        }

        public Stock GetStock(string stockName)
        {
            SqlCommand getStock = new SqlCommand("SELECT * FROM STOCK WHERE STOCK_NAME = @name", databaseHelper.GetConnection());
            getStock.Parameters.AddWithValue("@name", stockName);
            SqlDataReader reader = getStock.ExecuteReader();
            reader.Read();
            return new Stock(reader["STOCK_NAME"].ToString(), reader["STOCK_SYMBOL"].ToString(), reader["AUTHOR_CNP"].ToString());
        }
    }
}
