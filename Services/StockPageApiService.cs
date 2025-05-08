using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Services
{
    public class StockPageApiService : IStockPageService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private Stock _selectedStock;

        public StockPageApiService(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl;
        }

        public void SelectStock(Stock stock)
        {
            _selectedStock = stock;
        }

        public bool IsGuest()
        {
            try
            {
                var response = _httpClient.GetAsync($"{_baseUrl}/api/stockpage/isguest").Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadFromJsonAsync<bool>().Result;
            }
            catch
            {
                return true;
            }
        }

        public string GetStockName()
        {
            return _selectedStock?.Name;
        }

        public string GetStockSymbol()
        {
            return _selectedStock?.Symbol;
        }

        public bool GetFavorite()
        {
            if (_selectedStock == null) return false;
            
            try
            {
                return GetFavoriteAsync(_selectedStock.Name).Result;
            }
            catch
            {
                return false;
            }
        }

        public int GetUserBalance()
        {
            try
            {
                return GetUserBalanceAsync().Result;
            }
            catch
            {
                return 0;
            }
        }

        public int GetOwnedStocks()
        {
            if (_selectedStock == null) return 0;
            
            try
            {
                return GetOwnedStocksAsync().Result;
            }
            catch
            {
                return 0;
            }
        }

        public List<int> GetStockHistory()
        {
            if (_selectedStock == null) return new List<int>();
            
            try
            {
                return GetStockHistoryAsync().Result;
            }
            catch
            {
                return new List<int>();
            }
        }

        public User GetStockAuthor()
        {
            if (_selectedStock == null) return null;
            
            try
            {
                return GetStockAuthorAsync().Result;
            }
            catch
            {
                return null;
            }
        }

        public bool BuyStock(int quantity)
        {
            if (_selectedStock == null) return false;
            
            try
            {
                return BuyStockAsync(quantity).Result;
            }
            catch
            {
                return false;
            }
        }

        public bool SellStock(int quantity)
        {
            if (_selectedStock == null) return false;
            
            try
            {
                return SellStockAsync(quantity).Result;
            }
            catch
            {
                return false;
            }
        }

        public void ToggleFavorite(bool state)
        {
            if (_selectedStock == null) return;
            
            try
            {
                ToggleFavoriteAsync(state).Wait();
            }
            catch
            {
                // Handle error silently
            }
        }

        public async Task<int> GetUserBalanceAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/stockpage/balance");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<int>();
            }
            catch
            {
                return 0;
            }
        }

        public async Task<int> GetOwnedStocksAsync()
        {
            if (_selectedStock == null) return 0;
            
            try
            {
                return await GetOwnedStocksAsync(_selectedStock.Name);
            }
            catch
            {
                return 0;
            }
        }

        public async Task<List<int>> GetStockHistoryAsync()
        {
            if (_selectedStock == null) return new List<int>();
            
            try
            {
                return await GetStockHistoryAsync(_selectedStock.Name);
            }
            catch
            {
                return new List<int>();
            }
        }

        public async Task<User> GetStockAuthorAsync()
        {
            if (_selectedStock == null) return null;
            
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/stockpage/stocks/{_selectedStock.Name}/author");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<User>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> BuyStockAsync(int quantity)
        {
            if (_selectedStock == null) return false;
            
            try
            {
                await AddOrUpdateUserStockAsync(_selectedStock.Name, quantity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SellStockAsync(int quantity)
        {
            if (_selectedStock == null) return false;
            
            try
            {
                await AddOrUpdateUserStockAsync(_selectedStock.Name, -quantity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task ToggleFavoriteAsync(bool state)
        {
            if (_selectedStock == null) return;
            
            try
            {
                await ToggleFavoriteAsync(_selectedStock.Name, state);
            }
            catch
            {
                // Handle error silently
            }
        }

        public async Task<User> GetUserAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/stockpage/user");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<User>();
        }

        public async Task UpdateUserGemsAsync(int gems)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/api/stockpage/gems", gems);
            response.EnsureSuccessStatusCode();
        }

        public async Task AddStockValueAsync(string stockName, int price)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/stockpage/stocks/{stockName}/value", price);
            response.EnsureSuccessStatusCode();
        }

        public async Task<Stock> GetStockAsync(string stockName)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/stockpage/stocks/{stockName}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Stock>();
        }

        // Helper methods for API calls
        private async Task AddOrUpdateUserStockAsync(string stockName, int quantity)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/stockpage/stocks/{stockName}/quantity", quantity);
            response.EnsureSuccessStatusCode();
        }

        private async Task<List<int>> GetStockHistoryAsync(string stockName)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/stockpage/stocks/{stockName}/history");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<int>>();
        }

        private async Task<int> GetOwnedStocksAsync(string stockName)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/stockpage/stocks/{stockName}/owned");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }

        private async Task<bool> GetFavoriteAsync(string stockName)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/stockpage/stocks/{stockName}/favorite");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        private async Task ToggleFavoriteAsync(string stockName, bool state)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/api/stockpage/stocks/{stockName}/favorite", state);
            response.EnsureSuccessStatusCode();
        }
    }
} 