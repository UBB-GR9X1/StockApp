using System.Collections.Generic;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Services
{
    /// <summary>
    /// Business-layer contract for working with <see cref="BaseStock"/> objects.
    /// </summary>
    public interface IBaseStocksService
    {
        /// <summary>Returns every stock known to the system.</summary>
        Task<List<BaseStock>> GetAllStocksAsync();

        /// <summary>Adds a new stock (and its initial price) to the system.</summary>
        Task AddStockAsync(BaseStock stock, int initialPrice = 100);
    }
}
