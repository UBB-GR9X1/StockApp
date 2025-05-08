namespace StockApp.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Src.Model;

    public interface IBillSplitReportService
    {
        Task<List<BillSplitReport>> GetBillSplitReportsAsync();

        Task CreateBillSplitReportAsync(BillSplitReport billSplitReport);

        Task<int> GetDaysOverdueAsync(BillSplitReport billSplitReport);

        Task SolveBillSplitReportAsync(BillSplitReport billSplitReportToBeSolved);

        Task DeleteBillSplitReportAsync(BillSplitReport billSplitReportToBeSolved);

    }
}