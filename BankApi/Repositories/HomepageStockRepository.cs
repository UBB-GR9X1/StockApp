namespace BankApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BankApi.Data;
    using BankApi.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

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
                var a = await _context.HomepageStocks
                    .Include(hs => hs.StockDetails)
                    .OrderBy(s => s.Id)
                    .ToListAsync();
                return a;
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
                return await _context.HomepageStocks
                    .Include(hs => hs.StockDetails)
                    .FirstOrDefaultAsync(hs => hs.Id == id);
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
                    .Include(hs => hs.StockDetails)
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
                var existingStock = await _context.HomepageStocks
                    .Include(hs => hs.StockDetails)
                    .FirstOrDefaultAsync(hs => hs.Id == id);

                if (existingStock == null)
                {
                    return false;
                }

                existingStock.Symbol = updatedStock.Symbol;
                existingStock.StockDetails = updatedStock.StockDetails;
                existingStock.Change = updatedStock.Change;

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
                var stock = await _context.HomepageStocks
                    .Include(hs => hs.StockDetails)
                    .FirstOrDefaultAsync(hs => hs.Id == id);

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
