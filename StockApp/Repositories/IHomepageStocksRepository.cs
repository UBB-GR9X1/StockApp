namespace StockApp.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Models;

    public interface IHomepageStocksRepository
    {
        Task<List<HomepageStock>> GetAllStocksAsync();

        Task<HomepageStock> GetStockByIdAsync(int id);

        Task<bool> AddStockAsync(HomepageStock stock);

        Task<bool> UpdateStockAsync(int id, HomepageStock stock);

        Task<bool> DeleteStockAsync(int id);
    }
}
