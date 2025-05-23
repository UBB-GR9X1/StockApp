using Common.Models;
using Common.Services;
using System.Net.Http.Json;

namespace StockAppWeb.Services
{
    public class MessagesProxyService : IMessagesService
    {
        private readonly HttpClient _httpClient;

        public MessagesProxyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Message>> GetMessagesForUserAsync(string userCnp)
        {
            if (string.IsNullOrEmpty(userCnp))
            {
                // Current user
                var response = await _httpClient.GetFromJsonAsync<List<Message>>("api/messages/user");
                return response ?? new List<Message>();
            }
            else
            {
                // Admin: get messages for specific user
                var response = await _httpClient.GetFromJsonAsync<List<Message>>($"api/messages/user/{userCnp}");
                return response ?? new List<Message>();
            }
        }

        public async Task GiveMessageToUserAsync(string userCnp, string type, string messageText)
        {
            var request = new { Type = type, MessageText = messageText };
            await _httpClient.PostAsJsonAsync($"api/messages/user/{userCnp}/give", request);
        }
    }
} 