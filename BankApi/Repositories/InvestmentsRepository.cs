using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApi.Data;
using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Repositories
{
    public class InvestmentsRepository : IInvestmentsRepository
    {
        private readonly ApiDbContext _context;

        public InvestmentsRepository(ApiDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Investment>> GetInvestmentsHistory()
        {
            return await _context.Investments.ToListAsync();
        }

        public async Task AddInvestment(Investment investment)
        {
            if (investment == null)
                throw new ArgumentNullException(nameof(investment));

            _context.Investments.Add(investment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateInvestment(int investmentId, string investorCNP, float amountReturned)
        {
            var investment = await _context.Investments.FirstOrDefaultAsync(i => i.Id == investmentId && i.InvestorCnp == investorCNP);
            if (investment == null)
                throw new Exception("Investment not found or investor CNP does not match.");

            if (investment.AmountReturned != -1)
                throw new Exception("Investment return has already been processed.");

            investment.AmountReturned = amountReturned;
            await _context.SaveChangesAsync();
        }
    }
} 