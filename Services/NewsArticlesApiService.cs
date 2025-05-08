namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models.Articles;

    /// <summary>
    /// Service for interacting with the News Articles API.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="NewsArticlesApiService"/> class.
    /// </remarks>
    /// <param name="api">The API service used for HTTP requests.</param>
    /// <param name="baseUrl">The base URL of the API.</param>
    internal class NewsArticlesApiService(ApiService api, string baseUrl)
    {
        private readonly string routerUrl = $"{baseUrl}/api/NewsArticles";

        /// <summary>
        /// Adds a handler for load events.
        /// </summary>
        /// <param name="handler">The event handler to add.</param>
        public void AddLoadHandler(EventHandler<bool> handler) => api.AddLoadHandler(handler);

        /// <summary>
        /// Adds a handler for error events.
        /// </summary>
        /// <param name="handler">The event handler to add.</param>
        public void AddErrorHandler(EventHandler<string> handler) => api.AddErrorHandler(handler);

        /// <summary>
        /// Retrieves all news articles asynchronously.
        /// </summary>
        /// <returns>A list of all news articles.</returns>
        public async Task<List<NewsArticle>> GetAllNewsArticlesAsync() =>
            await api.GetAsync<List<NewsArticle>>($"{this.routerUrl}") ?? [];

        /// <summary>
        /// Retrieves news articles by category asynchronously.
        /// </summary>
        /// <param name="category">The category of the news articles.</param>
        /// <returns>A list of news articles in the specified category.</returns>
        public async Task<List<NewsArticle>> GetNewsArticlesByCategoryAsync(string category) =>
            await api.GetAsync<List<NewsArticle>>($"{this.routerUrl}/category/{category}") ?? [];

        /// <summary>
        /// Retrieves news articles by stock name asynchronously.
        /// </summary>
        /// <param name="stockName">The name of the stock.</param>
        /// <returns>A list of news articles related to the specified stock.</returns>
        public async Task<List<NewsArticle>> GetNewsArticlesByStockAsync(string stockName) =>
            await api.GetAsync<List<NewsArticle>>($"{this.routerUrl}/stock/{stockName}") ?? [];

        /// <summary>
        /// Retrieves a news article by its ID asynchronously.
        /// </summary>
        /// <param name="articleId">The ID of the news article.</param>
        /// <returns>The news article with the specified ID, or null if not found.</returns>
        public async Task<NewsArticle?> GetNewsArticleByIdAsync(int articleId) =>
            await api.GetAsync<NewsArticle>($"{this.routerUrl}/{articleId}");

        /// <summary>
        /// Creates a new news article asynchronously.
        /// </summary>
        /// <param name="article">The news article to create.</param>
        /// <returns>Nothing.</returns>
        public async Task CreateNewsArticleAsync(NewsArticle article) =>
            await api.PostAsync($"{this.routerUrl}", article);

        /// <summary>
        /// Adds related stocks to a news article asynchronously.
        /// </summary>
        /// <param name="articleId">The ID of the news article.</param>
        /// <param name="stockIds">The IDs of the related stocks.</param>
        /// <returns>Nothing.</returns>
        public async Task AddRelatedStocksAsync(int articleId, IEnumerable<int> stockIds) =>
            await api.PostAsync($"{this.routerUrl}/{articleId}/stocks", stockIds);

        /// <summary>
        /// Updates an existing news article asynchronously.
        /// </summary>
        /// <param name="article">The news article to update.</param>
        /// <returns>Nothing.</returns>
        public async Task UpdateNewsArticleAsync(NewsArticle article) =>
            await api.PutAsync($"{this.routerUrl}/{article.Id}", article);

        /// <summary>
        /// Deletes a news article by its ID asynchronously.
        /// </summary>
        /// <param name="articleId">The ID of the news article to delete.</param>
        /// <returns>Nothing.</returns>
        public async Task DeleteNewsArticleAsync(int articleId) =>
            await api.DeleteAsync($"{this.routerUrl}/{articleId}");

        /// <summary>
        /// Marks a news article as read asynchronously.
        /// </summary>
        /// <param name="articleId">The ID of the news article to mark as read.</param>
        /// <returns>Nothing.</returns>
        public async Task MarkNewsArticleAsReadAsync(int articleId) =>
            await api.PutAsync($"{this.routerUrl}/{articleId}/read");
    }
}
