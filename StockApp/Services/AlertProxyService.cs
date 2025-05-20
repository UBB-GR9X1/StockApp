using Common.Models;
using Common.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace StockApp.Services
{
    public class AlertProxyService(HttpClient httpClient) : IProxyService, IAlertService
    {
        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<Alert> CreateAlertAsync(string stockName, string name, decimal upperBound, decimal lowerBound, bool toggleOnOff)
        {
            var alertData = new
            {
                StockName = stockName,
                Name = name,
                UpperBound = upperBound,
                LowerBound = lowerBound,
                ToggleOnOff = toggleOnOff
            };

            var response = await _httpClient.PostAsJsonAsync("api/Alert", alertData);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Alert>(_jsonOptions)
                ?? throw new InvalidOperationException("Could not deserialize the response body.");
        }

        public async Task<Alert?> GetAlertByIdAsync(int alertId)
        {
            var response = await _httpClient.GetAsync($"api/Alert/{alertId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Alert>(_jsonOptions);
        }

        public async Task<List<Alert>> GetAllAlertsAsync()
        {
            var response = await _httpClient.GetAsync("api/Alert");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Alert>>(_jsonOptions)
                ?? [];
        }

        public async Task<List<Alert>> GetAllAlertsOnAsync()
        {
            var response = await _httpClient.GetAsync("api/Alert/on");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Alert>>(_jsonOptions)
                ?? [];
        }

        public async Task RemoveAlertAsync(int alertId)
        {
            var response = await _httpClient.DeleteAsync($"api/Alert/{alertId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAlertAsync(Alert alert)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Alert/{alert.AlertId}", alert);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAlertAsync(int alertId, string stockName, string name, decimal upperBound, decimal lowerBound, bool toggleOnOff)
        {
            var alertData = new
            {
                StockName = stockName,
                Name = name,
                UpperBound = upperBound,
                LowerBound = lowerBound,
                ToggleOnOff = toggleOnOff
            };

            var response = await _httpClient.PutAsJsonAsync($"api/Alert/{alertId}", alertData);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<TriggeredAlert>> GetTriggeredAlertsAsync()
        {
            var response = await _httpClient.GetAsync("api/Alert/triggered");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<TriggeredAlert>>(_jsonOptions)
                ?? [];
        }
    }
}