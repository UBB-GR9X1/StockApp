using System.Collections.Generic;
using System.Threading.Tasks;
using BankApi.Models;

namespace BankApi.Repositories
{
    public interface IBaseStocksRepository
    {
        Task<BaseStock> AddStockAsync(BaseStock stock, int initialPrice = 100);

        Task<List<BaseStock>> GetAllStocksAsync();

        Task<BaseStock> GetStockByNameAsync(string name);

        Task<bool> DeleteStockAsync(string name);

        Task<BaseStock> UpdateStockAsync(BaseStock stock);
    }
} 