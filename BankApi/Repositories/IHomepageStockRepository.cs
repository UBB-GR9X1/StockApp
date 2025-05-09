namespace BankApi.Repositories
{
    using BankApi.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IHomepageStockRepository
    {
        Task<List<HomepageStock>> GetAllAsync();
        Task<HomepageStock?> GetByIdAsync(int id);
        Task<HomepageStock?> GetBySymbolAsync(string symbol);
        Task<HomepageStock> CreateAsync(HomepageStock stock);
        Task<bool> UpdateAsync(int id, HomepageStock stock);
        Task<bool> DeleteAsync(int id);
    }
}
