namespace StockApp.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Src.Model;

    public interface IBillSplitReportRepository
    {
        Task<List<BillSplitReport>> GetBillSplitReportsAsync();

        Task<bool> DeleteBillSplitReportAsync(int id);

        Task<BillSplitReport> CreateBillSplitReportAsync(BillSplitReport billSplitReport);

        Task<bool> CheckLogsForSimilarPaymentsAsync(BillSplitReport billSplitReport);

        Task<int> GetCurrentBalanceAsync(BillSplitReport billSplitReport);

        Task<decimal> SumTransactionsSinceReportAsync(BillSplitReport billSplitReport);

        Task<bool> CheckHistoryOfBillSharesAsync(BillSplitReport billSplitReport);

        Task<bool> CheckFrequentTransfersAsync(BillSplitReport billSplitReport);

        Task<int> GetNumberOfOffensesAsync(BillSplitReport billSplitReport);

        Task<int> GetCurrentCreditScoreAsync(BillSplitReport billSplitReport);

        Task UpdateCreditScoreAsync(BillSplitReport billSplitReport, int newCreditScore);

        Task UpdateCreditScoreHistoryAsync(BillSplitReport billSplitReport, int newCreditScore);

        Task IncrementNoOfBillSharesPaidAsync(BillSplitReport billSplitReport);

        Task<int> GetDaysOverdueAsync(BillSplitReport billSplitReport);
    }
}
