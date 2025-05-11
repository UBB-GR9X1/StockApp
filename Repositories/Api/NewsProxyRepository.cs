using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Repositories.Api
{
    internal class NewsProxyRepository : INewsRepository
    {
        private readonly HttpClient _httpClient;

        public NewsProxyRepository(HttpClient httpClient)
        {
            this._httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task AddNewsArticleAsync(NewsArticle newsArticle)
        {
            var response = await this._httpClient.PostAsJsonAsync("api/News/AddNewsArticle", newsArticle);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateNewsArticleAsync(NewsArticle newsArticle)
        {
            var response = await this._httpClient.PutAsJsonAsync("api/News/UpdateNewsArticle", newsArticle);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteNewsArticleAsync(string articleId)
        {
            var response = await this._httpClient.DeleteAsync($"api/News/DeleteNewsArticle?articleId={articleId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<NewsArticle> GetNewsArticleByIdAsync(string articleId)
        {
            var result = await this._httpClient.GetFromJsonAsync<NewsArticle>($"api/News/GetNewsArticleById?articleId={articleId}");
            return result;
        }

        public async Task<List<NewsArticle>> GetAllNewsArticlesAsync()
        {
            var result = await this._httpClient.GetFromJsonAsync<List<NewsArticle>>("api/News/GetAllNewsArticles");
            return result ?? new List<NewsArticle>();
        }

        public async Task<List<NewsArticle>> GetNewsArticlesByAuthorCNPAsync(string authorCNP)
        {
            var result = await this._httpClient.GetFromJsonAsync<List<NewsArticle>>($"api/News/GetNewsArticlesByAuthorCNP?authorCNP={authorCNP}");
            return result ?? new List<NewsArticle>();
        }


        public async Task<List<NewsArticle>> GetNewsArticlesByCategoryAsync(string category)
        {
            var result = await this._httpClient.GetFromJsonAsync<List<NewsArticle>>($"api/News/GetNewsArticlesByCategory?category={category}");
            return result ?? new List<NewsArticle>();
        }

        public async Task<List<NewsArticle>> GetNewsArticlesByStockAsync(string stockName)
        {
            var result = await this._httpClient.GetFromJsonAsync<List<NewsArticle>>($"api/News/GetNewsArticlesByStock?stockName={stockName}");
            return result ?? new List<NewsArticle>();
        }

        public async Task MarkArticleAsReadAsync(string articleId)
        {
            var response = await this._httpClient.PostAsJsonAsync("api/News/MarkArticleAsRead", new { articleId });
            response.EnsureSuccessStatusCode();
        }
    }
}
