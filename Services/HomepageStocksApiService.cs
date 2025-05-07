namespace StockApp.Services
{
    using StockApp.Models;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    public class HomepageStocksApiService : IHomepageStocksApiService
    {
        private readonly HttpClient _httpClient;

        public HomepageStocksApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<HomepageStock>> GetAllStocksAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<HomepageStock>>("api/HomepageStocks");
            return result ?? new List<HomepageStock>();
        }

        public async Task<HomepageStock> GetStockByIdAsync(int id)
        {
            var result = await _httpClient.GetFromJsonAsync<HomepageStock>($"api/HomepageStocks/{id}");
            return result ?? throw new KeyNotFoundException("Stock not found.");
        }

        public async Task<bool> AddStockAsync(HomepageStock stock)
        {
            var response = await _httpClient.PostAsJsonAsync("api/HomepageStocks", stock);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateStockAsync(HomepageStock stock)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/HomepageStocks/{stock.Id}", stock);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteStockAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/HomepageStocks/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
