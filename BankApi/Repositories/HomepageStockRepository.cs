namespace BankApi.Repositories
{
    using BankApi.Data;
    using BankApi.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class HomepageStockRepository : IHomepageStockRepository
    {
        private readonly ApiDbContext _context;
        private readonly ILogger<HomepageStockRepository> _logger;

        public HomepageStockRepository(ApiDbContext context, ILogger<HomepageStockRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<HomepageStock>> GetAllAsync()
        {
            try
            {
                return await _context.HomepageStocks.OrderBy(s => s.Id).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all homepage stocks");
                throw;
            }
        }

        public async Task<HomepageStock?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.HomepageStocks.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving homepage stock with ID {Id}", id);
                throw;
            }
        }

        public async Task<HomepageStock?> GetBySymbolAsync(string symbol)
        {
            try
            {
                return await _context.HomepageStocks
                    .FirstOrDefaultAsync(hs => hs.Symbol == symbol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving homepage stock with symbol {Symbol}", symbol);
                throw;
            }
        }

        public async Task<HomepageStock> CreateAsync(HomepageStock stock)
        {
            try
            {
                await _context.HomepageStocks.AddAsync(stock);
                await _context.SaveChangesAsync();
                return stock;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new homepage stock");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int id, HomepageStock updatedStock)
        {
            try
            {
                var existingStock = await _context.HomepageStocks.FindAsync(id);
                if (existingStock == null)
                {
                    return false;
                }

                existingStock.Symbol = updatedStock.Symbol;
                existingStock.CompanyName = updatedStock.CompanyName;
                existingStock.Price = updatedStock.Price;
                existingStock.Change = updatedStock.Change;
                existingStock.PercentChange = updatedStock.PercentChange;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating homepage stock with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var stock = await _context.HomepageStocks.FindAsync(id);
                if (stock == null)
                {
                    return false;
                }

                _context.HomepageStocks.Remove(stock);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting homepage stock with ID {Id}", id);
                throw;
            }
        }
    }
}
