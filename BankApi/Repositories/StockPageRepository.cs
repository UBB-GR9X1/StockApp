using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApi.Data;
using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Repositories
{
    public class StockPageRepository : IStockPageRepository
    {
        private readonly ApiDbContext _context;
        private readonly string _userCNP;

        public StockPageRepository(ApiDbContext context, string userCNP)
        {
            _context = context;
            _userCNP = userCNP;
        }

        public async Task<User> GetUserAsync()
        {
            return await _context.Users.FindAsync(_userCNP);
        }

        public bool IsGuest => string.IsNullOrEmpty(_userCNP);

        public async Task UpdateUserGemsAsync(int gems)
        {
            var user = await _context.Users.FindAsync(_userCNP);
            if (user != null)
            {
                user.GemBalance = gems;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddOrUpdateUserStockAsync(string stockName, int quantity)
        {
            var userStock = await _context.UserStocks
                .FirstOrDefaultAsync(us => us.UserCNP == _userCNP && us.StockName == stockName);

            if (userStock != null)
            {
                userStock.Quantity += quantity;
            }
            else
            {
                userStock = new UserStock
                {
                    UserCNP = _userCNP,
                    StockName = stockName,
                    Quantity = quantity
                };
                _context.UserStocks.Add(userStock);
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddStockValueAsync(string stockName, int price)
        {
            var stockValue = new StockValue
            {
                StockName = stockName,
                Price = price,
                Timestamp = DateTime.UtcNow
            };

            _context.StockValues.Add(stockValue);
            await _context.SaveChangesAsync();
        }

        public async Task<Stock> GetStockAsync(string stockName)
        {
            var stock = await _context.Stocks
                .Include(s => s.StockValues.OrderByDescending(sv => sv.Timestamp).Take(1))
                .Include(s => s.UserStocks.Where(us => us.UserCNP == _userCNP))
                .FirstOrDefaultAsync(s => s.StockName == stockName);

            if (stock == null)
            {
                throw new InvalidOperationException($"Stock with name '{stockName}' not found.");
            }

            return stock;
        }

        public async Task<List<int>> GetStockHistoryAsync(string stockName)
        {
            return await _context.StockValues
                .Where(sv => sv.StockName == stockName)
                .OrderBy(sv => sv.Timestamp)
                .Select(sv => sv.Price)
                .ToListAsync();
        }

        public async Task<int> GetOwnedStocksAsync(string stockName)
        {
            var userStock = await _context.UserStocks
                .FirstOrDefaultAsync(us => us.UserCNP == _userCNP && us.StockName == stockName);

            return userStock?.Quantity ?? 0;
        }

        public async Task<bool> GetFavoriteAsync(string stockName)
        {
            return await _context.FavoriteStocks
                .AnyAsync(fs => fs.UserCNP == _userCNP && fs.StockName == stockName);
        }

        public async Task ToggleFavoriteAsync(string stockName, bool state)
        {
            var favorite = await _context.FavoriteStocks
                .FirstOrDefaultAsync(fs => fs.UserCNP == _userCNP && fs.StockName == stockName);

            if (state && favorite == null)
            {
                _context.FavoriteStocks.Add(new FavoriteStock
                {
                    UserCNP = _userCNP,
                    StockName = stockName
                });
            }
            else if (!state && favorite != null)
            {
                _context.FavoriteStocks.Remove(favorite);
            }

            await _context.SaveChangesAsync();
        }
    }
} 