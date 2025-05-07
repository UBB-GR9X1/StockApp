using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Src.Model;
using StockApp.Services;

namespace StockApp.ViewModels
{
    public class BillSplitReportViewModel : INotifyPropertyChanged
    {
        private readonly IBillSplitReportService billSplitReportService;
        private ObservableCollection<BillSplitReport> billSplitReports;

        public BillSplitReportViewModel(IBillSplitReportService billSplitReportService)
        {
            this.billSplitReportService = billSplitReportService;
            
            // Convert the List to ObservableCollection
            var reports = this.billSplitReportService.GetBillSplitReportsAsync().GetAwaiter().GetResult();
            this.billSplitReports = new ObservableCollection<BillSplitReport>(reports);
        }

        public ObservableCollection<BillSplitReport> BillSplitReports
        {
            get => this.billSplitReports;
            set
            {
                this.billSplitReports = value;
                this.OnPropertyChanged();
            }
        }

        public async Task RefreshReportsAsync()
        {
            var reports = await this.billSplitReportService.GetBillSplitReportsAsync();
            this.BillSplitReports = new ObservableCollection<BillSplitReport>(reports);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
