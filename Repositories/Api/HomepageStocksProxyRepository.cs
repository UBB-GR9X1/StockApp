using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using StockApp.Models;
using StockApp.Repositories; // <-- Interface is here

namespace StockApp.Repositories.Api
{
    public class HomepageStocksProxyRepository : IHomepageStocksProxyRepository
    {
        private readonly HttpClient _httpClient;

        public HomepageStocksProxyRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<HomepageStock>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<HomepageStock>>("api/HomepageStocks") ?? new List<HomepageStock>();
        }

        public async Task<HomepageStock?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<HomepageStock>($"api/HomepageStocks/{id}");
        }

        public async Task<HomepageStock?> GetBySymbolAsync(string symbol)
        {
            return await _httpClient.GetFromJsonAsync<HomepageStock>($"api/HomepageStocks/symbol/{symbol}");
        }

        public async Task<bool> CreateAsync(HomepageStock homepageStock)
        {
            var response = await _httpClient.PostAsJsonAsync("api/HomepageStocks", homepageStock);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, HomepageStock homepageStock)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/HomepageStocks/{id}", homepageStock);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/HomepageStocks/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
