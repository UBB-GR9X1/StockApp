namespace StockApp.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;

    public interface IBaseStocksApiService
    {
        Task<List<BaseStock>> GetAllStocksAsync();
        Task<BaseStock> GetStockByNameAsync(string name);
        Task<BaseStock> CreateStockAsync(BaseStock stock, int? initialPrice = null);
        Task<BaseStock> UpdateStockAsync(BaseStock stock);
        Task<bool> DeleteStockAsync(string name);
    }
} 