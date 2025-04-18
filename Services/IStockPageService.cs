namespace StockApp.Services
{
    using System.Collections.Generic;
    using StockApp.Models;

    public interface IStockPageService
    {
        bool IsGuest();

        string GetStockName();

        string GetStockSymbol();

        int GetUserBalance();

        List<int> GetStockHistory();

        int GetOwnedStocks();

        bool BuyStock(int quantity);

        bool SellStock(int quantity);

        bool GetFavorite();

        void ToggleFavorite(bool state);

        User GetStockAuthor();

        void SelectStock(Stock stock);
    }
}
