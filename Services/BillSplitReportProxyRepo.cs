using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Src.Model;
using StockApp.Repositories;

namespace StockApp.Services
{
    public class BillSplitReportProxyRepo : IBillSplitReportRepository
    {
        private readonly BillSplitReportApiService _apiService;

        public BillSplitReportProxyRepo(BillSplitReportApiService apiService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }

        public async Task<List<BillSplitReport>> GetBillSplitReportsAsync()
        {
            try
            {
                return await _apiService.GetAllReportsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving bill split reports", ex);
            }
        }

        public async Task<bool> DeleteBillSplitReportAsync(int id)
        {
            try
            {
                return await _apiService.DeleteReportAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting bill split report: {ex.Message}", ex);
            }
        }

        public async Task<BillSplitReport> CreateBillSplitReportAsync(BillSplitReport billSplitReport)
        {
            try
            {
                return await _apiService.CreateReportAsync(billSplitReport);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating bill split report: {ex.Message}", ex);
            }
        }

        public async Task<bool> CheckLogsForSimilarPaymentsAsync(BillSplitReport billSplitReport)
        {
            // This would be implemented server-side, for now return a default value
            await Task.Delay(1); // Simulate async operation
            return false;
        }

        public async Task<int> GetCurrentBalanceAsync(BillSplitReport billSplitReport)
        {
            try
            {
                return await _apiService.GetCurrentBalanceAsync(billSplitReport.ReportedUserCnp);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting current balance: {ex.Message}", ex);
            }
        }

        public async Task<decimal> SumTransactionsSinceReportAsync(BillSplitReport billSplitReport)
        {
            // This would be implemented server-side, for now return a default value
            await Task.Delay(1); // Simulate async operation
            return 0;
        }

        public async Task<bool> CheckHistoryOfBillSharesAsync(BillSplitReport billSplitReport)
        {
            // This would be implemented server-side, for now return a default value
            await Task.Delay(1); // Simulate async operation
            return true;
        }

        public async Task<bool> CheckFrequentTransfersAsync(BillSplitReport billSplitReport)
        {
            // This would be implemented server-side, for now return a default value
            await Task.Delay(1); // Simulate async operation
            return false;
        }

        public async Task<int> GetNumberOfOffensesAsync(BillSplitReport billSplitReport)
        {
            // This would be implemented server-side, for now return a default value
            await Task.Delay(1); // Simulate async operation
            return 0;
        }

        public async Task<int> GetCurrentCreditScoreAsync(BillSplitReport billSplitReport)
        {
            try
            {
                return await _apiService.GetCreditScoreAsync(billSplitReport.ReportedUserCnp);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting credit score: {ex.Message}", ex);
            }
        }

        public async Task UpdateCreditScoreAsync(BillSplitReport billSplitReport, int newCreditScore)
        {
            try
            {
                await _apiService.UpdateCreditScoreAsync(billSplitReport.ReportedUserCnp, newCreditScore);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating credit score: {ex.Message}", ex);
            }
        }

        public async Task UpdateCreditScoreHistoryAsync(BillSplitReport billSplitReport, int newCreditScore)
        {
            // This would be implemented server-side
            await Task.CompletedTask;
        }

        public async Task IncrementNoOfBillSharesPaidAsync(BillSplitReport billSplitReport)
        {
            // This would be implemented server-side
            await Task.CompletedTask;
        }

        public async Task<int> GetDaysOverdueAsync(BillSplitReport billSplitReport)
        {
            return (DateTime.Now - billSplitReport.DateOfTransaction).Days;
        }
    }
} 