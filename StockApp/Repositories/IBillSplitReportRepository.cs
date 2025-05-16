using System.Collections.Generic;
using System.Threading.Tasks;
using Src.Model;

namespace StockApp.Repositories
{
    public interface IBillSplitReportRepository
    {
        Task<List<BillSplitReport>> GetAllReportsAsync();
        Task<BillSplitReport> GetReportByIdAsync(int id);
        Task<BillSplitReport> AddReportAsync(BillSplitReport report);
        Task<BillSplitReport> UpdateReportAsync(BillSplitReport report);
        Task<bool> DeleteReportAsync(int id);

        Task IncrementBillSharesPaidAsync(string userCnp);
    }
}
