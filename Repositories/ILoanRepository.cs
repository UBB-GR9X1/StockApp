namespace StockApp.Repositories
{
    using System.Collections.Generic;
    using Src.Model;

    public interface ILoanRepository
    {
        List<Loan> GetLoans();

        List<Loan> GetUserLoans(string userCNP);

        void AddLoan(Loan loan);

        void UpdateLoan(Loan loan);

        void DeleteLoan(int loanID);

        Loan GetLoanById(int loanID);

        void UpdateCreditScoreHistoryForUser(string userCNP, int newScore);
    }
}
