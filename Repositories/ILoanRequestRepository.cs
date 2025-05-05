namespace StockApp.Repositories
{
    using System;
    using System.Collections.Generic;
    using Src.Model;

    public interface ILoanRequestRepository
    {
        List<LoanRequest> GetLoanRequests();

        List<LoanRequest> GetUnsolvedLoanRequests();

        void SolveLoanRequest(int loanRequestID);

        void DeleteLoanRequest(int loanRequestID);
    }
}
