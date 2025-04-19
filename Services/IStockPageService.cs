namespace StockApp.Services
{
    using System.Collections.Generic;

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

        string GetStockAuthor();
    }
}
