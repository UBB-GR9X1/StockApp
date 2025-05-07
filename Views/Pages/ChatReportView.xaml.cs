namespace StockApp.Views.Pages
{
    using System;
    using System.Collections.Generic;
    using Microsoft.UI.Xaml.Controls;
    using Src.Model;
    using StockApp.Services;
    using StockApp.Views.Components;

    public sealed partial class ChatReportView : Page
    {
        private readonly Func<ChatReportComponent> componentFactory;
        private readonly IChatReportService chatReportService;

        public ChatReportView(Func<ChatReportComponent> componentFactory, IChatReportService chatReportService)
        {
            this.componentFactory = componentFactory;
            this.chatReportService = chatReportService;
            this.InitializeComponent();
            this.LoadChatReports();
        }

        private void LoadChatReports()
        {
            this.ChatReportsContainer.Items.Clear();

            try
            {
                List<ChatReport> chatReports = this.chatReportService.GetChatReports();
                foreach (var report in chatReports)
                {
                    ChatReportComponent reportComponent = this.componentFactory();
                    reportComponent.SetReportData(report.Id, report.ReportedUserCnp, report.ReportedMessage);

                    reportComponent.ReportSolved += this.OnReportSolved;

                    this.ChatReportsContainer.Items.Add(reportComponent);
                }
            }
            catch (Exception)
            {
                this.ChatReportsContainer.Items.Add("There are no chat reports that need solving.");
            }
        }

        private void OnReportSolved(object sender, EventArgs e)
        {
            this.LoadChatReports();
        }
    }
}
