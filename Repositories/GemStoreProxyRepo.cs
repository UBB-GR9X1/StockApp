using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Repositories
{
    public class GemStoreProxyRepo : IGemStoreRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7001/api/GemStore";

        public GemStoreProxyRepo(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> GetCnpAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/cnp");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<int> GetUserGemBalanceAsync(string cnp)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/balance/{cnp}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task UpdateUserGemBalanceAsync(string cnp, int newBalance)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/balance/{cnp}", newBalance);
            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> IsGuestAsync(string cnp)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/isguest/{cnp}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }
    }
} 