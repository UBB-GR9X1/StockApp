using Common.Models;
using Common.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace StockApp.Services
{
    public class HistoryProxyService(HttpClient httpClient) : IProxyService, IHistoryService
    {
        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        public async Task<List<CreditScoreHistory>> GetAllHistoryAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<CreditScoreHistory>>("api/History") ??
                throw new InvalidOperationException("Failed to deserialize credit score history response.");
        }

        public async Task<CreditScoreHistory?> GetHistoryByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<CreditScoreHistory>($"api/History/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task AddHistoryAsync(CreditScoreHistory history)
        {
            var response = await _httpClient.PostAsJsonAsync("api/History", history);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateHistoryAsync(CreditScoreHistory history)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/History/{history.Id}", history);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteHistoryAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/History/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<CreditScoreHistory>> GetHistoryForUserAsync(string userCnp)
        {
            // If userCnp is provided, get history for the specified user (admin only)
            if (!string.IsNullOrEmpty(userCnp))
            {
                return await _httpClient.GetFromJsonAsync<List<CreditScoreHistory>>($"api/History/user/{userCnp}") ??
                    throw new InvalidOperationException("Failed to deserialize user credit score history response.");
            }

            // If no userCnp is provided, get history for the current user
            return await _httpClient.GetFromJsonAsync<List<CreditScoreHistory>>("api/History/user") ??
                throw new InvalidOperationException("Failed to deserialize credit score history response.");
        }

        public async Task<List<CreditScoreHistory>> GetHistoryWeeklyAsync(string userCnp)
        {
            // If userCnp is provided, get weekly history for the specified user (admin only)
            if (!string.IsNullOrEmpty(userCnp))
            {
                return await _httpClient.GetFromJsonAsync<List<CreditScoreHistory>>($"api/History/user/{userCnp}/weekly") ??
                    throw new InvalidOperationException("Failed to deserialize weekly credit score history response.");
            }

            // If no userCnp is provided, get weekly history for the current user
            return await _httpClient.GetFromJsonAsync<List<CreditScoreHistory>>("api/History/user/weekly") ??
                throw new InvalidOperationException("Failed to deserialize weekly credit score history response.");
        }

        public async Task<List<CreditScoreHistory>> GetHistoryMonthlyAsync(string userCnp)
        {
            // If userCnp is provided, get monthly history for the specified user (admin only)
            if (!string.IsNullOrEmpty(userCnp))
            {
                return await _httpClient.GetFromJsonAsync<List<CreditScoreHistory>>($"api/History/user/{userCnp}/monthly") ??
                    throw new InvalidOperationException("Failed to deserialize monthly credit score history response.");
            }

            // If no userCnp is provided, get monthly history for the current user
            return await _httpClient.GetFromJsonAsync<List<CreditScoreHistory>>("api/History/user/monthly") ??
                throw new InvalidOperationException("Failed to deserialize monthly credit score history response.");
        }

        public async Task<List<CreditScoreHistory>> GetHistoryYearlyAsync(string userCnp)
        {
            // If userCnp is provided, get yearly history for the specified user (admin only)
            if (!string.IsNullOrEmpty(userCnp))
            {
                return await _httpClient.GetFromJsonAsync<List<CreditScoreHistory>>($"api/History/user/{userCnp}/yearly") ??
                    throw new InvalidOperationException("Failed to deserialize yearly credit score history response.");
            }

            // If no userCnp is provided, get yearly history for the current user
            return await _httpClient.GetFromJsonAsync<List<CreditScoreHistory>>("api/History/user/yearly") ??
                throw new InvalidOperationException("Failed to deserialize yearly credit score history response.");
        }
    }
}