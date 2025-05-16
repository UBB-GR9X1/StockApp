namespace StockApp.Services
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
            return await this.stockRepository.CreateAsync(stock);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteStockAsync(int id)
        {
            return await this.stockRepository.DeleteAsync(id);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Stock>> GetAllStocksAsync()
        {
            return await this.stockRepository.GetAllAsync();
        }

        /// <inheritdoc/>
        public async Task<Stock?> GetStockByIdAsync(int id)
        {
            return await this.stockRepository.GetByIdAsync(id);
        }

        /// <inheritdoc/>
        public async Task<Stock?> UpdateStockAsync(int id, Stock updatedStock)
        {
            return await this.stockRepository.UpdateAsync(id, updatedStock);
        }
    }
}
