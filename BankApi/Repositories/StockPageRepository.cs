namespace BankApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BankApi.Data;
    using BankApi.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Repository for managing stock page data, including user and stock information.
    /// </summary>
    public class StockPageRepository : IStockPageRepository
    {
        private readonly ApiDbContext _context;

        public StockPageRepository(ApiDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Updates the user's gem balance in the database.
        /// </summary>
        /// <param name="userCNP">CNP of the user whose balance is to be updated.</param>
        /// <param name="newGemBalance">New gem balance to set.</param>
        public async Task UpdateUserGemsAsync(string userCNP, int newGemBalance)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.CNP == userCNP);
            if (user != null)
            {
                user.GemBalance = newGemBalance;
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Adds or updates the quantity of a user's stock holding.
        /// </summary>
        /// <param name="userCNP">CNP of the user whose balance is to be updated.</param>
        /// <param name="stockName">Name of the stock.</param>
        /// <param name="quantity">Quantity to add to existing holdings.</param>
        public async Task AddOrUpdateUserStockAsync(string userCNP, string stockName, int quantity)
        {
            var userStock = await _context.UserStocks
                .FirstOrDefaultAsync(us => us.UserCnp == userCNP && us.StockName == stockName);

            if (userStock != null)
            {
                userStock.Quantity += quantity;
            }
            else
            {
                userStock = new UserStock
                {
                    UserCnp = userCNP,
                    StockName = stockName,
                    Quantity = quantity
                };
                await _context.UserStocks.AddAsync(userStock);
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Inserts a new stock price record.
        /// </summary>
        /// <param name="stockName">Name of the stock.</param>
        /// <param name="price">Price to record.</param>
        public async Task AddStockValueAsync(string stockName, int price)
        {
            var stockValue = new StockValue
            {
                StockName = stockName,
                Price = price
            };

            await _context.AddAsync(stockValue);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves a <see cref="Stock"/> by name, including the latest price and user quantity.
        /// </summary>
        /// <param name="userCNP">CNP of the user whose stock is to be retrieved.</param>
        /// <param name="stockName">Name of the stock.</param>
        /// <returns>A <see cref="Stock"/> instance with populated fields.</returns>
        public async Task<Stock> GetStockAsync(string userCNP, string stockName)
        {
            var stock = await _context.UserStocks
                .Include(us => us.Stock)
                .FirstOrDefaultAsync(us => us.UserCnp == userCNP && us.Stock.Name == stockName) ?? throw new InvalidOperationException($"Stock with name '{stockName}' not found.");
            return new Stock
            {
                Name = stock.StockName,
                Symbol = stock.Stock.Symbol,
                Price = stock.Stock.Price,
                Quantity = stock.Quantity
            };
        }

        /// <summary>
        /// Retrieves the full price history for a given stock.
        /// </summary>
        /// <param name="stockName">Name of the stock.</param>
        /// <returns>List of historical prices.</returns>
        public async Task<List<int>> GetStockHistoryAsync(string stockName)
        {
            return await _context.StockValues
                .Where(sv => sv.StockName == stockName)
                .Select(sv => sv.Price)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves the quantity of stocks owned by the user.
        /// </summary>
        /// <param name="userCNP">CNP of the user whose stock quantity is to be retrieved.</param>
        /// <param name="stockName">Name of the stock.</param>
        /// <returns>Quantity owned.</returns>
        public async Task<int> GetOwnedStocksAsync(string userCNP, string stockName)
        {
            var userStock = await _context.UserStocks
                .FirstOrDefaultAsync(us => us.UserCnp == userCNP && us.StockName == stockName);

            return userStock?.Quantity ?? 0;
        }

        /// <summary>
        /// Checks if the specified stock is in the user's favorites.
        /// </summary>
        /// <param name="userCNP">CNP of the user whose favorite status is to be checked.</param>
        /// <param name="stockName">Name of the stock.</param>
        /// <returns><c>true</c> if favorite; otherwise, <c>false</c>.</returns>
        public async Task<bool> GetFavoriteAsync(string userCNP, string stockName)
        {
            return await _context.FavoriteStocks
                .AnyAsync(fs => fs.UserCNP == userCNP && fs.StockName == stockName);
        }

        /// <summary>
        /// Toggles the favorite status of a stock for the user.
        /// </summary>
        /// <param name="userCNP">CNP of the user whose favorite status is to be toggled.</param>
        /// <param name="stockName">Name of the stock.</param>
        /// <param name="state"><c>true</c> to add favorite; <c>false</c> to remove.</param>
        public async Task ToggleFavoriteAsync(string userCNP, string stockName, bool state)
        {
            if (state)
            {
                if (!await _context.FavoriteStocks
                    .AnyAsync(fs => fs.UserCNP == userCNP && fs.StockName == stockName))
                {
                    var favoriteStock = new FavoriteStock
                    {
                        UserCNP = userCNP,
                        StockName = stockName
                    };
                    await _context.FavoriteStocks.AddAsync(favoriteStock);
                }
            }
            else
            {
                var favoriteStock = await _context.FavoriteStocks
                    .FirstOrDefaultAsync(fs => fs.UserCNP == userCNP && fs.StockName == stockName);

                if (favoriteStock != null)
                {
                    _context.FavoriteStocks.Remove(favoriteStock);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
