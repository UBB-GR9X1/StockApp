using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Services
{
    public class BaseStocksApiService : IBaseStocksApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public BaseStocksApiService(HttpClient httpClient, string baseUrl = "api/BaseStocks")
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _baseUrl = baseUrl;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<List<BaseStock>> GetAllStocksAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadFromJsonAsync<List<BaseStock>>() 
                    ?? new List<BaseStock>();
            }
            catch (HttpRequestException ex)
            {
                throw new ApiException("Failed to fetch stocks from API", ex);
            }
        }

        public async Task<BaseStock> GetStockByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Stock name cannot be null or empty.", nameof(name));
            }

            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{name}");
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadFromJsonAsync<BaseStock>() 
                    ?? throw new ApiException($"Failed to deserialize stock '{name}' from API response");
            }
            catch (HttpRequestException ex)
            {
                throw new ApiException($"Failed to fetch stock '{name}' from API", ex);
            }
        }

        public async Task<BaseStock> CreateStockAsync(BaseStock stock, int? initialPrice = null)
        {
            if (stock == null)
            {
                throw new ArgumentNullException(nameof(stock));
            }

            try
            {
                var dto = new
                {
                    Name = stock.Name,
                    Symbol = stock.Symbol,
                    AuthorCNP = stock.AuthorCNP,
                    InitialPrice = initialPrice
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(dto, _jsonOptions),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(_baseUrl, content);
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadFromJsonAsync<BaseStock>()
                    ?? throw new ApiException("Failed to deserialize created stock from API response");
            }
            catch (HttpRequestException ex)
            {
                throw new ApiException("Failed to create stock via API", ex);
            }
        }

        public async Task<BaseStock> UpdateStockAsync(BaseStock stock)
        {
            if (stock == null)
            {
                throw new ArgumentNullException(nameof(stock));
            }

            try
            {
                var dto = new
                {
                    Name = stock.Name,
                    Symbol = stock.Symbol,
                    AuthorCNP = stock.AuthorCNP
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(dto, _jsonOptions),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PutAsync($"{_baseUrl}/{stock.Name}", content);
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadFromJsonAsync<BaseStock>()
                    ?? throw new ApiException($"Failed to deserialize updated stock '{stock.Name}' from API response");
            }
            catch (HttpRequestException ex)
            {
                throw new ApiException($"Failed to update stock '{stock.Name}' via API", ex);
            }
        }

        public async Task<bool> DeleteStockAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Stock name cannot be null or empty.", nameof(name));
            }

            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{name}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }
                
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                throw new ApiException($"Failed to delete stock '{name}' via API", ex);
            }
        }
    }

    public class ApiException : Exception
    {
        public ApiException(string message) : base(message)
        {
        }

        public ApiException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
} 