using Common.Models;
using Common.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace StockApp.Services
{
    public class TipsProxyService(HttpClient httpClient) : IProxyService, ITipsService
    {
        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        public async Task GiveTipToUserAsync(string userCNP)
        {
            if (string.IsNullOrEmpty(userCNP))
            {
                throw new ArgumentException("User CNP cannot be empty", nameof(userCNP));
            }

            var response = await _httpClient.PostAsync($"api/Tips/user/{userCNP}/give", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<Tip>> GetTipsForUserAsync(string userCnp)
        {
            // If userCnp is provided, get tips for the specified user (admin only)
            if (!string.IsNullOrEmpty(userCnp))
            {
                return await _httpClient.GetFromJsonAsync<List<Tip>>($"api/Tips/user/{userCnp}") ??
                    throw new InvalidOperationException("Failed to deserialize tips for user response.");
            }

            // If no userCnp is provided, get tips for the current user
            return await _httpClient.GetFromJsonAsync<List<Tip>>("api/Tips/user") ??
                throw new InvalidOperationException("Failed to deserialize tips response.");
        }
    }
}