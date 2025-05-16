namespace StockApp.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Models;

    public interface IBaseStocksRepository
    {
        Task AddStockAsync(BaseStock stock, int initialPrice = 100);

        Task<List<BaseStock>> GetAllStocksAsync();
    }
}