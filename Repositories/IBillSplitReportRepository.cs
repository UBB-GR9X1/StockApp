namespace StockApp.Repositories
{
    using System.Collections.Generic;
    using Src.Model;

    public interface IBillSplitReportRepository
    {
        List<BillSplitReport> GetBillSplitReports();

        void DeleteBillSplitReport(int id);

        void CreateBillSplitReport(BillSplitReport billSplitReport);

        bool CheckLogsForSimilarPayments(BillSplitReport billSplitReport);

        int GetCurrentBalance(BillSplitReport billSplitReport);

        decimal SumTransactionsSinceReport(BillSplitReport billSplitReport);

        bool CheckHistoryOfBillShares(BillSplitReport billSplitReport);

        bool CheckFrequentTransfers(BillSplitReport billSplitReport);

        int GetNumberOfOffenses(BillSplitReport billSplitReport);

        int GetCurrentCreditScore(BillSplitReport billSplitReport);

        void UpdateCreditScore(BillSplitReport billSplitReport, int newCreditScore);

        void UpdateCreditScoreHistory(BillSplitReport billSplitReport, int newCreditScore);

        void IncrementNoOfBillSharesPaid(BillSplitReport billSplitReport);

        int GetDaysOverdue(BillSplitReport billSplitReport);
    }
}
