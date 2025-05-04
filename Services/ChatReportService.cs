namespace StockApp.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Src.Data;
    using Src.Helpers;
    using Src.Model;
    using Src.Repos;
    using StockApp.Models;
    using StockApp.Repositories;

    public class ChatReportService : IChatReportService
    {
        private readonly IChatReportRepository chatReportRepository;

        public ChatReportService(IChatReportRepository chatReportRepository)
        {
            this.chatReportRepository = chatReportRepository;
        }

        public void DoNotPunishUser(ChatReport chatReportToBeSolved)
        {
            this.chatReportRepository.DeleteChatReport(chatReportToBeSolved.Id);
        }

        public async Task<bool> PunishUser(ChatReport chatReportToBeSolved)
        {
            DatabaseConnection dbConn = new DatabaseConnection();
            UserRepository userRepo = new UserRepository(dbConn);

            User reportedUser = userRepo.GetUserByCnp(chatReportToBeSolved.ReportedUserCnp);

            int noOffenses = reportedUser.NumberOfOffenses;
            const int MINIMUM_NUMBER_OF_OFFENSES_BEFORE_PUNISHMENT_GROWS_DISTOPIANLY_ABSURD = 3;
            const int CREDIT_SCORE_DECREASE_AMOUNT_FLAT_RATE = 15;

            int amount;

            if (noOffenses >= MINIMUM_NUMBER_OF_OFFENSES_BEFORE_PUNISHMENT_GROWS_DISTOPIANLY_ABSURD)
            {
                userRepo.PenalizeUser(chatReportToBeSolved.ReportedUserCnp, noOffenses * CREDIT_SCORE_DECREASE_AMOUNT_FLAT_RATE);
                int decrease = reportedUser.CreditScore - CREDIT_SCORE_DECREASE_AMOUNT_FLAT_RATE * noOffenses;
                this.UpdateHistoryForUser(chatReportToBeSolved.ReportedUserCnp, decrease);
                amount = CREDIT_SCORE_DECREASE_AMOUNT_FLAT_RATE * noOffenses;
            }
            else
            {
                userRepo.PenalizeUser(chatReportToBeSolved.ReportedUserCnp, CREDIT_SCORE_DECREASE_AMOUNT_FLAT_RATE);
                int decrease = userRepo.GetUserByCnp(chatReportToBeSolved.ReportedUserCnp).CreditScore - CREDIT_SCORE_DECREASE_AMOUNT_FLAT_RATE;
                this.UpdateHistoryForUser(chatReportToBeSolved.ReportedUserCnp, decrease);
                amount = CREDIT_SCORE_DECREASE_AMOUNT_FLAT_RATE;
            }
            userRepo.IncrementOffensesCount(chatReportToBeSolved.ReportedUserCnp);
            this.chatReportRepository.DeleteChatReport(chatReportToBeSolved.Id);
            TipsService service = new TipsService(new TipsRepository(dbConn));
            service.GiveTipToUser(chatReportToBeSolved.ReportedUserCnp);

            int countTips = this.chatReportRepository.GetNumberOfGivenTipsForUser(chatReportToBeSolved.ReportedUserCnp);

            if (countTips % 3 == 0)
            {
                MessagesService services = new MessagesService(new MessagesRepository(dbConn));
                services.GiveMessageToUser(chatReportToBeSolved.ReportedUserCnp);
            }

            this.chatReportRepository.UpdateActivityLog(chatReportToBeSolved.ReportedUserCnp, amount);
            return true;
        }
        public async Task<bool> IsMessageOffensive(string messageToBeChecked)
        {
            bool isOffensive = await ProfanityChecker.IsMessageOffensive(messageToBeChecked);
            return isOffensive;
        }

        public void UpdateHistoryForUser(string userCNP, int newScore)
        {
            this.chatReportRepository.UpdateScoreHistoryForUser(userCNP, newScore);
        }

        public List<ChatReport> GetChatReports()
        {
            return this.chatReportRepository.GetChatReports();
        }

        public void DeleteChatReport(int id)
        {
            this.chatReportRepository.DeleteChatReport(id);
        }
    }
}
