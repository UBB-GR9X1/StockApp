using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Src.Model;
using StockApp.Models;
using StockApp.Repositories;
using StockApp.Services;

namespace StockApp.ViewModels
{
    public class BillSplitReportViewModel
    {
        private readonly IBillSplitReportService _billSplitReportService;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;

        public ObservableCollection<BillSplitReport> BillSplitReports { get; }

        public event EventHandler? ReportUpdated;

        public BillSplitReportViewModel(
            IBillSplitReportService billSplitReportService,
            IUserService userService,
            IUserRepository userRepository)
        {
            _billSplitReportService = billSplitReportService
                                      ?? throw new ArgumentNullException(nameof(billSplitReportService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

            BillSplitReports = new ObservableCollection<BillSplitReport>();

            _ = LoadBillSplitReportsAsync();   // fire-and-forget; errors logged inside
        }

        /* ───────────────  Public API  ─────────────── */

        public async Task LoadBillSplitReportsAsync()
        {
            try
            {
                BillSplitReports.Clear();

                var reports = await _billSplitReportService
                                    .GetBillSplitReportsAsync()
                                    .ConfigureAwait(false);

                foreach (var r in reports) BillSplitReports.Add(r);

                OnReportUpdated();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading reports: {ex.Message}");
            }
        }

        public async Task<User> GetUserByCnpAsync(string cnp)
        {
            try
            {
                return await _userRepository.GetUserByCnpAsync(cnp)
                                            .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting user: {ex.Message}");
                return new User { FirstName = "Unknown", LastName = "User", CNP = cnp };
            }
        }

        public async Task DeleteReportAsync(BillSplitReport report)
        {
            try
            {
                await _billSplitReportService
                      .DeleteBillSplitReportAsync(report)
                      .ConfigureAwait(false);

                await LoadBillSplitReportsAsync().ConfigureAwait(false);
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
                var added = await _billSplitReportService
                             .CreateBillSplitReportAsync(report)
                             .ConfigureAwait(false);

                await LoadBillSplitReportsAsync().ConfigureAwait(false);
                return added;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding report: {ex.Message}");
                throw;
            }
        }

        public async Task<BillSplitReport> UpdateReportAsync(BillSplitReport report)
        {
            try
            {
                var updated = await _billSplitReportService
                               .UpdateBillSplitReportAsync(report)
                               .ConfigureAwait(false);

                await LoadBillSplitReportsAsync().ConfigureAwait(false);
                return updated;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating report: {ex.Message}");
                throw;
            }
        }

        /* ───────────────  Helpers  ─────────────── */

        private void OnReportUpdated() => ReportUpdated?.Invoke(this, EventArgs.Empty);
    }
}
