namespace StockApp.Views.Pages
{
    using System;
    using System.Collections.Generic;
    using Microsoft.UI.Xaml.Controls;
    using Src.Data;
    using Src.Model;
    using StockApp.Repositories;
    using StockApp.Services;
    using StockApp.Views.Components;

    public sealed partial class BillSplitReportPage : Page
    {
        private readonly Func<BillSplitReportComponent> componentFactory;
        private readonly IBillSplitReportService _billSplitReportService;

        public BillSplitReportPage(Func<BillSplitReportComponent> componentFactory, IBillSplitReportService billSplitReportService)
        {
            this.componentFactory = componentFactory;
            this._billSplitReportService = billSplitReportService;
            this.InitializeComponent();
            this.LoadReportsAsync();
        }

        private async void LoadReportsAsync()
        {
            BillSplitReportsContainer.Items.Clear();

            try
            {
                List<BillSplitReport> reports = await _billSplitReportService.GetBillSplitReportsAsync();

                foreach (var report in reports)
                {
                    var reportComponent = componentFactory();
                    reportComponent.SetReportData(report);
                    reportComponent.ReportSolved += OnReportSolved;
                    BillSplitReportsContainer.Items.Add(reportComponent);
                }
                
                if (reports.Count == 0)
                {
                    BillSplitReportsContainer.Items.Add("There are no bill split reports that need solving.");
                }
            }
            catch (Exception ex)
            {
                BillSplitReportsContainer.Items.Add($"Error loading reports: {ex.Message}");
            }
        }

        private void OnReportSolved(object sender, EventArgs e)
        {
            this.LoadReportsAsync();
        }
    }
}
