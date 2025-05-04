namespace StockApp.Services
{
    using System.Collections.Generic;
    using Src.Model;
    using StockApp.Models;

    public interface ILoanService
    {
        public List<Loan> GetLoans();
        public List<Loan> GetUserLoans(string userCNP);
        public void AddLoan(LoanRequest loanRequest);
        public void CheckLoans();
        public int ComputeNewCreditScore(User user, Loan loan);
        public void UpdateHistoryForUser(string userCNP, int newScore);
        public void IncrementMonthlyPaymentsCompleted(int loanID, float penalty);
    }
}
