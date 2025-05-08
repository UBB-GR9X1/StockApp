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

        public BillSplitReportPage(Func<BillSplitReportComponent> componentFactory)
        {
            this.componentFactory = componentFactory;
            this.InitializeComponent();
            this.LoadReports();
        }

        private void LoadReports()
        {
            this.BillSplitReportsContainer.Items.Clear();

            DatabaseConnection dbConnection = new DatabaseConnection();
            BillSplitReportRepository billSplitReportRepository = new BillSplitReportRepository(dbConnection);
            BillSplitReportService billSplitReportService = new BillSplitReportService(billSplitReportRepository);

            try
            {
                List<BillSplitReport> reports = billSplitReportService.GetBillSplitReports();

                foreach (var report in reports)
                {
                    var reportComponent = this.componentFactory();
                    reportComponent.SetReportData(report);
                    reportComponent.ReportSolved += this.OnReportSolved;
                    this.BillSplitReportsContainer.Items.Add(reportComponent);
                }
            }
            catch (Exception)
            {
                this.BillSplitReportsContainer.Items.Add("There are no chat reports that need solving.");
            }
        }

        private void OnReportSolved(object sender, EventArgs e)
        {
            this.LoadReports();
        }
    }
}
