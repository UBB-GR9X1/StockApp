using System.Collections.Generic;
using System.Threading.Tasks;
using Src.Model;

namespace StockApp.Services
{
    public interface IBillSplitReportApiService
    {
        Task<List<BillSplitReport>> GetAllReportsAsync();
        Task<BillSplitReport> GetReportByIdAsync(int id);
        Task<BillSplitReport> CreateReportAsync(BillSplitReport report);
        Task<bool> UpdateReportAsync(BillSplitReport report);
        Task<bool> DeleteReportAsync(int id);
        Task<int> GetCurrentBalanceAsync(string userCnp);
        Task<int> GetCreditScoreAsync(string userCnp);
        Task UpdateCreditScoreAsync(string userCnp, int newCreditScore);
    }
} 