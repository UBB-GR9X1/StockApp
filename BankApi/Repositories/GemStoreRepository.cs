using BankApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BankApi.Repositories
{
    public class GemStoreRepository : IGemStoreRepository
    {
        private readonly AppDbContext _context;

        public GemStoreRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<string> GetCnpAsync()
        {
            // TODO: Implement actual CNP retrieval from authentication system
            throw new NotImplementedException("CNP retrieval not implemented");
        }

        public async Task<int> GetUserGemBalanceAsync(string cnp)
        {
            var gemStore = await _context.GemStores
                .FirstOrDefaultAsync(g => g.Cnp == cnp);

            return gemStore?.GemBalance ?? 0;
        }

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

        public async Task<bool> IsGuestAsync(string cnp)
        {
            var gemStore = await _context.GemStores
                .FirstOrDefaultAsync(g => g.Cnp == cnp);

            return gemStore?.IsGuest ?? true;
        }
    }
} 