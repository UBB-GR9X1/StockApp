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

        public List<Investment> GetInvestmentsHistory()
        {
            var response = _httpClient.GetFromJsonAsync<List<Investment>>(BaseUrl).Result;
            return response ?? new List<Investment>();
        }

        public void AddInvestment(Investment investment)
        {
            if (investment == null)
                throw new ArgumentNullException(nameof(investment));

            var response = _httpClient.PostAsJsonAsync(BaseUrl, investment).Result;
            response.EnsureSuccessStatusCode();
        }

        public void UpdateInvestment(int investmentId, string investorCNP, float amountReturned)
        {
            if (investmentId <= 0)
                throw new ArgumentException("Invalid investment ID", nameof(investmentId));

            if (string.IsNullOrWhiteSpace(investorCNP))
                throw new ArgumentException("Investor CNP cannot be empty", nameof(investorCNP));

            var url = $"{BaseUrl}/{investmentId}?investorCNP={investorCNP}&amountReturned={amountReturned}";
            var response = _httpClient.PutAsync(url, null).Result;
            response.EnsureSuccessStatusCode();
        }
    }
} 