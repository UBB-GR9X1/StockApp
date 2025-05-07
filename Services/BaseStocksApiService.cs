using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using StockApp.Models;
using StockApp.Repositories;

namespace StockApp.Services
{
    /// <summary>
    /// API client service that implements IBaseStocksRepository to make calls to the BankAPI
    /// </summary>
    public class BaseStocksApiService : IBaseStocksRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7001/api/BaseStocks";
        private readonly JsonSerializerOptions _jsonOptions;

        public BaseStocksApiService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        /// <inheritdoc/>
        public async Task<BaseStock> AddStockAsync(BaseStock stock, int initialPrice = 100)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUrl, stock);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<BaseStock>(_jsonOptions);
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to add stock. Status code: {response.StatusCode}, Error: {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while adding the stock to the API.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteStockAsync(string name)
        {
            try
            {
                var url = $"{_baseUrl}/{Uri.EscapeDataString(name)}";
                var response = await _httpClient.DeleteAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to delete stock. Status code: {response.StatusCode}, Error: {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while deleting the stock '{name}' from the API.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<List<BaseStock>> GetAllStocksAsync()
        {
            try
            {
                var stocks = await _httpClient.GetFromJsonAsync<List<BaseStock>>(_baseUrl, _jsonOptions);
                return stocks ?? new List<BaseStock>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while retrieving stocks from the API.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<BaseStock> GetStockByNameAsync(string name)
        {
            try
            {
                var url = $"{_baseUrl}/{Uri.EscapeDataString(name)}";
                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<BaseStock>(_jsonOptions);
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new KeyNotFoundException($"Stock with name '{name}' not found.");
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve stock. Status code: {response.StatusCode}, Error: {errorContent}");
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while retrieving the stock '{name}' from the API.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<BaseStock> UpdateStockAsync(BaseStock stock)
        {
            try
            {
                var url = $"{_baseUrl}/{Uri.EscapeDataString(stock.Name)}";
                var response = await _httpClient.PutAsJsonAsync(url, stock);
                
                if (response.IsSuccessStatusCode)
                {
                    return stock;
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new KeyNotFoundException($"Stock with name '{stock.Name}' not found.");
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to update stock. Status code: {response.StatusCode}, Error: {errorContent}");
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while updating the stock '{stock.Name}' in the API.", ex);
            }
        }
    }
} 