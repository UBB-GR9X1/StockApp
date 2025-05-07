namespace StockApp.Views.Components
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Src.Model;
    using StockApp.Models;
    using StockApp.Services;
    using System.Threading.Tasks;

    public sealed partial class BillSplitReportComponent : Page
    {
        private readonly IBillSplitReportService _billSplitReportService;

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
            _billSplitReportService = billSplitReportService;
        }

        private async void OnSolveClick(object sender, RoutedEventArgs e)
        {
            BillSplitReport billSplitReport = new BillSplitReport
            {
                Id = Id,
                ReportedUserCnp = ReportedUserCNP,
                ReportingUserCnp = ReporterUserCNP,
                DateOfTransaction = DateTransaction,
                BillShare = BillShare
            };

            try
            {
                await _billSplitReportService.SolveBillSplitReportAsync(billSplitReport);
                ReportSolved?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // Show error message
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Failed to solve report: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = Content.XamlRoot
                };

                await errorDialog.ShowAsync();
            }
        }

        private async void OnDropReportClick(object sender, RoutedEventArgs e)
        {
            BillSplitReport billSplitReport = new BillSplitReport
            {
                Id = Id,
                ReportedUserCnp = ReportedUserCNP,
                ReportingUserCnp = ReporterUserCNP,
                DateOfTransaction = DateTransaction,
                BillShare = BillShare
            };

            try
            {
                await _billSplitReportService.DeleteBillSplitReportAsync(billSplitReport);
                ReportSolved?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // Show error message
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Failed to delete report: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = Content.XamlRoot
                };

                await errorDialog.ShowAsync();
            }
        }

        public async void SetReportData(BillSplitReport billSplitReport)
        {
            try
            {
                User reportedUser = await _billSplitReportService.GetUserByCNPAsync(billSplitReport.ReportedUserCnp);
                User reporterUser = await _billSplitReportService.GetUserByCNPAsync(billSplitReport.ReportingUserCnp);

                Id = billSplitReport.Id;
                ReportedUserCNP = billSplitReport.ReportedUserCnp;
                ReportedUserFirstName = reportedUser.FirstName;
                ReportedUserLastName = reportedUser.LastName;
                ReporterUserCNP = billSplitReport.ReportingUserCnp;
                ReporterUserFirstName = reporterUser.FirstName;
                ReporterUserLastName = reporterUser.LastName;
                DateTransaction = billSplitReport.DateOfTransaction;
                BillShare = billSplitReport.BillShare;

                IdTextBlock.Text = $"Report ID: {Id}";
                ReportedUserCNPTextBlock.Text = $"CNP: {ReportedUserCNP}";
                ReportedUserNameTextBlock.Text = $"{reportedUser.FirstName} {reportedUser.LastName}";
                ReporterUserCNPTextBlock.Text = $"CNP: {ReporterUserCNP}";
                ReporterUserNameTextBlock.Text = $"{reporterUser.FirstName} {reporterUser.LastName}";
                DateTransactionTextBlock.Text = $"{DateTransaction}";
                DaysOverdueTextBlock.Text = $"{await _billSplitReportService.GetDaysOverdueAsync(billSplitReport)} days overdue!";
                BillShareTextBlock.Text = $"Bill share: {BillShare}";
            }
            catch (Exception ex)
            {
                // Simple error handling
                IdTextBlock.Text = $"Error loading report: {ex.Message}";
            }
        }
    }
}