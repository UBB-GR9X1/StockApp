namespace StockApp.Views.Components
{
    using Common.Models;
    using Common.Services;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using System;

    public sealed partial class ChatReportComponent : Page
    {
        private readonly IChatReportService chatReportService;
        private readonly IProfanityChecker profanityChecker;

        public event EventHandler? ReportSolved;

        public string ReportedUserCNP { get; set; } = string.Empty;

        public string ReportedMessage { get; set; } = string.Empty;

        public int ReportId { get; set; }

        public ChatReportComponent(IChatReportService chatReportService, IProfanityChecker profanityChecker)
        {
            this.InitializeComponent();
            this.chatReportService = chatReportService;
            this.profanityChecker = profanityChecker;
        }

        private async void PunishReportedUser(object sender, RoutedEventArgs e)
        {
            ChatReport chatReport = new()
            {
                Id = this.ReportId,
                ReportedUserCnp = this.ReportedUserCNP,
                ReportedMessage = this.ReportedMessage,
            };

            await this.chatReportService.AddChatReportAsync(chatReport);
            this.ReportSolved?.Invoke(this, EventArgs.Empty);
        }

        private async void DoNotPunishReportedUser(object sender, RoutedEventArgs e)
        {
            ChatReport chatReport = new()
            {
                Id = this.ReportId,
                ReportedUserCnp = this.ReportedUserCNP,
                ReportedMessage = this.ReportedMessage,
            };
            await this.chatReportService.AddChatReportAsync(chatReport);
            this.ReportSolved?.Invoke(this, EventArgs.Empty);
        }

        public async void SetReportData(int id, string reportedUserCnp, string reportedMessage)
        {
            this.ReportId = id;
            this.ReportedUserCNP = reportedUserCnp;
            this.ReportedMessage = reportedMessage;

            bool apiSuggestion = await this.profanityChecker.IsMessageOffensive(reportedMessage);

            this.IdTextBlock.Text = $"Report ID: {id}";
            this.ReportedUserCNPTextBlock.Text = $"Reported user's CNP: {reportedUserCnp}";
            this.ReportedMessageTextBlock.Text = $"Message: {reportedMessage}";
            this.ApiSuggestionTextBlock.Text = apiSuggestion ? "The software marked this message as offensive" : "The software marked this message as inoffensive";
        }
    }
}
