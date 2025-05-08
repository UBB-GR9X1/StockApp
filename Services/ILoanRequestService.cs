namespace StockApp.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Src.Model;
    using StockApp.Models;

    public interface ILoanRequestService
    {
        Task<string> GiveSuggestion(LoanRequest loanRequest);

        Task SolveLoanRequest(LoanRequest loanRequest);

        Task DeleteLoanRequest(LoanRequest loanRequest);

        Task<List<LoanRequest>> GetLoanRequests();

        Task<List<LoanRequest>> GetUnsolvedLoanRequests();
    }
}