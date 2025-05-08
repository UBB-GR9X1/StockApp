using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Src.Model;
using StockApp.Services;

namespace StockApp.Repositories.Api
{
    class LoanRequestProxyRepo : ILoanRequestRepository
    {
        private readonly LoanRequestService _apiService;

        public LoanRequestProxyRepo(LoanRequestService apiService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }

        public async Task<List<LoanRequest>> GetLoanRequests()
        {
            return await _apiService.GetLoanRequests();
        }

        public async Task<List<LoanRequest>> GetUnsolvedLoanRequests()
        {
            return await _apiService.GetUnsolvedLoanRequests();
        }

        public async Task SolveLoanRequest(LoanRequest loanRequest)
        {
            await _apiService.SolveLoanRequest(loanRequest);
        }

        public async Task DeleteLoanRequest(LoanRequest loanRequest)
        {
            await _apiService.DeleteLoanRequest(loanRequest);
        }
    }
}
