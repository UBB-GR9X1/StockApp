using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Repositories.Api
{
    internal class StockPageProxyRepository(HttpClient httpClient) : IStockPageRepository
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task AddOrUpdateUserStockAsync(string userCNP, string stockName, int quantity)
        {
            var response = await this._httpClient.PostAsJsonAsync("api/StockPage/AddOrUpdateUserStock", new { userCNP, stockName, quantity });
            response.EnsureSuccessStatusCode();
        }

        public async Task AddStockValueAsync(string stockName, int price)
        {
            var response = await this._httpClient.PostAsJsonAsync("api/StockPage/AddStockValue", new { stockName, price });
            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> GetFavoriteAsync(string userCNP, string stockName)
        {
            var result = await this._httpClient.GetFromJsonAsync<bool>($"api/StockPage/GetFavorite?userCNP={userCNP}&stockName={stockName}");
            return result;
        }

        public async Task<int> GetOwnedStocksAsync(string userCNP, string stockName)
        {
            var result = await this._httpClient.GetFromJsonAsync<int>($"api/StockPage/GetOwnedStocks?userCNP={userCNP}&stockName={stockName}");
            return result;
        }

        public async Task<Stock?> GetStockAsync(string userCNP, string stockName)
        {
            var result = await this._httpClient.GetFromJsonAsync<Stock>($"api/StockPage/GetStock?userCNP={userCNP}&stockName={stockName}");
            return result;
        }

        public async Task<List<int>> GetStockHistoryAsync(string stockName)
        {
            var result = await this._httpClient.GetFromJsonAsync<List<int>>($"api/StockPage/GetStockHistory?stockName={stockName}");
            return result ?? [];
        }

        public async Task ToggleFavoriteAsync(string userCNP, string stockName, bool state)
        {
            var response = await this._httpClient.PostAsJsonAsync("api/StockPage/ToggleFavorite", new { userCNP, stockName, state });
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateUserGemsAsync(string userCNP, int newGemBalance)
        {
            var response = await this._httpClient.PostAsJsonAsync("api/StockPage/UpdateUserGems", new { userCNP, newGemBalance });
            response.EnsureSuccessStatusCode();
        }
    }
}
