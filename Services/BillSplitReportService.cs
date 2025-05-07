namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Src.Model;
    using StockApp.Models;
    using StockApp.Repositories;

    public class BillSplitReportService : IBillSplitReportService
    {
        private readonly IBillSplitReportRepository _billSplitReportRepository;
        private readonly IUserRepository _userRepository;

        public BillSplitReportService(IBillSplitReportRepository billSplitReportRepository, IUserRepository userRepository)
        {
            _billSplitReportRepository = billSplitReportRepository;
            _userRepository = userRepository;
        }

        public async Task<List<BillSplitReport>> GetBillSplitReportsAsync()
        {
            return await _billSplitReportRepository.GetBillSplitReportsAsync();
        }

        public async Task<BillSplitReport> CreateBillSplitReportAsync(BillSplitReport billSplitReport)
        {
            return await _billSplitReportRepository.CreateBillSplitReportAsync(billSplitReport);
        }

        public async Task<int> GetDaysOverdueAsync(BillSplitReport billSplitReport)
        {
            return await _billSplitReportRepository.GetDaysOverdueAsync(billSplitReport);
        }

        public async Task SolveBillSplitReportAsync(BillSplitReport billSplitReportToBeSolved)
        {
            int daysPastDue = await GetDaysOverdueAsync(billSplitReportToBeSolved);

            float timeFactor = Math.Min(50, (daysPastDue - 1) * 50 / 20.0f);
            float amountFactor = Math.Min(50, (billSplitReportToBeSolved.BillShare - 1) * 50 / 999.0f);
            float gravityFactor = timeFactor + amountFactor;

            int currentBalance = await _billSplitReportRepository.GetCurrentCreditScoreAsync(billSplitReportToBeSolved);
            decimal transactionsSum = await _billSplitReportRepository.SumTransactionsSinceReportAsync(billSplitReportToBeSolved);

            bool couldHavePaidBillShare = currentBalance + transactionsSum >= (decimal)billSplitReportToBeSolved.BillShare;

            if (couldHavePaidBillShare)
            {
                gravityFactor += gravityFactor * 0.1f;
            }

            bool historyOfBillShares = await _billSplitReportRepository.CheckHistoryOfBillSharesAsync(billSplitReportToBeSolved);
            if (!historyOfBillShares)
            {
                gravityFactor += gravityFactor * 0.2f;
            }

            bool frequentTransfers = await _billSplitReportRepository.CheckFrequentTransfersAsync(billSplitReportToBeSolved);
            if (frequentTransfers)
            {
                gravityFactor -= gravityFactor * 0.1f;
            }

            int numberOfOffenses = await _billSplitReportRepository.GetNumberOfOffensesAsync(billSplitReportToBeSolved);
            if (numberOfOffenses > 0)
            {
                gravityFactor += gravityFactor * (0.1f * numberOfOffenses);
            }

            int penalty = (int)Math.Floor(gravityFactor);
            int newCreditScore = currentBalance - penalty;

            await _billSplitReportRepository.UpdateCreditScoreAsync(billSplitReportToBeSolved, newCreditScore);
            await _billSplitReportRepository.UpdateCreditScoreHistoryAsync(billSplitReportToBeSolved, newCreditScore);
            await _billSplitReportRepository.IncrementNoOfBillSharesPaidAsync(billSplitReportToBeSolved);

            await DeleteBillSplitReportAsync(billSplitReportToBeSolved);
        }

        public async Task<bool> DeleteBillSplitReportAsync(BillSplitReport billSplitReportToBeSolved)
        {
            return await _billSplitReportRepository.DeleteBillSplitReportAsync(billSplitReportToBeSolved.Id);
        }

        public async Task<User> GetUserByCNPAsync(string cnp)
        {
            // This should be implemented to fetch user data
            // Placeholder implementation for now
            return await Task.FromResult(new User { CNP = cnp });
        }
    }
}
