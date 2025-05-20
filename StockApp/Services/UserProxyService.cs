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
    public class UserProxyService : IProxyService, IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

        public UserProxyService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task CreateUser(User user)
        {
            var response = await _httpClient.PostAsJsonAsync("api/User", user);
            response.EnsureSuccessStatusCode();
        }

        public async Task<User> GetCurrentUserAsync(string? userCNP = null) // userCNP is not used by the API endpoint for current user
        {
            // If userCNP is provided, it implies fetching a specific user, otherwise the current one.
            // However, the UserController has GetCurrentUser for the authenticated user (no CNP in path)
            // and GetUserByCnp for a specific user.
            // The IUserService interface's GetCurrentUserAsync(string? userCNP = null) is a bit ambiguous here.
            // Assuming if userCNP is null, we get the current authenticated user.
            // If userCNP is not null, we should call the specific user endpoint.
            // For now, sticking to the "current" user endpoint as per method name.
            // A dedicated GetUserByCnpAsync will handle the other case.
            var response = await _httpClient.GetAsync("api/User/current");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<User>(_options) ?? throw new InvalidOperationException("Failed to deserialize user.");
        }

        public async Task<int> GetCurrentUserGemsAsync(string? userCNP = null)
        {
            // This seems to map to StoreController's GetUserGemBalanceAsync
            // Assuming the API endpoint is "api/Store/user-gem-balance"
            // The userCNP parameter in IUserService.GetCurrentUserGemsAsync might be problematic if the API
            // always derives the user from the token for this specific call.
            // The StoreController's GetUserGemBalance uses the authenticated user's CNP.
            // If userCNP is provided and different, this proxy method might not behave as expected without API changes.
            // For now, we assume the API uses the authenticated user.
            var response = await _httpClient.GetAsync("api/Store/user-gem-balance");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>(_options);
        }

        public async Task<User> GetUserByCnpAsync(string cnp)
        {
            if (string.IsNullOrEmpty(cnp))
            {
                throw new ArgumentNullException(nameof(cnp));
            }

            var response = await _httpClient.GetAsync($"api/User/{cnp}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<User>(_options) ?? throw new InvalidOperationException("Failed to deserialize user.");
        }

        public async Task<List<User>> GetUsers()
        {
            var response = await _httpClient.GetAsync("api/User");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<User>>(_options) ?? new List<User>();
        }

        public async Task UpdateIsAdminAsync(bool newIsAdmin, string? userCNP = null)
        {
            if (string.IsNullOrEmpty(userCNP))
            {
                throw new ArgumentNullException(nameof(userCNP), "User CNP must be provided to update admin status.");
            }

            var payload = new { IsAdmin = newIsAdmin };
            var response = await _httpClient.PutAsJsonAsync($"api/User/{userCNP}/admin-status", payload);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateUserAsync(string newUsername, string newImage, string newDescription, bool newHidden, string? userCNP = null)
        {
            var payload = new { UserName = newUsername, Image = newImage, Description = newDescription, IsHidden = newHidden };
            HttpResponseMessage response;
            if (string.IsNullOrEmpty(userCNP))
            {
                // Update current user
                response = await _httpClient.PutAsJsonAsync("api/User/current", payload);
            }
            else
            {
                // Update user by CNP (typically admin action)
                response = await _httpClient.PutAsJsonAsync($"api/User/{userCNP}", payload);
            }
            response.EnsureSuccessStatusCode();
        }

        public async Task<int> AddDefaultRoleToAllUsersAsync()
        {
            var response = await _httpClient.PostAsync("api/User/add-default-role", null);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<DefaultRoleResponse>(_options);
            return result?.UpdatedCount ?? 0;
        }

        private class DefaultRoleResponse
        {
            public string? Message { get; set; }
            public int UpdatedCount { get; set; }
        }
    }
}
