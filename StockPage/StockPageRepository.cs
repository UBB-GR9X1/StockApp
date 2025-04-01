using StocksHomepage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.StockPage
{
    class StockPageRepository
    {
        private Database.DatabaseHelper databaseHelper;

        public StockPageRepository()
        {
            databaseHelper = Database.DatabaseHelper.Instance;
            Console.WriteLine("KMSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
        }
    }
}
