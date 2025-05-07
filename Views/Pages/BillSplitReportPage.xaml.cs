namespace StockApp.Views.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.UI.Xaml.Controls;
    using Src.Model;
    using StockApp.Repositories;
    using StockApp.Services;
    using StockApp.Views.Components;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Navigation;
    using StockApp.ViewModels;

    public sealed partial class BillSplitReportPage : Page
    {
        private readonly IBillSplitReportRepository repository;
        private readonly BillSplitReportViewModel viewModel;
        private readonly Func<BillSplitReportComponent> billSplitReportComponentFactory;
        private readonly IUserRepository userRepository;

        public BillSplitReportPage(
            IBillSplitReportRepository repository,
            BillSplitReportViewModel viewModel,
            Func<BillSplitReportComponent> billSplitReportComponentFactory,
            IUserRepository userRepository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            this.billSplitReportComponentFactory = billSplitReportComponentFactory ?? throw new ArgumentNullException(nameof(billSplitReportComponentFactory));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await viewModel.LoadBillSplitReportsAsync();
            
            // Clear existing items and add reports
            this.BillSplitReportsContainer.Items.Clear();
            foreach (var report in viewModel.BillSplitReports)
            {
                var component = billSplitReportComponentFactory();
                component.SetReportData(report, this.userRepository);
                component.ReportSolved += async (s, args) => await viewModel.LoadBillSplitReportsAsync();
                this.BillSplitReportsContainer.Items.Add(component);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is BillSplitReport report)
            {
                try
                {
                    await repository.DeleteReportAsync(report.Id);
                    await viewModel.LoadBillSplitReportsAsync();
                    // Refresh the UI
                    this.OnNavigatedTo(null);
                }
                catch (Exception ex)
                {
                    ShowError($"Error deleting report: {ex.Message}");
                }
            }
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var component = billSplitReportComponentFactory();
                component.XamlRoot = this.XamlRoot;
                await component.ShowCreateDialogAsync();
                await viewModel.LoadBillSplitReportsAsync();
                // Refresh the UI
                this.OnNavigatedTo(null);
            }
            catch (Exception ex)
            {
                ShowError($"Error creating report: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            // Simple error handling, you could enhance this with a proper error display
            System.Diagnostics.Debug.WriteLine(message);
            // You could use ContentDialog to display errors to the user
        }
    }
}
