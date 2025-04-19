using System.Collections.Generic;
using StockApp.Models;

namespace StockApp.Services
{
    internal interface IStockPageService
    {
        bool BuyStock(int quantity);
        bool GetFavorite();
        int GetOwnedStocks();
        User GetStockAuthor();
        List<int> GetStockHistory();
        string GetStockName();
        string GetStockSymbol();
        int GetUserBalance();
        bool IsGuest();
        void SelectStock(Stock stock);
        bool SellStock(int quantity);
        void ToggleFavorite(bool state);
    }
}