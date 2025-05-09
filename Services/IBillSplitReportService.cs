using System.Collections.Generic;
using System.Threading.Tasks;
using Src.Model;

namespace StockApp.Services
{
    public interface IBillSplitReportService
    {
        Task<List<BillSplitReport>> GetBillSplitReportsAsync();

        Task<BillSplitReport> CreateBillSplitReportAsync(BillSplitReport billSplitReport);

        Task<int> GetDaysOverdueAsync(BillSplitReport billSplitReport);

        Task SolveBillSplitReportAsync(BillSplitReport billSplitReportToBeSolved);

        Task DeleteBillSplitReportAsync(BillSplitReport billSplitReportToBeSolved);

        Task<BillSplitReport> UpdateBillSplitReportAsync(BillSplitReport billSplitReport);
    }
}
