using System.Collections.Generic;
using System.Threading.Tasks;
using Src.Model;
using StockApp.Models;

namespace StockApp.Services
{
    public interface ILoanRequestService
    {
        float ComputeMonthlyDebtAmount(User user, LoanService loanServices);
        void DenyLoanRequest(LoanRequest loanRequest);
        List<LoanRequest> GetLoanRequests();
        List<LoanRequest> GetUnsolvedLoanRequests();
        Task<string> GiveSuggestion(LoanRequest loanRequest);
        bool PastUnpaidLoans(User user, LoanService loanService);
        void SolveLoanRequest(LoanRequest loanRequest);
    }
}