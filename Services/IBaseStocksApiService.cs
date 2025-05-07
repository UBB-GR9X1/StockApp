using System.Collections.Generic;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Services
{
    public interface IBaseStocksApiService
    {
        Task<List<BaseStock>> GetAllStocksAsync();
        Task<BaseStock> GetStockByNameAsync(string name);
        Task<bool> AddStockAsync(BaseStock stock, int initialPrice = 100);
        Task<bool> UpdateStockAsync(BaseStock stock);
        Task<bool> DeleteStockAsync(string name);
    }
} 