using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApi.Data;
using BankApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BankApi.Repositories
{
    public class LoanRequestRepository : ILoanRequestRepository
    {
        private readonly ApiDbContext _context;
        private readonly ILogger<LoanRequestRepository> _logger;

        public LoanRequestRepository(ApiDbContext context, ILogger<LoanRequestRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<LoanRequest>> GetLoanRequestsAsync()
        {
            try
            {
                return await _context.LoanRequests
                    .OrderByDescending(lr => lr.ApplicationDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving loan requests");
                throw;
            }
        }

        public async Task<List<LoanRequest>> GetUnsolvedLoanRequestsAsync()
        {
            try
            {
                return await _context.LoanRequests
                    .Where(lr => lr.Status != "Solved" || lr.Status == null)
                    .OrderByDescending(lr => lr.ApplicationDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving unsolved loan requests");
                throw;
            }
        }

        public async Task SolveLoanRequestAsync(int loanRequestId)
        {
            if (loanRequestId <= 0)
                throw new ArgumentException("Invalid loan request ID", nameof(loanRequestId));

            try
            {
                var request = await _context.LoanRequests.FindAsync(loanRequestId);
                if (request == null)
                {
                    throw new KeyNotFoundException($"Loan request with ID {loanRequestId} not found");
                }

                request.Status = "Solved";
                await _context.SaveChangesAsync();

                _logger.LogInformation("Loan request {LoanRequestId} marked as solved", loanRequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error solving loan request {LoanRequestId}", loanRequestId);
                throw;
            }
        }

        public async Task DeleteLoanRequestAsync(int loanRequestId)
        {
            if (loanRequestId <= 0)
                throw new ArgumentException("Invalid loan request ID", nameof(loanRequestId));

            try
            {
                var request = await _context.LoanRequests.FindAsync(loanRequestId);
                if (request == null)
                {
                    throw new KeyNotFoundException($"Loan request with ID {loanRequestId} not found");
                }

                _context.LoanRequests.Remove(request);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Loan request {LoanRequestId} deleted", loanRequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting loan request {LoanRequestId}", loanRequestId);
                throw;
            }
        }
    }
}
