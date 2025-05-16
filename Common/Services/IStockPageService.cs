namespace Common.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Models;

    public interface IStockPageService
    {
        // merge with StocksService
        Task<bool> BuyStockAsync(int quantity);

        Task<bool> GetFavoriteAsync();

        Task<int> GetOwnedStocksAsync();

        Task<UserStock> GetUserStockAsync();

        Task<User> GetStockAuthorAsync();

        Task<List<int>> GetStockHistoryAsync();

        Task<string> GetStockNameAsync();

        Task<string> GetStockSymbolAsync();

        Task<int> GetUserBalanceAsync();

        void SelectStock(Stock stock);

        Task<bool> SellStockAsync(int quantity);

        Task ToggleFavoriteAsync(bool state);
    }
}