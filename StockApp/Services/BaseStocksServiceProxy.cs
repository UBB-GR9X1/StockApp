/*
 * using Common.Models;
using Common.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace StockApp.Services
{
    public class BaseStocksProxyService : IBaseStocksService // This should now resolve correctly
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthenticationService _authenticationService;

        public BaseStocksProxyService(HttpClient httpClient, IAuthenticationService authenticationService)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        private async Task AddAuthorizationHeader()
        {
            var token = _authenticationService.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                throw new InvalidOperationException("User is not authenticated.");
            }
        }

        public async Task<BaseStock> AddStockAsync(BaseStock stock, int initialPrice = 100)
        {
            await AddAuthorizationHeader();
            var stockDto = new { Name = stock.Name, Symbol = stock.Symbol, InitialPrice = initialPrice };
            var response = await _httpClient.PostAsJsonAsync("api/BaseStocks", stockDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<BaseStock>();
        }

        public async Task<bool> DeleteStockAsync(string name)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/BaseStocks/{name}");
            response.EnsureSuccessStatusCode();
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<List<BaseStock>> GetAllStocksAsync()
        {
            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<List<BaseStock>>("api/BaseStocks");
        }

        public async Task<BaseStock> GetStockByNameAsync(string name)
        {
            await AddAuthorizationHeader();
            return await _httpClient.GetFromJsonAsync<BaseStock>($"api/BaseStocks/{name}");
        }

        public async Task<BaseStock> UpdateStockAsync(BaseStock stock)
        {
            await AddAuthorizationHeader();
            var updateDto = new { Name = stock.Name, Symbol = stock.Symbol };
            var response = await _httpClient.PutAsJsonAsync($"api/BaseStocks/{stock.Name}", updateDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<BaseStock>();
        }
    }
}*/