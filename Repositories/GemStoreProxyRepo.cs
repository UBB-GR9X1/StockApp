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
        private readonly string _baseUrl = "https://localhost:7001/api/GemStore";
        private readonly JsonSerializerOptions _jsonOptions;

        public GemStoreProxyRepo(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<string> GetCnpAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/cnp");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while retrieving CNP from the API.", ex);
            }
        }

        public async Task<int> GetUserGemBalanceAsync(string cnp)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/balance/{cnp}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<int>(_jsonOptions);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while retrieving gem balance for CNP {cnp} from the API.", ex);
            }
        }

        public async Task UpdateUserGemBalanceAsync(string cnp, int newBalance)
        {
            try
            {
                var updateRequest = new { Cnp = cnp, NewBalance = newBalance };
                var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/balance", updateRequest, _jsonOptions);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while updating gem balance for CNP {cnp} in the API.", ex);
            }
        }

        public async Task<bool> IsGuestAsync(string cnp)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/guest/{cnp}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<bool>(_jsonOptions);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while checking guest status for CNP {cnp} from the API.", ex);
            }
        }
    }
} 