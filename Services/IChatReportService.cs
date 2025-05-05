namespace StockApp.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Src.Model;

    public interface IChatReportService
    {
        void DoNotPunishUser(ChatReport chatReportToBeSolved);

        Task<bool> PunishUser(ChatReport chatReportToBeSolved);

        Task<bool> IsMessageOffensive(string messageToBeChecked);

        void UpdateHistoryForUser(string userCNP, int newScore);

        List<ChatReport> GetChatReports();
    }
}
