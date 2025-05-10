namespace StockApp.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;

    public interface IHistoryService
    {
        Task<List<CreditScoreHistory>> GetAllHistoryAsync();
        Task<CreditScoreHistory> GetHistoryByIdAsync(int id);
        Task AddHistoryAsync(CreditScoreHistory history);
        Task UpdateHistoryAsync(CreditScoreHistory history);
        Task DeleteHistoryAsync(int id);
        Task<List<CreditScoreHistory>> GetHistoryForUserAsync(string userCnp);
        Task<List<CreditScoreHistory>> GetHistoryWeeklyAsync(string userCnp);
        Task<List<CreditScoreHistory>> GetHistoryMonthlyAsync(string userCnp);
        Task<List<CreditScoreHistory>> GetHistoryYearlyAsync(string userCnp);
    }
}
