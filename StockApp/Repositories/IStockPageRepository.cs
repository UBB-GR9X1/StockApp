using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace StockApp.Repositories
{
    public interface IStockPageRepository
    {
        Task AddOrUpdateUserStockAsync(string userCNP, string stockName, int quantity);
        Task AddStockValueAsync(string stockName, int price);
        Task<bool> GetFavoriteAsync(string userCNP, string stockName);
        Task<int> GetOwnedStocksAsync(string userCNP, string stockName);
        Task<Stock> GetStockAsync(string stockName);
        Task<UserStock?> GetUserStockAsync(string userCNP, string stockName);
        Task<List<int>> GetStockHistoryAsync(string stockName);
        Task ToggleFavoriteAsync(string userCNP, string stockName, bool state);
    }
}