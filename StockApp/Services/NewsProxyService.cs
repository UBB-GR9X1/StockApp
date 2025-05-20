using Common.Models;
using Common.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace StockApp.Services
{
    public class NewsProxyService(HttpClient httpClient) : IProxyService, INewsService
    {
        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        public async Task<bool> ApproveUserArticleAsync(string articleId, string? userCNP = null)
        {
            // userCNP is not part of the controller method signature for approve, it's derived from claims.
            var response = await _httpClient.PostAsync($"api/News/{articleId}/approve", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> CreateArticleAsync(NewsArticle article, string? authorCNP = null)
        {
            // authorCNP is not part of the controller method signature for create, it's derived from claims or set in service.
            var response = await _httpClient.PostAsJsonAsync("api/News/create", article);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> DeleteUserArticleAsync(string articleId, string? userCNP = null)
        {
            // userCNP is not part of the controller method signature for delete, it's derived from claims.
            var response = await _httpClient.DeleteAsync($"api/News/{articleId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<NewsArticle> GetNewsArticleByIdAsync(string articleId)
        {
            return await _httpClient.GetFromJsonAsync<NewsArticle>($"api/News/{articleId}") ?? throw new InvalidOperationException("Failed to deserialize news article response.");
        }

        public async Task<List<NewsArticle>> GetNewsArticlesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<NewsArticle>>("api/News") ?? throw new InvalidOperationException("Failed to deserialize news articles response.");
        }

        public async Task<List<Stock>> GetRelatedStocksForArticleAsync(string articleId)
        {
            return await _httpClient.GetFromJsonAsync<List<Stock>>($"api/News/{articleId}/relatedstocks") ?? throw new InvalidOperationException("Failed to deserialize related stocks response.");
        }

        public async Task<List<NewsArticle>> GetUserArticlesAsync(Status status = Status.All, string topic = "All", string? authorCNP = null)
        {
            // The controller expects authorCnp as a query parameter, not userCNP.
            return await _httpClient.GetFromJsonAsync<List<NewsArticle>>($"api/News/userarticles?authorCnp={authorCNP}&status={status}&topic={topic}") ?? throw new InvalidOperationException("Failed to deserialize user articles response.");
        }

        public async Task<bool> MarkArticleAsReadAsync(string articleId)
        {
            var response = await _httpClient.PostAsync($"api/News/{articleId}/markasread", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> RejectUserArticleAsync(string articleId, string? userCNP = null)
        {
            // userCNP is not part of the controller method signature for reject, it's derived from claims.
            var response = await _httpClient.PostAsync($"api/News/{articleId}/reject", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<bool> SubmitUserArticleAsync(NewsArticle article, string? userCNP = null)
        {
            // userCNP is not part of the controller method signature for submit, it's derived from claims.
            var response = await _httpClient.PostAsJsonAsync("api/News/submit", article);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }
    }
}