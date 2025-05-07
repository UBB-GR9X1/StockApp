using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankApi.Models;

namespace BankApi.Repositories
{
    public interface IRepository
    {
        // BaseStock methods
        Task<BaseStock> AddStockAsync(BaseStock stock, int initialPrice = 100);
        Task<List<BaseStock>> GetAllStocksAsync();
        Task<BaseStock> GetStockByNameAsync(string name);
        Task<bool> DeleteStockAsync(string name);
        Task<BaseStock> UpdateStockAsync(BaseStock stock);

        // BillSplitReport methods
        Task<List<BillSplitReport>> GetAllBillSplitReportsAsync();
        Task<BillSplitReport> GetBillSplitReportByIdAsync(int id);
        Task<BillSplitReport> AddBillSplitReportAsync(BillSplitReport report);
        Task<bool> DeleteBillSplitReportAsync(int id);
        Task<BillSplitReport> UpdateBillSplitReportAsync(BillSplitReport report);
        Task<int> GetCurrentBalanceAsync(string userCnp);
        Task<decimal> SumTransactionsSinceReportAsync(string userCnp, DateTime date);
        Task<bool> CheckHistoryOfBillSharesAsync(string userCnp);
        Task<int> GetCurrentCreditScoreAsync(string userCnp);
        Task UpdateCreditScoreAsync(string userCnp, int newCreditScore);
        Task<int> GetDaysOverdueAsync(DateTime transactionDate);
    }
} 