namespace StockApp.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Src.Model;
    using StockApp.Repositories;

    public class BillSplitReportViewModel
    {
        private readonly IBillSplitReportRepository repository;

        public ObservableCollection<BillSplitReport> BillSplitReports { get; set; }

        public BillSplitReportViewModel(IBillSplitReportRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.BillSplitReports = new ObservableCollection<BillSplitReport>();
            LoadBillSplitReportsAsync().ConfigureAwait(false);
        }

        public async Task LoadBillSplitReportsAsync()
        {
            try
            {
                this.BillSplitReports.Clear();
                var reports = await this.repository.GetAllReportsAsync();
                foreach (var report in reports)
                {
                    this.BillSplitReports.Add(report);
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {exception.Message}");
            }
        }
    }
}
