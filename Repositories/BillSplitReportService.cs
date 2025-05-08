namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Src.Model;
    using StockApp.Models;
    using StockApp.Repositories;

    /// <summary>
    /// Service for managing bill split reports.
    /// </summary>
    public class BillSplitReportService : IBillSplitReportService
    {
        private readonly IBillSplitReportRepository billSplitReportRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IUserRepository userRepository;

        public BillSplitReportService(IBillSplitReportRepository billSplitReportRepository, IUserRepository userRepository, ITransactionRepository transactionRepository)
        {
            this.billSplitReportRepository = billSplitReportRepository;
            this.userRepository = userRepository;
            this.transactionRepository = transactionRepository;
        }

        public async Task<List<BillSplitReport>> GetBillSplitReportsAsync()
        {
            return await this.billSplitReportRepository.GetAllReportsAsync();
        }

        public async Task CreateBillSplitReport(BillSplitReport billSplitReport)
        {
            await this.billSplitReportRepository.AddReportAsync(billSplitReport);
        }

        public int GetDaysOverdue(BillSplitReport billSplitReport)
        {
            DateTime currentDate = DateTime.Now;
            TimeSpan timeSpan = currentDate - billSplitReport.DateOfTransaction;
            int daysPastDue = (int)timeSpan.TotalDays;
            return daysPastDue;
        }

        private async Task<decimal> SumTransactionsSinceReport(BillSplitReport billSplitReport)
        {
            // Assuming there is a method in the repository to get transactions since a specific date
            var transactions = await this.transactionRepository.GetTransactionsSinceAsync(billSplitReport.DateOfTransaction, billSplitReport.ReportedUserCnp);

            // Sum the transaction amounts
            decimal totalSum = 0;
            foreach (var transaction in transactions)
            {
                totalSum += transaction.Amount;
            }

            return totalSum;
        }

        public async Task SolveBillSplitReport(BillSplitReport billSplitReportToBeSolved)
        {
            int daysPastDue = this.GetDaysOverdue(billSplitReportToBeSolved);

            float timeFactor = Math.Min(50, (daysPastDue - 1) * 50 / 20.0f);

            float amountFactor = Math.Min(50, (billSplitReportToBeSolved.BillShare - 1) * 50 / 999.0f);

            float gravityFactor = timeFactor + amountFactor;

            User user = await this.userRepository.GetUserByCnpAsync(billSplitReportToBeSolved.ReportedUserCnp);
            decimal transactionsSum = await this.SumTransactionsSinceReport(billSplitReportToBeSolved);

            bool couldHavePaidBillShare = user.Balance + transactionsSum >= (decimal)billSplitReportToBeSolved.BillShare;

            if (couldHavePaidBillShare)
            {
                gravityFactor += gravityFactor * 0.1f;
            }

            if (this.CheckHistoryOfBillShares(billSplitReportToBeSolved))
            {
                gravityFactor -= gravityFactor * 0.05f;
            }

            if (this.CheckFrequentTransfers(billSplitReportToBeSolved))
            {
                gravityFactor -= gravityFactor * 0.05f;
            }

            int numberOfOffenses = this.GetNumberOfOffenses(billSplitReportToBeSolved);
            gravityFactor += (float)Math.Floor(numberOfOffenses * 0.1f);

            int newCreditScore = (int)Math.Floor(currentBalance - 0.2f * gravityFactor);

            this.UpdateCreditScore(billSplitReportToBeSolved, newCreditScore);
            this.UpdateCreditScoreHistory(billSplitReportToBeSolved, newCreditScore);

            this.IncrementNoOfBillSharesPaid(billSplitReportToBeSolved);

            await this.billSplitReportRepository.DeleteReportAsync(billSplitReportToBeSolved.Id);
        }

        private bool CheckHistoryOfBillShares(BillSplitReport billSplitReportToBeSolved)
        {

        }

        public void DeleteBillSplitReport(BillSplitReport billSplitReportToBeSolved)
        {
            this.billSplitReportRepository.DeleteReportAsync(billSplitReportToBeSolved.Id);
        }
    }
}
