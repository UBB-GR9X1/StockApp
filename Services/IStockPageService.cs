namespace StockApp.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;

    public interface IStockPageService
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

        Task<int> GetUserBalanceAsync();
        Task<int> GetOwnedStocksAsync();
        Task<List<int>> GetStockHistoryAsync();
        Task<User> GetStockAuthorAsync();
        Task<bool> BuyStockAsync(int quantity);
        Task<bool> SellStockAsync(int quantity);
        Task ToggleFavoriteAsync(bool state);
    }
}