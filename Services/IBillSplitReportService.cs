namespace StockApp.Services
{
    using System.Collections.Generic;
    using Src.Model;
    using StockApp.Models;

    public interface IBillSplitReportService
    {
        public List<BillSplitReport> GetBillSplitReports();
        public void CreateBillSplitReport(BillSplitReport billSplitReport);
        public int GetDaysOverdue(BillSplitReport billSplitReport);
        public void SolveBillSplitReport(BillSplitReport billSplitReportToBeSolved);
        public void DeleteBillSplitReport(BillSplitReport billSplitReportToBeSolved);
        public User GetUserByCNP(string cNP);
    }
}
