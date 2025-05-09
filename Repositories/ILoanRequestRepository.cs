namespace StockApp.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;

    public interface ILoanRequestRepository
    {
        Task<List<LoanRequest>> GetLoanRequests();

        Task<List<LoanRequest>> GetUnsolvedLoanRequests();

        Task SolveLoanRequest(LoanRequest loanRequest);

        Task DeleteLoanRequest(LoanRequest loanRequest);
    }
}
