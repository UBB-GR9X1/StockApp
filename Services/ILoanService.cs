namespace StockApp.Services
{
    using System.Collections.Generic;
    using Src.Model;
    using StockApp.Models;

    public interface ILoanService
    {
        List<Loan> GetLoans();

        List<Loan> GetUserLoans(string userCNP);

        void AddLoan(LoanRequest loanRequest);

        void CheckLoans();

        int ComputeNewCreditScore(User user, Loan loan);

        void UpdateHistoryForUser(string userCNP, int newScore);

        void IncrementMonthlyPaymentsCompleted(int loanID, float penalty);
    }
}
