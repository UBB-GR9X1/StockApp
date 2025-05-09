using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Repositories.Api
{
    internal class LoanProxyRepository : ILoanRepository
    {
        private readonly HttpClient _httpClient;

        public LoanProxyRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<Loan>> GetLoansAsync()
        {
            var response = await _httpClient.GetAsync("api/Loan");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Loan>>();
        }

        public async Task<Loan> GetLoanByIdAsync(int loanID)
        {
            var response = await _httpClient.GetAsync($"api/Loan/{loanID}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Loan>();
        }

        public async Task AddLoanAsync(Loan loan)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Loan", loan);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateLoanAsync(Loan loan)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Loan/{loan.Id}", loan);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteLoanAsync(int loanID)
        {
            var response = await _httpClient.DeleteAsync($"api/Loan/{loanID}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<Loan>> GetUserLoansAsync(string userCNP)
        {
            var response = await _httpClient.GetAsync($"api/Loan?userCnp={userCNP}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Loan>>();
        }

        public async Task UpdateCreditScoreHistoryForUserAsync(string userCNP, int newScore)
        {
            var response = await _httpClient.PatchAsync($"api/User/{userCNP}/creditScore?newScore={newScore}", null);
            response.EnsureSuccessStatusCode();
        }
    }
}
