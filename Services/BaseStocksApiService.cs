namespace StockApp.Services
{
    using StockApp.Models;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    public class BaseStocksApiService : IBaseStocksApiService
    {
        private readonly HttpClient _httpClient;

        public BaseStocksApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<BaseStock>> GetAllStocksAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<BaseStock>>("api/BaseStocks");
            return result ?? new List<BaseStock>();
        }

        public async Task<BaseStock> GetStockByNameAsync(string name)
        {
            var result = await _httpClient.GetFromJsonAsync<BaseStock>($"api/BaseStocks/{Uri.EscapeDataString(name)}");
            return result ?? throw new KeyNotFoundException($"Stock with name '{name}' not found.");
        }

        public async Task<bool> AddStockAsync(BaseStock stock, int initialPrice = 100)
        {
            var response = await _httpClient.PostAsJsonAsync("api/BaseStocks", stock);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateStockAsync(BaseStock stock)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/BaseStocks/{Uri.EscapeDataString(stock.Name)}", stock);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteStockAsync(string name)
        {
            var response = await _httpClient.DeleteAsync($"api/BaseStocks/{Uri.EscapeDataString(name)}");
            return response.IsSuccessStatusCode;
        }
    }
} 