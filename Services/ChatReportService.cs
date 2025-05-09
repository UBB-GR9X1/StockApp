using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Src.Helpers;
using Src.Model;
using StockApp.Models;
using StockApp.Repositories;
using StockApp.Services;

public class ChatReportService(IChatReportRepository chatReportRepository, IUserRepository userRepository, ITipsService tipsService, IMessagesService messageService) : IChatReportService
{
    private readonly IChatReportRepository chatReportRepository = chatReportRepository ?? throw new System.ArgumentNullException(nameof(chatReportRepository));
    private readonly IUserRepository userRepository = userRepository ?? throw new System.ArgumentNullException(nameof(userRepository));
    private readonly ITipsService tipsService = tipsService ?? throw new System.ArgumentNullException(nameof(tipsService));
    private readonly IMessagesService messageService = messageService ?? throw new System.ArgumentNullException(nameof(messageService));

    public async Task DoNotPunishUser(ChatReport chatReportToBeSolved)
    {
        await this.chatReportRepository.DeleteChatReportAsync(chatReportToBeSolved.Id);
    }

    public async Task PunishUser(ChatReport chatReportToBeSolved)
    {
        User reportedUser = await userRepository.GetByCnpAsync(chatReportToBeSolved.ReportedUserCnp) ?? throw new Exception("User not found");

        int noOffenses = reportedUser.NumberOfOffenses;
        const int MINIMUM = 3;
        const int FLAT_PENALTY = 15;

        int amount = noOffenses >= MINIMUM
            ? FLAT_PENALTY * noOffenses
            : FLAT_PENALTY;

        reportedUser.GemBalance -= amount;
        await this.userRepository.UpdateAsync(reportedUser.Id, reportedUser);

        int updatedScore = reportedUser.CreditScore - amount;
        await this.chatReportRepository.UpdateScoreHistoryForUserAsync(chatReportToBeSolved.ReportedUserCnp, updatedScore);

        reportedUser.NumberOfOffenses++;
        await this.userRepository.UpdateAsync(reportedUser.Id, reportedUser);
        await this.chatReportRepository.DeleteChatReportAsync(chatReportToBeSolved.Id);

        tipsService.GiveTipToUser(chatReportToBeSolved.ReportedUserCnp);

        int tipCount = await this.chatReportRepository.GetNumberOfGivenTipsForUserAsync(chatReportToBeSolved.ReportedUserCnp);
        if (tipCount % 3 == 0)
        {
            messageService.GiveMessageToUser(chatReportToBeSolved.ReportedUserCnp);
        }

        await this.chatReportRepository.UpdateActivityLogAsync(chatReportToBeSolved.ReportedUserCnp, amount);
    }

    public async Task<bool> IsMessageOffensive(string messageToBeChecked)
    {
        return await ProfanityChecker.IsMessageOffensive(messageToBeChecked);
    }

    public async Task<List<ChatReport>> GetChatReports()
    {
        return await this.chatReportRepository.GetAllChatReportsAsync();
    }

    public async Task DeleteChatReport(int id)
    {
        await this.chatReportRepository.DeleteChatReportAsync(id);
    }

    public async Task UpdateHistoryForUser(string userCNP, int newScore)
    {
        await this.chatReportRepository.UpdateScoreHistoryForUserAsync(userCNP, newScore);
    }

}
