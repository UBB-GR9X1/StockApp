namespace StockApp.Services.Api
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Src.Model;
    using StockApp.Models;

    public class ChatReportApiService
    {
        private readonly HttpClient _httpClient;

        public ChatReportApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ChatReport>> GetReportsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ChatReport>>("api/ChatReport") ?? new List<ChatReport>();
        }

        public async Task<ChatReport?> GetReportByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ChatReport>($"api/ChatReport/{id}");
            }
            catch (HttpRequestException)
            {
                return null; // Optionally log the error or rethrow
            }
        }

        public async Task<bool> CreateReportAsync(ChatReport report)
        {
            var response = await _httpClient.PostAsJsonAsync("api/ChatReport", report);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteReportAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/ChatReport/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
