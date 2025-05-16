using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Common.Models;

namespace StockApp.Repositories.Api
{
    internal class StockProxyRepository : IStockRepository
    {
        private const string BaseUrl = "api/Stock";
        private readonly HttpClient _httpClient;

        public StockProxyRepository(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task<Stock> CreateAsync(Stock stock)
        {
            var response = await this._httpClient.PostAsJsonAsync(BaseUrl, stock);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Stock>();
            return result ?? throw new InvalidOperationException("The response content is null.");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await this._httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<Stock>> GetAllAsync()
        {
            var response = await this._httpClient.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<Stock>>();
            return result ?? throw new InvalidOperationException("The response content is null.");
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            var response = await this._httpClient.GetAsync($"{BaseUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Stock>();
                return result ?? throw new InvalidOperationException("The response content is null.");
            }

            return null;
        }

        public async Task<Stock?> UpdateAsync(int id, Stock updatedStock)
        {
            var response = await this._httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", updatedStock);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Stock>();
                return result ?? throw new InvalidOperationException("The response content is null.");
            }

            return null;
        }

        public async Task<List<Stock>> UserStocksAsync(string cnp)
        {
            if (string.IsNullOrEmpty(cnp))
            {
                throw new ArgumentException("CNP cannot be null or empty.", nameof(cnp));
            }

            var response = await this._httpClient.GetAsync($"{BaseUrl}/user/{cnp}");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<Stock>>();
            return result ?? throw new InvalidOperationException("The response content is null.");
        }
    }
}
