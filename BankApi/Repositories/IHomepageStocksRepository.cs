namespace BankApi.Repositories
{
    using BankApi.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IHomepageStocksRepository
    {
        Task<List<HomepageStock>> GetAllStocksAsync();
        Task<HomepageStock?> GetStockByIdAsync(int id);
        Task AddStockAsync(HomepageStock stock);
        Task UpdateStockAsync(HomepageStock stock);
        Task DeleteStockAsync(int id);
    }
}
