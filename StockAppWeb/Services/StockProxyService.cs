using Common.Models;
using Common.Services;
using System.Net.Http.Json;

namespace StockAppWeb.Services
{
    public class StockProxyService : IStockService
    {
        private readonly HttpClient _httpClient;

        public StockProxyService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<Stock> CreateStockAsync(Stock stock)
        {
            var response = await _httpClient.PostAsJsonAsync("api/stock", stock);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Stock>() ?? throw new InvalidOperationException("Failed to deserialize response");
        }

        public async Task<bool> DeleteStockAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/stock/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<Stock>> GetAllStocksAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Stock>>("api/stock") ?? Array.Empty<Stock>();
        }

        public async Task<Stock?> GetStockByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Stock>($"api/stock/{id}");
        }

        public async Task<Stock?> UpdateStockAsync(int id, Stock stock)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/stock/{id}", stock);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Stock>();
        }

        public async Task<List<Stock>> UserStocksAsync(string? userId = null)
        {
            var url = userId == null ? "api/stock/user" : $"api/stock/user/{userId}";
            var stocks = await _httpClient.GetFromJsonAsync<IEnumerable<Stock>>(url) ?? Array.Empty<Stock>();
            return stocks.ToList();
        }

        public async Task AddToFavoritesAsync(HomepageStock stock)
        {
            var response = await _httpClient.PostAsJsonAsync("api/stock/favorites", stock);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveFromFavoritesAsync(HomepageStock stock)
        {
            var response = await _httpClient.DeleteAsync($"api/stock/favorites/{stock.Id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<HomepageStock>> GetFilteredAndSortedStocksAsync(string searchTerm, string sortBy, bool ascending, string? userId = null)
        {
            var url = $"api/stock/filter?searchTerm={searchTerm}&sortBy={sortBy}&ascending={ascending}";
            if (userId != null)
            {
                url += $"&userId={userId}";
            }
            var stocks = await _httpClient.GetFromJsonAsync<IEnumerable<HomepageStock>>(url) ?? Array.Empty<HomepageStock>();
            return stocks.ToList();
        }
    }
} 