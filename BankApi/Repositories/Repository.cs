using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankApi.Data;
using BankApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BankApi.Repositories
{
    public class Repository : IRepository
    {
        private readonly ApiDbContext _dbContext;
        private readonly ILogger<Repository> _logger;

        public Repository(ApiDbContext dbContext, ILogger<Repository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<BaseStock> AddStockAsync(BaseStock stock, int initialPrice = 100)
        {
            if (stock == null)
            {
                throw new ArgumentNullException(nameof(stock));
            }

            try
            {
                // Check if stock with the same name already exists
                var existingStock = await _dbContext.BaseStocks.FirstOrDefaultAsync(s => s.Name == stock.Name);
                
                if (existingStock != null)
                {
                    _logger.LogWarning("Attempted to add duplicate stock: {StockName}", stock.Name);
                    throw new InvalidOperationException($"Stock with name '{stock.Name}' already exists.");
                }

                // Add the stock to the context
                await _dbContext.BaseStocks.AddAsync(stock);
                
                // Save changes to the database
                await _dbContext.SaveChangesAsync();
                
                _logger.LogInformation("Added new stock: {StockName}", stock.Name);
                return stock;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to add stock {StockName} to database", stock.Name);
                throw new InvalidOperationException("Failed to add stock to the database.", ex);
            }
        }

        public async Task<BaseStock> GetStockByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Stock name cannot be null or empty.", nameof(name));
            }

            var stock = await _dbContext.BaseStocks.FirstOrDefaultAsync(s => s.Name == name);
            
            if (stock == null)
            {
                _logger.LogWarning("Stock not found: {StockName}", name);
                throw new KeyNotFoundException($"Stock with name '{name}' not found.");
            }

            return stock;
        }

        public async Task<List<BaseStock>> GetAllStocksAsync()
        {
            return await _dbContext.BaseStocks.ToListAsync();
        }

        public async Task<BaseStock> UpdateStockAsync(BaseStock stock)
        {
            if (stock == null)
            {
                throw new ArgumentNullException(nameof(stock));
            }

            try
            {
                var existingStock = await _dbContext.BaseStocks.FirstOrDefaultAsync(s => s.Name == stock.Name);
                
                if (existingStock == null)
                {
                    _logger.LogWarning("Attempted to update non-existent stock: {StockName}", stock.Name);
                    throw new KeyNotFoundException($"Stock with name '{stock.Name}' not found.");
                }

                // Update properties
                existingStock.Symbol = stock.Symbol;
                existingStock.AuthorCNP = stock.AuthorCNP;

                // Update the entity
                _dbContext.BaseStocks.Update(existingStock);
                
                // Save changes
                await _dbContext.SaveChangesAsync();
                
                _logger.LogInformation("Updated stock: {StockName}", stock.Name);
                return existingStock;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to update stock {StockName}", stock.Name);
                throw new InvalidOperationException($"Failed to update stock '{stock.Name}'.", ex);
            }
        }

        public async Task<bool> DeleteStockAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Stock name cannot be null or empty.", nameof(name));
            }

            try
            {
                var stock = await _dbContext.BaseStocks.FirstOrDefaultAsync(s => s.Name == name);
                
                if (stock == null)
                {
                    _logger.LogWarning("Attempted to delete non-existent stock: {StockName}", name);
                    return false;
                }

                _dbContext.BaseStocks.Remove(stock);
                await _dbContext.SaveChangesAsync();
                
                _logger.LogInformation("Deleted stock: {StockName}", name);
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to delete stock {StockName}", name);
                throw new InvalidOperationException($"Failed to delete stock '{name}'.", ex);
            }
        }

        public async Task<List<BillSplitReport>> GetAllBillSplitReportsAsync()
        {
            return await _dbContext.BillSplitReports.ToListAsync();
        }

        public async Task<BillSplitReport> GetBillSplitReportByIdAsync(int id)
        {
            var report = await _dbContext.BillSplitReports.FindAsync(id);
            
            if (report == null)
            {
                _logger.LogWarning("BillSplitReport not found: {ReportId}", id);
                throw new KeyNotFoundException($"BillSplitReport with ID '{id}' not found.");
            }
            
            return report;
        }

        public async Task<BillSplitReport> AddBillSplitReportAsync(BillSplitReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }

            try
            {
                // Add the report to the context
                await _dbContext.BillSplitReports.AddAsync(report);
                
                // Save changes to the database
                await _dbContext.SaveChangesAsync();
                
                _logger.LogInformation("Added new bill split report with ID: {ReportId}", report.Id);
                return report;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to add bill split report to database");
                throw new InvalidOperationException("Failed to add bill split report to the database.", ex);
            }
        }

        public async Task<bool> DeleteBillSplitReportAsync(int id)
        {
            try
            {
                var report = await _dbContext.BillSplitReports.FindAsync(id);
                
                if (report == null)
                {
                    _logger.LogWarning("Attempted to delete non-existent bill split report: {ReportId}", id);
                    return false;
                }

                _dbContext.BillSplitReports.Remove(report);
                await _dbContext.SaveChangesAsync();
                
                _logger.LogInformation("Deleted bill split report: {ReportId}", id);
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to delete bill split report {ReportId}", id);
                throw new InvalidOperationException($"Failed to delete bill split report '{id}'.", ex);
            }
        }

        public async Task<BillSplitReport> UpdateBillSplitReportAsync(BillSplitReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }

            try
            {
                var existingReport = await _dbContext.BillSplitReports.FindAsync(report.Id);
                
                if (existingReport == null)
                {
                    _logger.LogWarning("Attempted to update non-existent bill split report: {ReportId}", report.Id);
                    throw new KeyNotFoundException($"Bill split report with ID '{report.Id}' not found.");
                }

                // Update properties
                existingReport.ReportedUserCnp = report.ReportedUserCnp;
                existingReport.ReportingUserCnp = report.ReportingUserCnp;
                existingReport.DateOfTransaction = report.DateOfTransaction;
                existingReport.BillShare = report.BillShare;

                // Update the entity
                _dbContext.BillSplitReports.Update(existingReport);
                
                // Save changes
                await _dbContext.SaveChangesAsync();
                
                _logger.LogInformation("Updated bill split report: {ReportId}", report.Id);
                return existingReport;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to update bill split report {ReportId}", report.Id);
                throw new InvalidOperationException($"Failed to update bill split report '{report.Id}'.", ex);
            }
        }

        public async Task<int> GetCurrentBalanceAsync(string userCnp)
        {
            // This would typically query a Users table, but since we don't have the complete schema
            // we'll implement a placeholder for now
            _logger.LogInformation("Getting current balance for user: {UserCnp}", userCnp);
            
            // Placeholder implementation
            return 1000;
        }

        public async Task<decimal> SumTransactionsSinceReportAsync(string userCnp, DateTime date)
        {
            _logger.LogInformation("Summing transactions since {Date} for user: {UserCnp}", date, userCnp);
            
            // Placeholder implementation
            return 500;
        }

        public async Task<bool> CheckHistoryOfBillSharesAsync(string userCnp)
        {
            _logger.LogInformation("Checking bill share history for user: {UserCnp}", userCnp);
            
            // Placeholder implementation
            return true;
        }

        public async Task<int> GetCurrentCreditScoreAsync(string userCnp)
        {
            _logger.LogInformation("Getting credit score for user: {UserCnp}", userCnp);
            
            // Placeholder implementation
            return 700;
        }

        public async Task UpdateCreditScoreAsync(string userCnp, int newCreditScore)
        {
            _logger.LogInformation("Updating credit score to {Score} for user: {UserCnp}", newCreditScore, userCnp);
            
            // Placeholder implementation - would update in a Users table
            await Task.CompletedTask;
        }

        public async Task<int> GetDaysOverdueAsync(DateTime transactionDate)
        {
            var daysOverdue = (DateTime.Now - transactionDate).Days;
            return await Task.FromResult(daysOverdue);
        }
    }
} 