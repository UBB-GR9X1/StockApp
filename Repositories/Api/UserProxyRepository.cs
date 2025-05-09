using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Repositories.Api
{
    public class UserProxyRepository : IUserRepository
    {
        private readonly HttpClient _httpClient;

        public UserProxyRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<User>>("api/User") ?? new List<User>();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<User>($"api/User/{id}");
        }

        public async Task<User?> GetByCnpAsync(string cnp)
        {
            return await _httpClient.GetFromJsonAsync<User>($"api/User/cnp/{cnp}");
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _httpClient.GetFromJsonAsync<User>($"api/User/username/{username}");
        }

        public async Task<bool> CreateAsync(User user)
        {
            var response = await _httpClient.PostAsJsonAsync("api/User", user);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, User user)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/User/{id}", user);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/User/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
