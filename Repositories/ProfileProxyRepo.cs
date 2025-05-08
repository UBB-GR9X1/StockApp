using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using StockApp.Models;

namespace StockApp.Repositories
{
    public class ProfileProxyRepo : IProfileRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7001/api/Profile";

        public ProfileProxyRepo(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public User CurrentUser()
        {
            throw new NotImplementedException("This method should be handled by the UserService");
        }

        public string GenerateUsername()
        {
            var response = _httpClient.GetAsync($"{BaseUrl}/username/random").Result;
            response.EnsureSuccessStatusCode();
            return response.Content.ReadFromJsonAsync<string>().Result ?? throw new Exception("Failed to generate username");
        }

        public User GetUserProfile(string authorCnp)
        {
            var response = _httpClient.GetAsync($"{BaseUrl}/{authorCnp}").Result;
            response.EnsureSuccessStatusCode();
            var profile = response.Content.ReadFromJsonAsync<ApiProfile>().Result ?? throw new Exception("Failed to get profile");
            
            return new User(
                cnp: profile.Cnp,
                username: profile.Name,
                description: profile.Description,
                isModerator: profile.IsAdmin,
                image: profile.ProfilePicture,
                isHidden: profile.IsHidden,
                gem_balance: profile.GemBalance);
        }

        public void UpdateMyUser(string newUsername, string newImage, string newDescription, bool newHidden)
        {
            var profile = new ApiProfile
            {
                Cnp = _httpClient.DefaultRequestHeaders.GetValues("X-User-CNP").First(),
                Name = newUsername,
                ProfilePicture = newImage,
                Description = newDescription,
                IsHidden = newHidden
            };

            var response = _httpClient.PutAsJsonAsync($"{BaseUrl}/{profile.Cnp}", profile).Result;
            response.EnsureSuccessStatusCode();
        }

        public void UpdateRepoIsAdmin(bool isAdmin)
        {
            var cnp = _httpClient.DefaultRequestHeaders.GetValues("X-User-CNP").First();
            var response = _httpClient.PutAsJsonAsync($"{BaseUrl}/{cnp}/admin", isAdmin).Result;
            response.EnsureSuccessStatusCode();
        }

        public List<Stock> UserStocks()
        {
            var cnp = _httpClient.DefaultRequestHeaders.GetValues("X-User-CNP").First();
            var response = _httpClient.GetAsync($"{BaseUrl}/{cnp}/stocks").Result;
            response.EnsureSuccessStatusCode();
            var apiStocks = response.Content.ReadFromJsonAsync<List<ApiStock>>().Result ?? new List<ApiStock>();
            
            return apiStocks.Select(s => new Stock(
                name: s.Name,
                symbol: s.Symbol,
                authorCNP: s.AuthorCnp,
                price: s.CurrentPrice,
                quantity: s.UserStocks.FirstOrDefault()?.Quantity ?? 0
            )).ToList();
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

        private class ApiStock
        {
            public string Name { get; set; } = string.Empty;
            public string Symbol { get; set; } = string.Empty;
            public int CurrentPrice { get; set; }
            public string AuthorCnp { get; set; } = string.Empty;
            public List<ApiUserStock> UserStocks { get; set; } = new();
        }

        private class ApiUserStock
        {
            public string UserCnp { get; set; } = string.Empty;
            public string StockName { get; set; } = string.Empty;
            public int Quantity { get; set; }
        }
    }
} 