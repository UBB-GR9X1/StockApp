using Common.Models;
using Common.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace StockApp.Services
{
    public class ChatReportProxyService(HttpClient httpClient) : IProxyService, IChatReportService
    {
        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        public async Task<List<ChatReport>> GetAllChatReportsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ChatReport>>("api/ChatReport") ??
                throw new InvalidOperationException("Failed to deserialize chat reports response.");
        }

        public async Task<ChatReport?> GetChatReportByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ChatReport>($"api/ChatReport/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task AddChatReportAsync(ChatReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report), "Chat report cannot be null");
            }

            var response = await _httpClient.PostAsJsonAsync("api/ChatReport", report);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteChatReportAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/ChatReport/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<int> GetNumberOfGivenTipsForUserAsync(string? userCnp = null)
        {
            string endpoint = string.IsNullOrEmpty(userCnp)
                ? "api/ChatReport/user-tips/current"
                : $"api/ChatReport/user-tips/{userCnp}";

            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task UpdateActivityLogAsync(int amount, string? userCnp = null)
        {
            var updateDto = new ActivityLogUpdateDto { Amount = amount };
            var response = await _httpClient.PostAsJsonAsync("api/ChatReport/activity-log", updateDto);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateScoreHistoryForUserAsync(int newScore, string? userCnp = null)
        {
            var updateDto = new ScoreHistoryUpdateDto { NewScore = newScore };
            var response = await _httpClient.PostAsJsonAsync("api/ChatReport/score-history", updateDto);
            response.EnsureSuccessStatusCode();
        }
    }

    // DTOs needed for API calls
    internal class ActivityLogUpdateDto
    {
        public int Amount { get; set; }
    }

    internal class ScoreHistoryUpdateDto
    {
        public int NewScore { get; set; }
    }
}