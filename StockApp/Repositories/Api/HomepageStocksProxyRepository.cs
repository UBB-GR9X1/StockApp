namespace StockApp.Repositories.Api
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Common.Models;

    public class HomepageStocksProxyRepository : IHomepageStocksRepository
    {
        private readonly HttpClient _httpClient;

        public HomepageStocksProxyRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<HomepageStock>> GetAllStocksAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<HomepageStock>>($"api/HomepageStock?userCNP={IUserRepository.CurrentUserCNP}");
            return result ?? new List<HomepageStock>();
        }

        public async Task<HomepageStock> GetStockByIdAsync(int id)
        {
            var result = await _httpClient.GetFromJsonAsync<HomepageStock>($"api/HomepageStock/{id}?userCNP={IUserRepository.CurrentUserCNP}");
            return result ?? throw new KeyNotFoundException("Stock not found.");
        }

        public async Task<bool> AddStockAsync(HomepageStock stock)
        {
            var response = await _httpClient.PostAsJsonAsync("api/HomepageStock", stock);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateStockAsync(int id, HomepageStock stock)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/HomepageStock/{stock.Id}", stock);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteStockAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/HomepageStock/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
