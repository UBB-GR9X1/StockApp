using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Src.Model;
using StockApp.Models;
using StockApp.ViewModels;
using System.Threading.Tasks;

namespace StockApp.Views.Components
{
    public sealed partial class BillSplitReportComponent : Page
    {
        private readonly BillSplitReportViewModel viewModel;

        public event EventHandler ReportSolved;
        public XamlRoot XamlRoot { get; set; }

        public int Id { get; set; }
        public string ReportedUserCNP { get; set; }
        public string ReportedUserFirstName { get; set; }
        public string ReportedUserLastName { get; set; }
        public string ReporterUserCNP { get; set; }
        public string ReporterUserFirstName { get; set; }
        public string ReporterUserLastName { get; set; }
        public DateTime DateTransaction { get; set; }
        private float BillShare { get; set; }

        public BillSplitReportComponent(BillSplitReportViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            this.viewModel.ReportUpdated += (s, e) => this.ReportSolved?.Invoke(this, EventArgs.Empty);
        }

        public async Task ShowCreateDialogAsync()
        {
            ContentDialog createDialog = new ContentDialog
            {
                Title = "Create Bill Split Report",
                PrimaryButtonText = "Create",
                CloseButtonText = "Cancel",
                XamlRoot = this.XamlRoot
            };

            var result = await createDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // Implement the create logic
            }
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

            try
            {
                // Solve = delete in this implementation
                await viewModel.DeleteReportAsync(billSplitReport);
                this.ReportSolved?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // Show error to user
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Failed to solve report: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };

                await errorDialog.ShowAsync();
            }
        }

        private async void OnDropReportClick(object sender, RoutedEventArgs e)
        {
            BillSplitReport billSplitReport = new BillSplitReport
            {
                Id = this.Id,
                ReportedUserCnp = this.ReportedUserCNP,
                ReportingUserCnp = this.ReporterUserCNP,
                DateOfTransaction = this.DateTransaction,
                BillShare = this.BillShare
            };

            try
            {
                await viewModel.DeleteReportAsync(billSplitReport);
                this.ReportSolved?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // Show error to user
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Failed to delete report: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };

                await errorDialog.ShowAsync();
            }
        }

        public async Task SetReportDataAsync(BillSplitReport billSplitReport)
        {
            try
            {
                // Get user details using the ViewModel
                User reportedUser = await viewModel.GetUserByCnpAsync(billSplitReport.ReportedUserCnp);
                User reporterUser = await viewModel.GetUserByCnpAsync(billSplitReport.ReportingUserCnp);

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
                this.DaysOverdueTextBlock.Text = $"{(DateTime.Now - billSplitReport.DateOfTransaction).Days} days overdue!";
                this.BillShareTextBlock.Text = $"Bill share: {this.BillShare}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting report data: {ex.Message}");
                this.IdTextBlock.Text = $"Error: {ex.Message}";
            }
        }
    }
}