using BankApi.Models;

namespace BankApi.Repositories
{
    public interface IHistoryRepository
    {
        Task<IEnumerable<CreditScoreHistory>> GetAllHistoryAsync();
        Task<CreditScoreHistory?> GetHistoryByIdAsync(int id);
        Task<CreditScoreHistory> AddHistoryAsync(CreditScoreHistory history);
        Task<CreditScoreHistory> UpdateHistoryAsync(CreditScoreHistory history);
        Task DeleteHistoryAsync(int id);
        Task<IEnumerable<CreditScoreHistory>> GetHistoryForUserAsync(string userCnp);
        Task<IEnumerable<CreditScoreHistory>> GetHistoryWeeklyAsync(string userCnp);
        Task<IEnumerable<CreditScoreHistory>> GetHistoryMonthlyAsync(string userCnp);
        Task<IEnumerable<CreditScoreHistory>> GetHistoryYearlyAsync(string userCnp);
    }
} 