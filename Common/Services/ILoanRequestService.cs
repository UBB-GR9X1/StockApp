namespace Common.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Models;

    public interface ILoanRequestService
    {
        Task<string> GiveSuggestion(LoanRequest loanRequest);

        Task SolveLoanRequest(int loanRequestId);

        Task DeleteLoanRequest(int loanRequestId);

        Task<List<LoanRequest>> GetLoanRequests();

        Task<List<LoanRequest>> GetUnsolvedLoanRequests();
    }
}