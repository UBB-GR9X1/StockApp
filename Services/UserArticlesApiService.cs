namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;
    using StockApp.Models.Articles;

    /// <summary>
    /// Provides methods to interact with the User Articles API.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="UserArticlesApiService"/> class.
    /// </remarks>
    /// <param name="api">The API service used for HTTP requests.</param>
    /// <param name="baseUrl">The base URL for the API.</param>
    public class UserArticlesApiService(ApiService api, string baseUrl)
    {
        private readonly string routerUrl = $"{baseUrl}/api/UserArticles";

        /// <summary>
        /// Adds a handler for the load event.
        /// </summary>
        /// <param name="handler">The event handler to add.</param>
        public void AddLoadHandler(EventHandler<bool> handler) => api.AddLoadHandler(handler);

        /// <summary>
        /// Adds a handler for the error event.
        /// </summary>
        /// <param name="handler">The event handler to add.</param>
        public void AddErrorHandler(EventHandler<string> handler) => api.AddErrorHandler(handler);

        /// <summary>
        /// Retrieves all user articles asynchronously.
        /// </summary>
        /// <returns>A list of user articles.</returns>
        public async Task<List<UserArticle>> GetAllUserArticlesAsync() =>
            await api.GetAsync<List<UserArticle>>(this.routerUrl) ?? [];

        /// <summary>
        /// Retrieves user articles by their status asynchronously.
        /// </summary>
        /// <param name="status">The status of the articles to retrieve.</param>
        /// <returns>A list of user articles with the specified status.</returns>
        public async Task<List<UserArticle>> GetUserArticlesByStatus(Status status) =>
            await api.GetAsync<List<UserArticle>>($"{this.routerUrl}/status/{status}") ?? [];

        /// <summary>
        /// Retrieves user articles by their topic asynchronously.
        /// </summary>
        /// <param name="topic">The topic of the articles to retrieve.</param>
        /// <returns>A list of user articles with the specified topic.</returns>
        public async Task<List<UserArticle>> GetUserArticlesByTopic(string topic) =>
            await api.GetAsync<List<UserArticle>>($"{this.routerUrl}/topic/{topic}") ?? [];

        /// <summary>
        /// Retrieves a user article by its ID asynchronously.
        /// </summary>
        /// <param name="articleId">The ID of the article to retrieve.</param>
        /// <returns>The user article with the specified ID, or null if not found.</returns>
        public async Task<UserArticle?> GetUserArticleById(int articleId) =>
            await api.GetAsync<UserArticle?>($"{this.routerUrl}/{articleId}");

        /// <summary>
        /// Adds a new user article asynchronously.
        /// </summary>
        /// <param name="article">The user article to add.</param>
        /// <returns>Nothing.</returns>
        public async Task AddUserArticle(UserArticle article) =>
            await api.PostAsync(this.routerUrl, article);

        /// <summary>
        /// Approves a user article asynchronously.
        /// </summary>
        /// <param name="articleId">The ID of the article to approve.</param>
        /// <returns>Nothing.</returns>
        public async Task ApproveUserArticle(int articleId) =>
            await api.PutAsync($"{this.routerUrl}/{articleId}/approve");

        /// <summary>
        /// Rejects a user article asynchronously.
        /// </summary>
        /// <param name="articleId">The ID of the article to reject.</param>
        /// <returns>Nothing.</returns>
        public async Task RejectUserArticle(int articleId) =>
            await api.PutAsync($"{this.routerUrl}/{articleId}/reject");

        /// <summary>
        /// Updates an existing user article asynchronously.
        /// </summary>
        /// <param name="article">The user article to update.</param>
        /// <returns>Nothing.</returns>
        public async Task UpdateUserArticle(UserArticle article) =>
            await api.PutAsync($"{this.routerUrl}/{article.Id}", article);

        /// <summary>
        /// Deletes a user article asynchronously.
        /// </summary>
        /// <param name="articleId">The ID of the article to delete.</param>
        /// <returns>Nothing.</returns>
        public async Task DeleteUserArticle(int articleId) =>
            await api.DeleteAsync($"{this.routerUrl}/{articleId}");
    }
}
