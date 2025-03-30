using ISS_CreateStockPage.Commands;
using ISS_CreateStockPage.Models;
using StockApp.Views;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace ISS_CreateStockPage.ViewModels
{
    public class CreateStockViewModel
    {
        private StockManager repo = new StockManager();

        public CreateStockViewModel() { }

        public ObservableCollection<Stock> Stocks => StockManager.GetStocks();

        public void AddStock(Stock stock)
        {
            repo.AddStock(stock);
        }

        public string validateStock (Stock s)
        {

            if (!Regex.IsMatch(s.Name, "^[A-Za-z ]{1,20}$"))
                return "Invalid Stock Name! Only letters and spaces, max 20 characters.";

            if (s.Quantity < 1 || s.Quantity > 1000000)
                return "Invalid Stock Quantity! Must be between 1 and 1,000,000.";

            if (s.Symbol.Length < 1 || s.Symbol.Length > 5)
                return "Invalid Stock Symbol! Max 5 characters.";

            if (!Regex.IsMatch(s.AuthorCNP, @"^\d{1,13}$"))
                return "Invalid Author CNP! Only numbers, max 13 characters.";

            if (s.Price < 1 || s.Price > 100)
                return "Invalid Stock Price! Must be between 1 and 100.";

            return ""; 
        }
    }
}
