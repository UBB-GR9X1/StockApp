namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using Src.Model;
    using StockApp.Models;
    using StockApp.Repositories;

    public class BillSplitReportService : IBillSplitReportService
    {
        private readonly IBillSplitReportRepository billSplitReportRepository;

        public BillSplitReportService(IBillSplitReportRepository billSplitReportRepository)
        {
            this.billSplitReportRepository = billSplitReportRepository;
        }

        public List<BillSplitReport> GetBillSplitReports()
        {
            return this.billSplitReportRepository.GetBillSplitReports();
        }

        public void CreateBillSplitReport(BillSplitReport billSplitReport)
        {
            this.billSplitReportRepository.CreateBillSplitReport(billSplitReport);
        }

        public int GetDaysOverdue(BillSplitReport billSplitReport)
        {
            return this.billSplitReportRepository.GetDaysOverdue(billSplitReport);
        }

        public void SolveBillSplitReport(BillSplitReport billSplitReportToBeSolved)
        {
            int daysPastDue = this.GetDaysOverdue(billSplitReportToBeSolved);

            float timeFactor = Math.Min(50, (daysPastDue - 1) * 50 / 20.0f);

            float amountFactor = Math.Min(50, (billSplitReportToBeSolved.BillShare - 1) * 50 / 999.0f);

            float gravityFactor = timeFactor + amountFactor;

            int currentBalance = this.billSplitReportRepository.GetCurrentCreditScore(billSplitReportToBeSolved);
            decimal transactionsSum = this.billSplitReportRepository.SumTransactionsSinceReport(billSplitReportToBeSolved);

            bool couldHavePaidBillShare = currentBalance + transactionsSum >= (decimal)billSplitReportToBeSolved.BillShare;

            if (couldHavePaidBillShare)
            {
                gravityFactor += gravityFactor * 0.1f;
            }

            if (this.billSplitReportRepository.CheckHistoryOfBillShares(billSplitReportToBeSolved))
            {
                gravityFactor -= gravityFactor * 0.05f;
            }

            if (this.billSplitReportRepository.CheckFrequentTransfers(billSplitReportToBeSolved))
            {
                gravityFactor -= gravityFactor * 0.05f;
            }

            int numberOfOffenses = this.billSplitReportRepository.GetNumberOfOffenses(billSplitReportToBeSolved);
            gravityFactor += (float)Math.Floor(numberOfOffenses * 0.1f);

            int newCreditScore = (int)Math.Floor(currentBalance - 0.2f * gravityFactor);

            this.billSplitReportRepository.UpdateCreditScore(billSplitReportToBeSolved, newCreditScore);
            this.billSplitReportRepository.UpdateCreditScoreHistory(billSplitReportToBeSolved, newCreditScore);

            this.billSplitReportRepository.IncrementNoOfBillSharesPaid(billSplitReportToBeSolved);

            this.billSplitReportRepository.DeleteBillSplitReport(billSplitReportToBeSolved.Id);
        }

        public void DeleteBillSplitReport(BillSplitReport billSplitReportToBeSolved)
        {
            this.billSplitReportRepository.DeleteBillSplitReport(billSplitReportToBeSolved.Id);
        }

        public User GetUserByCNP(string cNP)
        {
            return new User();
        }
    }
}
