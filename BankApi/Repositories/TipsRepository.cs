using BankApi.Data;
using BankApi.Repositories;
using Microsoft.EntityFrameworkCore;
using BankApi.Data;
using BankApi.Models;

namespace BankApi.Repositories
{
    public class TipsRepository : ITipsRepository
    {
        private readonly ApiDbContext _context;

        public TipsRepository(ApiDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Fetch all tips for a given user
        public async Task<List<Tip>> GetTipsForUserAsync(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
                throw new ArgumentException("User CNP must be provided.");

            return await _context.GivenTips
                .Include(gt => gt.Tip)
                .Where(gt => gt.UserCnp == userCnp)
                .Select(gt => gt.Tip)
                .ToListAsync();
        }

        // Provide a tip to a user based on credit score bracket
        public async Task<GivenTip> GiveTipToUserAsync(string userCnp, string creditScoreBracket)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
                throw new ArgumentException("User CNP must be provided.");

            if (string.IsNullOrWhiteSpace(creditScoreBracket))
                throw new ArgumentException("Credit score bracket must be provided.");

            var randomTip = await _context.Tips
                .Where(t => t.CreditScoreBracket == creditScoreBracket)
                .OrderBy(t => Guid.NewGuid())
                .FirstOrDefaultAsync();

            if (randomTip == null)
                throw new InvalidOperationException($"No tip found for bracket '{creditScoreBracket}'");

            var givenTip = new GivenTip
            {
                UserCnp = userCnp,
                TipId = randomTip.Id,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Tip = randomTip
            };

            _context.GivenTips.Add(givenTip);
            await _context.SaveChangesAsync();

            return givenTip;
        }

        // Provide low-credit bracket tip
        public Task<GivenTip> GiveLowBracketTipAsync(string userCnp) =>
            GiveTipToUserAsync(userCnp, "Low-credit");

        // Provide medium-credit bracket tip
        public Task<GivenTip> GiveMediumBracketTipAsync(string userCnp) =>
            GiveTipToUserAsync(userCnp, "Medium-credit");

        // Provide high-credit bracket tip
        public Task<GivenTip> GiveHighBracketTipAsync(string userCnp) =>
            GiveTipToUserAsync(userCnp, "High-credit");
    }
}
