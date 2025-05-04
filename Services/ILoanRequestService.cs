namespace StockApp.Services
{
    using System.Collections.Generic;
    using Src.Model;
    using StockApp.Models;

    public interface ILoanRequestService
    {
        public string GiveSuggestion(LoanRequest loanRequest);
        public void SolveLoanRequest(LoanRequest loanRequest);
        public void DenyLoanRequest(LoanRequest loanRequest);
        public List<LoanRequest> GetLoanRequests();
        public List<LoanRequest> GetUnsolvedLoanRequests();
        bool PastUnpaidLoans(User user, LoanService loanService);
    }
}
