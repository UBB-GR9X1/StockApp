namespace BankApi.Repositories
{
    using BankApi.Database;
    using BankApi.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class HomepageStocksRepository : IHomepageStocksRepository
    {
        private readonly AppDbContext _context;

        public HomepageStocksRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<HomepageStock>> GetAllStocksAsync()
        {
            return await _context.HomepageStocks.ToListAsync();
        }

        public async Task<HomepageStock?> GetStockByIdAsync(int id)
        {
            return await _context.HomepageStocks.FindAsync(id);
        }

        public async Task AddStockAsync(HomepageStock stock)
        {
            _context.HomepageStocks.Add(stock);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStockAsync(HomepageStock stock)
        {
            _context.HomepageStocks.Update(stock);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStockAsync(int id)
        {
            var stock = await _context.HomepageStocks.FindAsync(id);
            if (stock != null)
            {
                _context.HomepageStocks.Remove(stock);
                await _context.SaveChangesAsync();
            }
        }
    }
}
