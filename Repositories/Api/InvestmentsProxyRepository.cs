using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Src.Model;
using StockApp.Repositories;

namespace StockApp.Repositories.Api
{
    public class InvestmentsProxyRepository : IInvestmentsRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/investments";

        public InvestmentsProxyRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<Investment>> GetInvestmentsHistory()
        {
            var response = await _httpClient.GetFromJsonAsync<List<Investment>>(BaseUrl);
            return response ?? new List<Investment>();
        }

        public async Task AddInvestment(Investment investment)
        {
            if (investment == null)
                throw new ArgumentNullException(nameof(investment));

            var response = await _httpClient.PostAsJsonAsync(BaseUrl, investment);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateInvestment(int investmentId, string investorCNP, float amountReturned)
        {
            if (investmentId <= 0)
                throw new ArgumentException("Invalid investment ID", nameof(investmentId));

            if (string.IsNullOrWhiteSpace(investorCNP))
                throw new ArgumentException("Investor CNP cannot be empty", nameof(investorCNP));

            var url = $"{BaseUrl}/{investmentId}?investorCNP={investorCNP}&amountReturned={amountReturned}";
            var response = await _httpClient.PutAsync(url, null);
            response.EnsureSuccessStatusCode();
        }
    }
} 