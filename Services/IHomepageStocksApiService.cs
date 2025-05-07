namespace StockApp.Services
{
    using StockApp.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IHomepageStocksApiService
    {
        Task<List<HomepageStock>> GetAllStocksAsync();
        Task<HomepageStock> GetStockByIdAsync(int id);
        Task<bool> AddStockAsync(HomepageStock stock);
        Task<bool> UpdateStockAsync(HomepageStock stock);
        Task<bool> DeleteStockAsync(int id);
    }
}
