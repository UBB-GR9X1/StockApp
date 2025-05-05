namespace StockApp.Services
{
    using System.Collections.Generic;
    using Src.Model;
    using StockApp.Models;

    public interface ILoanRequestService
    {
        string GiveSuggestion(LoanRequest loanRequest);

        void SolveLoanRequest(LoanRequest loanRequest);

        void DenyLoanRequest(LoanRequest loanRequest);

        List<LoanRequest> GetLoanRequests();

        List<LoanRequest> GetUnsolvedLoanRequests();

        bool PastUnpaidLoans(User user, LoanService loanService);
    }
}
