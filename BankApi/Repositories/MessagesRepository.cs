namespace BankApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using BankApi.Data;
    using BankApi.Models;
    using Microsoft.EntityFrameworkCore;

    public class MessagesRepository : IMessagesRepository
    {
        private readonly ApiDbContext _context;

        public MessagesRepository(ApiDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Message>> GetMessagesForGivenUserAsync(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
            {
                throw new ArgumentException("User CNP cannot be empty", nameof(userCnp));
            }

            return await _context.GivenTips
                .Include(gt => gt.User)
                .Where(gt => gt.User.CNP == userCnp)
                .Join(
                    _context.Tips, // Inner sequence
                    gt => gt.Tip.Id, // Outer key selector
                    m => m.Id, // Inner key selector
                    (gt, m) => new Message // Result selector
                    {
                        Id = m.Id,
                        Type = m.CreditScoreBracket,
                        MessageContent = m.TipText
                    }
                )
                .ToListAsync();
        }

        public async Task GiveUserRandomMessageAsync(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
            {
                throw new ArgumentException("User CNP cannot be empty", nameof(userCnp));
            }

            var randomMessage = await _context.Tips
                .Where(m => m.Type == "Punishment")
                .OrderBy(m => Guid.NewGuid())
                .FirstOrDefaultAsync();

            if (randomMessage == null)
            {
                throw new Exception("No congratulatory messages found");
            }

            var givenTip = new GivenTip
            {
                User = await _context.Users
                    .FirstOrDefaultAsync(u => u.CNP == userCnp) ?? throw new Exception("User not found"),
                Date = DateTime.UtcNow,
                Tip = randomMessage,
            };

            _context.GivenTips.Add(givenTip);
            await _context.SaveChangesAsync();
        }

        public async Task GiveUserRandomRoastMessageAsync(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
            {
                throw new ArgumentException("User CNP cannot be empty", nameof(userCnp));
            }

            var randomMessage = await _context.Tips
                .Where(m => m.Type == "Roast")
                .OrderBy(m => Guid.NewGuid())
                .FirstOrDefaultAsync();

            if (randomMessage == null)
            {
                throw new Exception("No roast messages found");
            }

            var givenTip = new GivenTip
            {
                User = await _context.Users
                    .FirstOrDefaultAsync(u => u.CNP == userCnp) ?? throw new Exception("User not found"),
                Date = DateTime.UtcNow,
                Tip = randomMessage,
            };

            _context.GivenTips.Add(givenTip);
            await _context.SaveChangesAsync();
        }
    }
}