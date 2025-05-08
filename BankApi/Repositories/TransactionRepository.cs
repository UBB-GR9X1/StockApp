using BankApi.Models;
using BankApi.Data;
using Microsoft.EntityFrameworkCore;
using StockApp.Repositories;


namespace BankApi.Repositories
{
    /// <summary>
    /// Repository for managing transactions in the application using Entity Framework Core.
    /// </summary>
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApiDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionRepository"/> class.
        /// </summary>
        /// <param name="context">The database context to use for data access.</param>
        public TransactionRepository(ApiDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets the list of all transaction logs asynchronously.
        /// </summary>
        public async Task<List<TransactionLogTransaction>> getAllTransactions()
        {
                return await _context.TransactionLogTransactions.ToListAsync();
        }

        /// <summary>
        /// Retrieves a list of transactions that match the specified filter criteria.
        /// </summary>
        /// <param name="criteria">The filter criteria to apply.</param>
        /// <returns>A list of transactions matching the criteria.</returns>
        public async Task<List<TransactionLogTransaction>> GetByFilterCriteria(TransactionFilterCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            // Build the query dynamically based on the filter criteria
            IQueryable<TransactionLogTransaction> query = _context.TransactionLogTransactions;

            if (!string.IsNullOrEmpty(criteria.StockName))
            {
                query = query.Where(t => EF.Functions.Like(t.StockName, $"%{criteria.StockName}%"));
            }

            if (!string.IsNullOrEmpty(criteria.Type))
            {
                query = query.Where(t => t.Type.Equals(criteria.Type, StringComparison.OrdinalIgnoreCase));
            }

            if (criteria.MinTotalValue.HasValue)
            {
                query = query.Where(t => t.TotalValue >= criteria.MinTotalValue.Value);
            }

            if (criteria.MaxTotalValue.HasValue)
            {
                query = query.Where(t => t.TotalValue <= criteria.MaxTotalValue.Value);
            }

            if (criteria.StartDate.HasValue)
            {
                query = query.Where(t => t.Date >= criteria.StartDate.Value);
            }

            if (criteria.EndDate.HasValue)
            {
                query = query.Where(t => t.Date <= criteria.EndDate.Value);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Adds a new transaction to the repository.
        /// </summary>
        /// <param name="transaction">The transaction to add.</param>
        public async Task AddTransaction(TransactionLogTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            // Ensure the stock exists before adding the transaction
            bool stockExists = await _context.BaseStocks.AnyAsync(s => s.Name == transaction.StockName);
            if (!stockExists)
            {
                throw new InvalidOperationException($"Stock with name '{transaction.StockName}' does not exist.");
            }

            // Add the transaction to the database
            await _context.TransactionLogTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
