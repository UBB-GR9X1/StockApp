using StockApp.StockPage.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.StockPage
{
    class StockPageRepository
    {
        private Database.DatabaseHelper databaseHelper;
        private string cnp;

        public StockPageRepository()
        {
            databaseHelper = Database.DatabaseHelper.Instance;

            SQLiteCommand getCNP = new SQLiteCommand("SELECT * FROM HARDCODED_CNPS", databaseHelper.GetConnection());
            SQLiteDataReader reader = getCNP.ExecuteReader();
            reader.Read();
            this.cnp = reader["CNP"].ToString();
        }

        public Stock GetStock(string stockName)
        {
            SQLiteCommand getStock = new SQLiteCommand("SELECT * FROM STOCK WHERE STOCK_NAME = @name", databaseHelper.GetConnection());
            getStock.Parameters.AddWithValue("@name", stockName);
            SQLiteDataReader reader = getStock.ExecuteReader();
            reader.Read();
            return new Stock(reader["STOCK_NAME"].ToString(), reader["STOCK_SYMBOL"].ToString(), reader["AUTHOR_CNP"].ToString());
        }
    }
}
