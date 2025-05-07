using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Src.Model;
using StockApp.Models;
using StockApp.Repositories;
using System.Threading.Tasks;

namespace StockApp.Views.Components
{
    public sealed partial class BillSplitReportComponent : Page
    {
        private readonly IBillSplitReportRepository repository;
        private IUserRepository userRepository;

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

        public BillSplitReportComponent(IBillSplitReportRepository repository)
        {
            this.InitializeComponent();
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
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
                await repository.DeleteReportAsync(billSplitReport.Id);
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
                await repository.DeleteReportAsync(billSplitReport.Id);
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

        public void SetReportData(BillSplitReport billSplitReport, IUserRepository userRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            
            // Try to get user details, but handle missing users gracefully
            User reportedUser = null;
            User reporterUser = null;
            
            try
            {
                reportedUser = this.userRepository.GetUserByCnpAsync(billSplitReport.ReportedUserCnp).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting reported user: {ex.Message}");
                reportedUser = new User
                {
                    FirstName = "Unknown",
                    LastName = "User",
                    CNP = billSplitReport.ReportedUserCnp
                };
            }
            
            try
            {
                reporterUser = this.userRepository.GetUserByCnpAsync(billSplitReport.ReportingUserCnp).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting reporter user: {ex.Message}");
                reporterUser = new User
                {
                    FirstName = "Unknown",
                    LastName = "User",
                    CNP = billSplitReport.ReportingUserCnp
                };
            }

            try
            {
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