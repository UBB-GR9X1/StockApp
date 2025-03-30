using StockApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS_CreateStockPage.Models
{
    class StockManager
    {
        public static ObservableCollection<Stock> _databaseStocks = new ObservableCollection<Stock>() 
        {
            new Stock("Apple Inc.", "AAPL", 100, 150.00f, "1234567890123"),
            new Stock("Microsoft Corporation", "MSFT", 200, 250.00f, "1234567890123"),
            new Stock("Tesla Inc.", "TSLA", 300, 350.00f, "1234567890123"),
            new Stock("Amazon.com Inc.", "AMZN", 400, 450.00f, "1234567890123"),
            new Stock("Alphabet Inc.", "GOOGL", 500, 550.00f, "1234567890123"),
            new Stock("Facebook Inc.", "FB", 600, 650.00f, "1234567890123"),
            new Stock("NVIDIA Corporation", "NVDA", 700, 750.00f, "1234567890123"),
            new Stock("PayPal Holdings Inc.", "PYPL", 800, 850.00f, "1234567890123"),
            new Stock("Netflix Inc.", "NFLX", 900, 950.00f, "1234567890123"),
            new Stock("Adobe Inc.", "ADBE", 1000, 1050.00f, "1234567890123")
        };

        public static ObservableCollection<Stock> GetStocks()
        {
            return _databaseStocks;
        }

        public void AddStock(Stock stock)
        {
            _databaseStocks.Add(stock);
        }

    }
}
