using System.Collections.Generic;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Services
{
    /// <summary>
    /// Provides methods for managing stock entities.
    /// </summary>
    public interface IStockService
    {
        /// <summary>
        /// Creates a new stock entity.
        /// </summary>
        /// <param name="stock">The stock entity to create.</param>
        /// <returns>The created stock entity.</returns>
        Task<Stock> CreateStockAsync(Stock stock);

        /// <summary>
        /// Deletes a stock entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the stock to delete.</param>
        /// <returns>A boolean indicating whether the deletion was successful.</returns>
        Task<bool> DeleteStockAsync(int id);

        /// <summary>
        /// Retrieves all stock entities.
        /// </summary>
        /// <returns>A collection of all stock entities.</returns>
        Task<IEnumerable<Stock>> GetAllStocksAsync();

        /// <summary>
        /// Retrieves a stock entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the stock to retrieve.</param>
        /// <returns>The stock entity if found; otherwise, null.</returns>
        Task<Stock?> GetStockByIdAsync(int id);

        /// <summary>
        /// Updates an existing stock entity.
        /// </summary>
        /// <param name="id">The identifier of the stock to update.</param>
        /// <param name="updatedStock">The updated stock entity.</param>
        /// <returns>The updated stock entity if successful; otherwise, null.</returns>
        Task<Stock?> UpdateStockAsync(int id, Stock updatedStock);
    }
}