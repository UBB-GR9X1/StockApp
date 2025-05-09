namespace BankApi.Repositories
{
<<<<<<<< HEAD:BankApi/Repositories/IBaseStocksRepository.cs
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BankApi.Models;

    public interface IBaseStocksRepository
========
    public interface IBaseStockRepository
>>>>>>>> migrate-homepage-stock-repo2:BankApi/Repositories/IBaseStockRepository.cs
    {
        Task<BaseStock> AddStockAsync(BaseStock stock, int initialPrice = 100);

        Task<List<BaseStock>> GetAllStocksAsync();

        Task<BaseStock> GetStockByNameAsync(string name);

        Task<bool> DeleteStockAsync(string name);

        Task<BaseStock> UpdateStockAsync(BaseStock stock);
    }
}