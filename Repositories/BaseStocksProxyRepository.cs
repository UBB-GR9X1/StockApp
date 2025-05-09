using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Repositories
{
    /// <summary>
    /// Proxy repository that implements <see cref="IBaseStocksRepository"/> and
    /// talks to the Bank API over HTTP.
    /// </summary>
    public class BaseStocksProxyRepository : IBaseStocksRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/BaseStocks";
        private readonly JsonSerializerOptions _jsonOptions;

        public BaseStocksProxyRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        /*─────────────────────────  CREATE  ─────────────────────────*/

        public async Task AddStockAsync(BaseStock stock, int initialPrice = 100)
        {
            try
            {
                // The API ignores initialPrice, mirroring BaseStocksApiService.
                var response = await _httpClient
                    .PostAsJsonAsync(_baseUrl, stock);

                if (!response.IsSuccessStatusCode)
                {
                    var error = response.Content.ReadAsStringAsync()
                                                .GetAwaiter()
                                                .GetResult();
                    throw new Exception(
                        $"Failed to add stock. Status code: {response.StatusCode}, Error: {error}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding stock: {ex.Message}");
                throw new Exception("Error occurred while adding stock to the API.", ex);
            }
        }

        /*─────────────────────────  READ  ──────────────────────────*/

        public async Task<List<BaseStock>> GetAllStocksAsync()
        {
            try
            {
                var stocks = await _httpClient
                             .GetFromJsonAsync<List<BaseStock>>(_baseUrl, _jsonOptions);

                return stocks ?? [];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting all stocks: {ex.Message}");
                throw new Exception("Error occurred while retrieving stocks from the API.", ex);
            }
        }
    }
}
