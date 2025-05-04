namespace StockApp.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Src.Model;

    public interface IChatReportService
    {
        public void DoNotPunishUser(ChatReport chatReportToBeSolved);
        public Task<bool> PunishUser(ChatReport chatReportToBeSolved);
        public Task<bool> IsMessageOffensive(string messageToBeChecked);
        public void UpdateHistoryForUser(string userCNP, int newScore);
        public List<ChatReport> GetChatReports();
    }
}
