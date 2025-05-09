using System.Collections.Generic;
using System.Threading.Tasks;
using StockApp.Models;

namespace StockApp.Services
{
    public interface INewsService
    {
        Task<bool> ApproveUserArticle(string articleId);

        bool CreateArticle(NewsArticle article);

        Task<bool> DeleteUserArticle(string articleId);

        List<NewsArticle> GetCachedArticles();

        Task<User> GetCurrentUser();

        NewsArticle GetNewsArticleById(string articleId);

        List<NewsArticle> GetNewsArticles();

        List<string> GetRelatedStocksForArticle(string articleId);

        UserArticle GetUserArticleForPreview(string articleId);

        Task<List<UserArticle>> GetUserArticles(string status = null, string topic = null);

        Task<User> LoginAsync(string username, string password);

        void Logout();

        bool MarkArticleAsRead(string articleId);

        Task<bool> RejectUserArticle(string articleId);

        void StorePreviewArticle(NewsArticle article, UserArticle userArticle);

        Task<bool> SubmitUserArticle(UserArticle article);

        void UpdateCachedArticles(List<NewsArticle> articles);
    }
}