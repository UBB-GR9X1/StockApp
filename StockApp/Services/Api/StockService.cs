namespace StockApp.Services.Api
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;
    using StockApp.Repositories;

    public class StockService(IStockRepository stockRepository) : IStockService
    {
        private readonly IStockRepository stockRepository = stockRepository;

        /// <inheritdoc/>
        public async Task<Stock> CreateStockAsync(Stock stock)
        {
            return await stockRepository.CreateAsync(stock);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteStockAsync(int id)
        {
            return await stockRepository.DeleteAsync(id);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Stock>> GetAllStocksAsync()
        {
            return await stockRepository.GetAllAsync();
        }

        /// <inheritdoc/>
        public async Task<Stock?> GetStockByIdAsync(int id)
        {
            return await stockRepository.GetByIdAsync(id);
        }

        /// <inheritdoc/>
        public async Task<Stock?> UpdateStockAsync(int id, Stock updatedStock)
        {
            return await stockRepository.UpdateAsync(id, updatedStock);
        }

        public async Task<List<Stock>> UserStocksAsync(string cnp)
        {
            return await stockRepository.UserStocksAsync(cnp);
        }
    }
}
