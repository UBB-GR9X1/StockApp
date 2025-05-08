namespace StockApp.Views.Pages
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.UI.Xaml.Controls;
    using Src.Model;
    using StockApp.ViewModels;
    using StockApp.Views.Components;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Navigation;

    public sealed partial class BillSplitReportPage : Page
    {
        private readonly BillSplitReportViewModel viewModel;
        private readonly Func<BillSplitReportComponent> billSplitReportComponentFactory;

        public BillSplitReportPage(
            BillSplitReportViewModel viewModel,
            Func<BillSplitReportComponent> billSplitReportComponentFactory)
        {
            this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            this.billSplitReportComponentFactory = billSplitReportComponentFactory ?? throw new ArgumentNullException(nameof(billSplitReportComponentFactory));
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
                await component.SetReportDataAsync(report);
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
                    await viewModel.DeleteReportAsync(report.Id);
                    // The view will be updated via the ReportUpdated event in the ViewModel
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
                // The ViewModel will update the UI when the report is created
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
