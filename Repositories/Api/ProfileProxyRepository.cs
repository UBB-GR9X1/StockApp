using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Repositories.Api
{
    public class ProfileProxyRepository : IProfileRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/Profile";

        public ProfileProxyRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> GenerateUsernameAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/username/random");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<string>() ?? throw new Exception("Failed to generate username");
        }

        public async Task UpdateMyUserAsync(string newUsername, string newImage, string newDescription, bool newHidden)
        {
            var profile = new ApiProfile
            {
                Cnp = IUserRepository.CurrentUserCNP,
                Name = newUsername,
                ProfilePicture = newImage,
                Description = newDescription,
                IsHidden = newHidden
            };

            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{profile.Cnp}", profile);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateRepoIsAdminAsync(bool isAdmin)
        {
            var cnp = IUserRepository.CurrentUserCNP;
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{cnp}/admin", isAdmin);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<Stock>> UserStocksAsync()
        {
            var cnp = IUserRepository.CurrentUserCNP;
            var response = await _httpClient.GetAsync($"{BaseUrl}/{cnp}/stocks");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Stock>>() ?? new List<Stock>();
        }

        private class ApiProfile
        {
            public string Cnp { get; set; } = string.Empty;

            public string Name { get; set; } = string.Empty;

            public string? ProfilePicture { get; set; }

            public string? Description { get; set; }

            public bool IsHidden { get; set; }

            public bool IsAdmin { get; set; }

            public int GemBalance { get; set; }

            public DateTime LastUpdated { get; set; }
        }

        private class ApiUserStock
        {
            public string UserCnp { get; set; } = string.Empty;

            public string StockName { get; set; } = string.Empty;

            public int Quantity { get; set; }
        }
    }
}