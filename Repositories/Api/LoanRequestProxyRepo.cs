using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Repositories.Api
{
    public class LoanRequestProxyRepo : ILoanRequestRepository
    {
        private readonly HttpClient _httpClient;

        public LoanRequestProxyRepo(HttpClient httpClient)
        {
            this._httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<LoanRequest>> GetLoanRequests()
        {
            var response = await _httpClient.GetAsync("api/LoanRequest");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<LoanRequest>>();
        }

        public async Task<List<LoanRequest>> GetUnsolvedLoanRequests()
        {
            var response = await _httpClient.GetAsync("api/LoanRequest/unsolved");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<LoanRequest>>();
        }

        public async Task SolveLoanRequest(LoanRequest loanRequest)
        {
            var response = await _httpClient.PatchAsync($"api/LoanRequest/{loanRequest.Id}/solve", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteLoanRequest(LoanRequest loanRequest)
        {
            var response = await _httpClient.DeleteAsync($"api/LoanRequest/{loanRequest.Id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
