using System.Collections.Generic;
using System.Threading.Tasks;
using BankApi.Models;

namespace BankApi.Repositories
{
    public interface IStockPageRepository
    {
        Task<User> GetUserAsync();
        bool IsGuest { get; }
        Task UpdateUserGemsAsync(int gems);
        Task AddOrUpdateUserStockAsync(string stockName, int quantity);
        Task AddStockValueAsync(string stockName, int price);
        Task<Stock> GetStockAsync(string stockName);
        Task<List<int>> GetStockHistoryAsync(string stockName);
        Task<int> GetOwnedStocksAsync(string stockName);
        Task<bool> GetFavoriteAsync(string stockName);
        Task ToggleFavoriteAsync(string stockName, bool state);
    }
} 