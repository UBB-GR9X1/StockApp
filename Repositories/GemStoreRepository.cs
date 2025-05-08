using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockApp.Database;
using StockApp.Models;

namespace StockApp.Repositories
{
    /// <summary>
    /// Repository for retrieving and updating user gem balances and CNP values.
    /// </summary>
    public class GemStoreRepository : IGemStoreRepository
    {
        private readonly AppDbContext _context;

        public GemStoreRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Retrieves the CNP for the current user.
        /// </summary>
        /// <returns>The CNP string, or empty if not found.</returns>
        public async Task<string> GetCnpAsync()
        {
            // Implementation depends on your authentication system
            // This is a placeholder - you'll need to implement based on your auth system
            throw new NotImplementedException("GetCnpAsync needs to be implemented based on your authentication system");
        }

        /// <summary>
        /// Retrieves the current gem balance for the specified user.
        /// </summary>
        /// <param name="cnp">User identifier (CNP).</param>
        /// <returns>User's gem balance as an integer.</returns>
        public async Task<int> GetUserGemBalanceAsync(string cnp)
        {
            var gemStore = await _context.GemStores
                .FirstOrDefaultAsync(g => g.Cnp == cnp);

            return gemStore?.GemBalance ?? 0;
        }

        /// <summary>
        /// Updates the gem balance for a given user.
        /// </summary>
        /// <param name="cnp">User identifier (CNP).</param>
        /// <param name="newBalance">New gem balance to set.</param>
        public async Task UpdateUserGemBalanceAsync(string cnp, int newBalance)
        {
            var gemStore = await _context.GemStores
                .FirstOrDefaultAsync(g => g.Cnp == cnp);

            if (gemStore == null)
            {
                gemStore = new GemStore
                {
                    Cnp = cnp,
                    GemBalance = newBalance,
                    IsGuest = false,
                    LastUpdated = DateTime.UtcNow
                };
                _context.GemStores.Add(gemStore);
            }
            else
            {
                gemStore.GemBalance = newBalance;
                gemStore.LastUpdated = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Determines if the specified user is considered a guest (no record in the database).
        /// </summary>
        /// <param name="cnp">User identifier (CNP).</param>
        /// <returns><c>true</c> if no user record exists; otherwise, <c>false</c>.</returns>
        public async Task<bool> IsGuestAsync(string cnp)
        {
            var gemStore = await _context.GemStores
                .FirstOrDefaultAsync(g => g.Cnp == cnp);

            return gemStore?.IsGuest ?? true;
        }
    }
}
