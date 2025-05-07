using System.Collections.Generic;
using System.Threading.Tasks;
using Src.Model;
using StockApp.Models;

namespace StockApp.Services
{
    public interface IBillSplitReportService
    {
        Task<List<BillSplitReport>> GetBillSplitReportsAsync();
        Task<BillSplitReport> CreateBillSplitReportAsync(BillSplitReport billSplitReport);
        Task<int> GetDaysOverdueAsync(BillSplitReport billSplitReport);
        Task SolveBillSplitReportAsync(BillSplitReport billSplitReportToBeSolved);
        Task<bool> DeleteBillSplitReportAsync(BillSplitReport billSplitReportToBeSolved);
        Task<User> GetUserByCNPAsync(string cnp);
    }
}
