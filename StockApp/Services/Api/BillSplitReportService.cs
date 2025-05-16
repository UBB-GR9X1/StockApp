using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Src.Model;
using StockApp.Models;
using StockApp.Repositories;
using StockApp.Repositories.Api;

namespace StockApp.Services.Api
{
    public class BillSplitReportService : IBillSplitReportService
    {
        private readonly IBillSplitReportRepository _repo;
        private const int PaymentTermDays = 30;

        public BillSplitReportService(IBillSplitReportRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /* ───────────────  CRUD wrappers  ─────────────── */

        public async Task<List<BillSplitReport>> GetBillSplitReportsAsync() =>
            await _repo.GetAllReportsAsync().ConfigureAwait(false);

        public async Task<BillSplitReport> CreateBillSplitReportAsync(BillSplitReport report)
        {
            if (report is null) throw new ArgumentNullException(nameof(report));
            return await _repo.AddReportAsync(report).ConfigureAwait(false);
        }

        public async Task<BillSplitReport> UpdateBillSplitReportAsync(BillSplitReport report)   // NEW
        {
            if (report is null) throw new ArgumentNullException(nameof(report));
            return await _repo.UpdateReportAsync(report).ConfigureAwait(false);
        }

        public async Task DeleteBillSplitReportAsync(BillSplitReport report)
        {
            if (report is null) throw new ArgumentNullException(nameof(report));
            _ = await _repo.DeleteReportAsync(report.Id).ConfigureAwait(false);
        }

        /* ───────────────  Helper logic  ─────────────── */

        public Task<int> GetDaysOverdueAsync(BillSplitReport report)
        {
            if (report is null) throw new ArgumentNullException(nameof(report));

            var due = report.DateOfTransaction.AddDays(PaymentTermDays);
            var today = DateTime.UtcNow;
            var overdue = due > today ? 0 : (int)(today - due).TotalDays;

            return Task.FromResult(overdue);
        }

        public async Task SolveBillSplitReportAsync(BillSplitReport report)
        {
            if (report is null) throw new ArgumentNullException(nameof(report));

            int daysPastDue = await GetDaysOverdueAsync(report).ConfigureAwait(false);
            string userCnp = report.ReportedUserCnp;

            float timeFactor = Math.Min(50, (daysPastDue - 1) * 50 / 20.0f);
            float amountFactor = Math.Min(50, (report.BillShare - 1) * 50 / 999.0f);
            float gravity = timeFactor + amountFactor;

            // Extra credit-score endpoints live only on the proxy
            var proxy = (BillSplitReportProxyRepository)_repo;

            int currentScore = await proxy.GetCurrentCreditScoreAsync(userCnp)
                                            .ConfigureAwait(false);
            float txSum = await proxy.SumTransactionsSinceReportAsync(
                                            userCnp, report.DateOfTransaction)
                                            .ConfigureAwait(false);

            bool couldHavePaid = currentScore + txSum >= report.BillShare;
            if (couldHavePaid) gravity += gravity * 0.10f;

            int newScore = (int)Math.Floor(currentScore - 0.20f * gravity);

            await proxy.UpdateCreditScoreAsync(userCnp, newScore).ConfigureAwait(false);
            await _repo.IncrementBillSharesPaidAsync(userCnp).ConfigureAwait(false);
            await _repo.DeleteReportAsync(report.Id).ConfigureAwait(false);
        }

        
        public User GetUserByCNP(string cnp) => new User();
    }
}
