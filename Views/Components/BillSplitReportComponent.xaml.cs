namespace StockApp.Views.Components
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Src.Model;
    using StockApp.Models;
    using StockApp.Services;

    public sealed partial class BillSplitReportComponent : Page
    {
        private readonly IBillSplitReportService billSplitReportService;

        public event EventHandler ReportSolved;

        public int Id { get; set; }

        public string ReportedUserCNP { get; set; }

        public string ReportedUserFirstName { get; set; }

        public string ReportedUserLastName { get; set; }

        public string ReporterUserCNP { get; set; }

        public string ReporterUserFirstName { get; set; }

        public string ReporterUserLastName { get; set; }

        public DateTime DateTransaction { get; set; }

        private float BillShare { get; set; }

        public BillSplitReportComponent(IBillSplitReportService billSplitReportService)
        {
            this.InitializeComponent();
            this.billSplitReportService = billSplitReportService;
        }

        private async void OnSolveClick(object sender, RoutedEventArgs e)
        {
            BillSplitReport billSplitReport = new BillSplitReport
            {
                Id = this.Id,
                ReportedUserCnp = this.ReportedUserCNP,
                ReportingUserCnp = this.ReporterUserCNP,
                DateOfTransaction = this.DateTransaction,
                BillShare = this.BillShare
            };

            this.billSplitReportService.SolveBillSplitReport(billSplitReport);
            this.ReportSolved?.Invoke(this, EventArgs.Empty);
        }

        private void OnDropReportClick(object sender, RoutedEventArgs e)
        {
            BillSplitReport billSplitReport = new BillSplitReport
            {
                Id = this.Id,
                ReportedUserCnp = this.ReportedUserCNP,
                ReportingUserCnp = this.ReporterUserCNP,
                DateOfTransaction = this.DateTransaction,
                BillShare = this.BillShare
            };

            this.billSplitReportService.DeleteBillSplitReport(billSplitReport);
            this.ReportSolved?.Invoke(this, EventArgs.Empty);
        }

        public void SetReportData(BillSplitReport billSplitReport)
        {
            User reportedUser = this.billSplitReportService.GetUserByCNP(billSplitReport.ReportedUserCnp);
            User reporterUser = this.billSplitReportService.GetUserByCNP(billSplitReport.ReportingUserCnp);

            this.Id = billSplitReport.Id;
            this.ReportedUserCNP = billSplitReport.ReportedUserCnp;
            this.ReportedUserFirstName = reportedUser.FirstName;
            this.ReportedUserLastName = reportedUser.LastName;
            this.ReporterUserCNP = billSplitReport.ReportingUserCnp;
            this.ReporterUserFirstName = reporterUser.FirstName;
            this.ReporterUserLastName = reporterUser.LastName;
            this.DateTransaction = billSplitReport.DateOfTransaction;
            this.BillShare = billSplitReport.BillShare;

            this.IdTextBlock.Text = $"Report ID: {this.Id}";
            this.ReportedUserCNPTextBlock.Text = $"CNP: {this.ReportedUserCNP}";
            this.ReportedUserNameTextBlock.Text = $"{reportedUser.FirstName} {reportedUser.LastName}";
            this.ReporterUserCNPTextBlock.Text = $"CNP: {this.ReporterUserCNP}";
            this.ReporterUserNameTextBlock.Text = $"{reporterUser.FirstName} {reporterUser.LastName}";
            this.DateTransactionTextBlock.Text = $"{this.DateTransaction}";
            this.DaysOverdueTextBlock.Text = $"{this.billSplitReportService.GetDaysOverdue(billSplitReport)} days overdue!";
            this.BillShareTextBlock.Text = $"Bill share: {this.BillShare}";
        }
    }
}