namespace StockApp.Services
{
    using System.Collections.Generic;
    using Src.Model;
    using StockApp.Models;

    public interface IBillSplitReportService
    {
        List<BillSplitReport> GetBillSplitReports();

        void CreateBillSplitReport(BillSplitReport billSplitReport);

        int GetDaysOverdue(BillSplitReport billSplitReport);

        void SolveBillSplitReport(BillSplitReport billSplitReportToBeSolved);

        void DeleteBillSplitReport(BillSplitReport billSplitReportToBeSolved);

        User GetUserByCNP(string cNP);
    }
}
