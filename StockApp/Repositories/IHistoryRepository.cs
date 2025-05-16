using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace StockApp.Repositories
{
    public interface IHistoryRepository
    {
        Task AddHistoryAsync(CreditScoreHistory history);

        Task DeleteHistoryAsync(int id);

        Task<List<CreditScoreHistory>> GetAllHistoryAsync();

        Task<CreditScoreHistory> GetHistoryByIdAsync(int id);

        Task<List<CreditScoreHistory>> GetHistoryForUserAsync(string userCnp);

        Task<List<CreditScoreHistory>> GetHistoryMonthlyAsync(string userCnp);

        Task<List<CreditScoreHistory>> GetHistoryWeeklyAsync(string userCnp);

        Task<List<CreditScoreHistory>> GetHistoryYearlyAsync(string userCnp);

        Task UpdateHistoryAsync(CreditScoreHistory history);
    }
}