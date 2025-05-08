namespace StockApp.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;

    public interface IHomepageStocksProxyRepository
    {
        Task<List<HomepageStock>> GetAllAsync();
        Task<HomepageStock?> GetByIdAsync(int id);
        Task<HomepageStock?> GetBySymbolAsync(string symbol);
        Task<bool> CreateAsync(HomepageStock homepageStock);
        Task<bool> UpdateAsync(int id, HomepageStock homepageStock);
        Task<bool> DeleteAsync(int id);
    }
}
