namespace StockApp.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;

    public interface INewsService
    {
        Task<List<NewsArticle>> GetNewsArticlesAsync();

        Task<NewsArticle> GetNewsArticleByIdAsync(string articleId);

        Task<bool> MarkArticleAsReadAsync(string articleId);

        Task<bool> CreateArticleAsync(NewsArticle article);

        Task<List<UserArticle>> GetUserArticlesAsync(string status = null, string topic = null);

        Task<bool> ApproveUserArticleAsync(string articleId);

        Task<bool> RejectUserArticleAsync(string articleId);

        Task<bool> DeleteUserArticleAsync(string articleId);

        Task<bool> SubmitUserArticleAsync(UserArticle article);

        Task<User> GetCurrentUserAsync();

        Task<User> LoginAsync(string username, string password);

        void Logout();

        void StorePreviewArticle(NewsArticle article, UserArticle userArticle);

        UserArticle GetUserArticleForPreview(string articleId);

        List<string> GetRelatedStocksForArticle(string articleId);

        void UpdateCachedArticles(List<NewsArticle> articles);

        List<NewsArticle> GetCachedArticles();
    }
}
