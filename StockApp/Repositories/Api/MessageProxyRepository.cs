
namespace StockApp.Repositories.Api
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using StockApp.Models;

    internal class MessageProxyRepository : IMessagesRepository
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/Message";

        public MessageProxyRepository(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<Message>> GetMessagesForUserAsync(string userCnp)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{userCnp}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Message>>() ?? new List<Message>();
        }

        public async Task GiveUserRandomMessageAsync(string userCnp)
        {
            var response = await _httpClient.PostAsync($"{BaseUrl}/{userCnp}/random", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task GiveUserRandomRoastMessageAsync(string userCnp)
        {
            var response = await _httpClient.PostAsync($"{BaseUrl}/{userCnp}/roast", null);
            response.EnsureSuccessStatusCode();
        }
    }
}
