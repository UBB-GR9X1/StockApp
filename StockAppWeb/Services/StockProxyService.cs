using Common.Models;
using Common.Services;
using System.Text.Json;

namespace StockAppWeb.Services
{
    public class StockProxyService : IProxyService, IStockService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

        public StockProxyService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<Stock> CreateStockAsync(Stock stock)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Stock", stock);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Stock>(_options) ?? throw new InvalidOperationException("Failed to deserialize stock response.");
        }

        public async Task<bool> DeleteStockAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Stock/{id}");
            response.EnsureSuccessStatusCode();
            return response.StatusCode == System.Net.HttpStatusCode.NoContent || await response.Content.ReadFromJsonAsync<bool>(_options);
        }

        public async Task<IEnumerable<Stock>> GetAllStocksAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Stock>>("api/Stock", _options) ?? throw new InvalidOperationException("Failed to deserialize stocks response.");
        }

        public async Task<Stock?> GetStockByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Stock?>($"api/Stock/{id}", _options);
        }

        public async Task<Stock?> GetStockByNameAsync(string name)
        {
            var allStocks = await this.GetAllStocksAsync();
            return allStocks.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Stock?> UpdateStockAsync(int id, Stock updatedStock)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Stock/{id}", updatedStock);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Stock?>(_options);
        }

        public async Task<List<Stock>> UserStocksAsync(string? userCNP = null)
        {
            string endpoint = string.IsNullOrEmpty(userCNP) ? "api/Stock/user" : $"api/Stock/user/{userCNP}";
            return await _httpClient.GetFromJsonAsync<List<Stock>>(endpoint, _options) ?? [];
        }

        public async Task AddToFavoritesAsync(HomepageStock stock)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Stock/favorites/add", stock);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveFromFavoritesAsync(HomepageStock stock)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Stock/favorites/remove", stock);
            response.EnsureSuccessStatusCode();
        }
        
        public async Task<List<HomepageStock>> GetFilteredAndSortedStocksAsync(string filterColumn, string filterValue, bool sortAscending, string? userCNP = null)
        {
            string query = $"api/Stock/filter?filterColumn={Uri.EscapeDataString(filterColumn)}&filterValue={Uri.EscapeDataString(filterValue)}&sortAscending={sortAscending}";
            
            if (!string.IsNullOrEmpty(userCNP))
            {
                query += $"&userCNP={Uri.EscapeDataString(userCNP)}";
            }
            
            return await _httpClient.GetFromJsonAsync<List<HomepageStock>>(query, _options) 
                ?? throw new InvalidOperationException("Failed to deserialize filtered stocks response.");
        }
    }
} 