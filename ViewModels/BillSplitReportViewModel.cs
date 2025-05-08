namespace StockApp.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Src.Model;
    using StockApp.Models;
    using StockApp.Services;
    using StockApp.Repositories;

    public class BillSplitReportViewModel
    {
        private readonly IBillSplitReportApiService apiService;
        private readonly IUserService userService;
        private readonly IUserRepository userRepository;

        public ObservableCollection<BillSplitReport> BillSplitReports { get; set; }

        public event EventHandler ReportUpdated;

        public BillSplitReportViewModel(
            IBillSplitReportApiService apiService, 
            IUserService userService,
            IUserRepository userRepository)
        {
            this.apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.BillSplitReports = new ObservableCollection<BillSplitReport>();
            LoadBillSplitReportsAsync().ConfigureAwait(false);
        }

        public async Task LoadBillSplitReportsAsync()
        {
            try
            {
                this.BillSplitReports.Clear();
                var reports = await this.apiService.GetAllReportsAsync();
                foreach (var report in reports)
                {
                    this.BillSplitReports.Add(report);
                }
                OnReportUpdated();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {exception.Message}");
            }
        }

        public async Task<User> GetUserByCnpAsync(string cnp)
        {
            try
            {
                return await this.userRepository.GetUserByCnpAsync(cnp);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting user by CNP: {ex.Message}");
                return new User
                {
                    FirstName = "Unknown",
                    LastName = "User",
                    CNP = cnp
                };
            }
        }

        public async Task<bool> DeleteReportAsync(int id)
        {
            try
            {
                var result = await this.apiService.DeleteReportAsync(id);
                await LoadBillSplitReportsAsync();
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting report: {ex.Message}");
                throw;
            }
        }

        public async Task<BillSplitReport> AddReportAsync(BillSplitReport report)
        {
            try
            {
                var result = await this.apiService.CreateReportAsync(report);
                await LoadBillSplitReportsAsync();
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding report: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateReportAsync(BillSplitReport report)
        {
            try
            {
                var result = await this.apiService.UpdateReportAsync(report);
                await LoadBillSplitReportsAsync();
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating report: {ex.Message}");
                throw;
            }
        }

        private void OnReportUpdated()
        {
            ReportUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
