using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockApp.Models;
using StockApp.Repositories;

namespace StockApp.Services
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

        public Task<List<BaseStock>> GetAllStocksAsync() =>
            // Repository is synchronous, so wrap with Task.FromResult
            Task.FromResult(_repo.GetAllStocks());

        /*───────────────────  Commands  ────────────────────*/

        public Task AddStockAsync(BaseStock stock, int initialPrice = 100)
        {
            if (stock is null) throw new ArgumentNullException(nameof(stock));

            // Repository call is synchronous; execute and return a completed Task
            _repo.AddStock(stock, initialPrice);
            return Task.CompletedTask;
        }
    }
}
