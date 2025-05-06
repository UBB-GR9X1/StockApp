using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("StockApp.Repository.Tests")]

namespace StockApp.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using StockApp.Database;
    using StockApp.Exceptions;
    using StockApp.Models;

    /// <summary>
    /// Repository for managing <see cref="BaseStock"/> entities, including addition and retrieval from the database.
    /// </summary>
    internal class BaseStocksRepository : IBaseStocksRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseStocksRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public BaseStocksRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Adds a new stock to the database.
        /// </summary>
        /// <param name="stock">The <see cref="BaseStock"/> to add.</param>
        /// <param name="initialPrice">Initial price for the stock.</param>
        /// <returns>The added stock.</returns>
        public async Task<BaseStock> AddStockAsync(BaseStock stock, int initialPrice = 100)
        {
            if (stock == null)
            {
                throw new ArgumentNullException(nameof(stock));
            }

            try
            {
                // Check if stock with the same name already exists
                var existingStock = await _dbContext.BaseStocks.FirstOrDefaultAsync(s => s.Name == stock.Name);
                
                if (existingStock != null)
                {
                    throw new DuplicateStockException(stock.Name);
                }

                // Add the stock to the context
                await _dbContext.BaseStocks.AddAsync(stock);
                
                // Save changes to the database
                await _dbContext.SaveChangesAsync();

                // TODO: Add initial price to a stock_value table if needed
                // This would require creating a StockValue entity and DbSet

                return stock;
            }
            catch (DbUpdateException ex)
            {
                throw new StockRepositoryException("Failed to add stock to the database.", ex);
            }
        }

        /// <summary>
        /// Gets a stock by its name.
        /// </summary>
        /// <param name="name">The name of the stock to retrieve.</param>
        /// <returns>The stock with the specified name.</returns>
        public async Task<BaseStock> GetStockByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Stock name cannot be null or empty.", nameof(name));
            }

            var stock = await _dbContext.BaseStocks.FirstOrDefaultAsync(s => s.Name == name);
            
            if (stock == null)
            {
                throw new StockNotFoundException(name);
            }

            return stock;
        }

        /// <summary>
        /// Retrieves all stocks from the database.
        /// </summary>
        /// <returns>A list of all <see cref="BaseStock"/> objects.</returns>
        public async Task<List<BaseStock>> GetAllStocksAsync()
        {
            return await _dbContext.BaseStocks.ToListAsync();
        }

        /// <summary>
        /// Updates a stock in the database.
        /// </summary>
        /// <param name="stock">The updated stock.</param>
        /// <returns>The updated stock.</returns>
        public async Task<BaseStock> UpdateStockAsync(BaseStock stock)
        {
            if (stock == null)
            {
                throw new ArgumentNullException(nameof(stock));
            }

            try
            {
                var existingStock = await _dbContext.BaseStocks.FirstOrDefaultAsync(s => s.Name == stock.Name);
                
                if (existingStock == null)
                {
                    throw new StockNotFoundException(stock.Name);
                }

                // Update properties
                existingStock.Symbol = stock.Symbol;
                existingStock.AuthorCNP = stock.AuthorCNP;

                // Update the entity
                _dbContext.BaseStocks.Update(existingStock);
                
                // Save changes
                await _dbContext.SaveChangesAsync();

                return existingStock;
            }
            catch (DbUpdateException ex)
            {
                throw new StockRepositoryException($"Failed to update stock '{stock.Name}'.", ex);
            }
        }

        /// <summary>
        /// Deletes a stock from the database.
        /// </summary>
        /// <param name="name">The name of the stock to delete.</param>
        /// <returns>True if deletion was successful, false otherwise.</returns>
        public async Task<bool> DeleteStockAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Stock name cannot be null or empty.", nameof(name));
            }

            try
            {
                var stock = await _dbContext.BaseStocks.FirstOrDefaultAsync(s => s.Name == name);
                
                if (stock == null)
                {
                    return false;
                }

                _dbContext.BaseStocks.Remove(stock);
                await _dbContext.SaveChangesAsync();
                
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new StockRepositoryException($"Failed to delete stock '{name}'.", ex);
            }
        }
    }
}
