using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;
using Common.Services;
using StockApp.Repositories;

namespace StockApp.Services.Api
{
    /// <summary>
    /// Thin service wrapper around <see cref="IBaseStocksRepository"/> that exposes an async API.
    /// </summary>
    public class BaseStocksService : IBaseStocksService
    {
        private readonly IBaseStocksRepository _repo;

        public BaseStocksService(IBaseStocksRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /*────────────────────  Queries  ────────────────────*/

        public async Task<List<BaseStock>> GetAllStocksAsync()
        {
            return await _repo.GetAllStocksAsync();
        }


        /*───────────────────  Commands  ────────────────────*/

        public async Task AddStockAsync(BaseStock stock, int initialPrice = 100)
        {
            if (stock is null) throw new ArgumentNullException(nameof(stock));

            // Repository call is synchronous; execute and return a completed Task
            await _repo.AddStockAsync(stock, initialPrice);
        }
    }
}
