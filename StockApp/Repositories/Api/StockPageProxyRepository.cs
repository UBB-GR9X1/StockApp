using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Common.Models;

namespace StockApp.Repositories.Api
{
    internal class StockPageProxyRepository(HttpClient httpClient) : IStockPageRepository
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task AddOrUpdateUserStockAsync(string userCNP, string stockName, int quantity)
        {
            var response = await this._httpClient.PostAsJsonAsync("api/StockPage/UserStock", new { userCNP, stockName, quantity });
            response.EnsureSuccessStatusCode();
        }

        public async Task AddStockValueAsync(string stockName, int price)
        {
            var response = await this._httpClient.PostAsJsonAsync("api/StockPage/StockValue", new { stockName, price });
            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> GetFavoriteAsync(string userCNP, string stockName)
        {
            var result = await this._httpClient.GetFromJsonAsync<bool>($"api/StockPage/Favorite?userCNP={userCNP}&stockName={stockName}");
            return result;
        }

        public async Task<int> GetOwnedStocksAsync(string userCNP, string stockName)
        {
            var result = await this._httpClient.GetFromJsonAsync<int>($"api/StockPage/OwnedStocks?userCNP={userCNP}&stockName={stockName}");
            return result;
        }

        public async Task<Stock?> GetStockAsync(string stockName)
        {
            var result = await this._httpClient.GetFromJsonAsync<Stock>($"api/StockPage/Stock?stockName={stockName}");
            return result;
        }

        public async Task<UserStock?> GetUserStockAsync(string userCNP, string stockName)
        {
            var result = await this._httpClient.GetFromJsonAsync<UserStock>($"api/StockPage/UserStock?userCNP={userCNP}&stockName={stockName}");
            return result;
        }

        public async Task<List<int>> GetStockHistoryAsync(string stockName)
        {
            var result = await this._httpClient.GetFromJsonAsync<List<int>>($"api/StockPage/StockHistory?stockName={stockName}");
            return result ?? [];
        }

        public async Task ToggleFavoriteAsync(string userCNP, string stockName, bool state)
        {
            var response = await this._httpClient.PostAsJsonAsync("api/StockPage/ToggleFavorite", new { userCNP, stockName, state });
            response.EnsureSuccessStatusCode();
        }
    }
}
