namespace StockApp.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;

    public interface INewsService
    {
        List<NewsArticle> GetNewsArticles();

        NewsArticle GetNewsArticleById(string articleId);

        bool MarkArticleAsRead(string articleId);

        bool CreateArticle(NewsArticle article);

        List<UserArticle> GetUserArticles(string status = null, string topic = null);

        bool ApproveUserArticle(string articleId);

        bool RejectUserArticle(string articleId);

        bool DeleteUserArticle(string articleId);

        bool SubmitUserArticle(UserArticle article);

        User GetCurrentUser();

        Task<User> LoginAsync(string username, string password);

        void Logout();

        void StorePreviewArticle(NewsArticle article, UserArticle userArticle);

        UserArticle GetUserArticleForPreview(string articleId);

        List<string> GetRelatedStocksForArticle(string articleId);

        void UpdateCachedArticles(List<NewsArticle> articles);

        List<NewsArticle> GetCachedArticles();
    }
}
