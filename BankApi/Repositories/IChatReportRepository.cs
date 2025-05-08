﻿namespace StockApp.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BankApi.Models;

    public interface IChatReportRepository
    {
        Task<List<ChatReport>> GetAllChatReportsAsync();

        Task<ChatReport?> GetChatReportByIdAsync(int id);

        Task<bool> AddChatReportAsync(ChatReport report);

        Task<bool> DeleteChatReportAsync(int id);

        Task<int> GetNumberOfGivenTipsForUserAsync(string userCnp);

        Task UpdateActivityLogAsync(string userCnp, int amount);

        Task UpdateScoreHistoryForUserAsync(string userCnp, int newScore);
    }
}
