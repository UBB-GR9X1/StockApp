namespace StockApp.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;
    using StockApp.Models.Articles;

    public interface INewsService
    {
        Task<List<NewsArticle>> GetNewsArticles();

        Task<NewsArticle> GetNewsArticleById(int articleId);

        Task<bool> MarkArticleAsRead(int articleId);

        Task<bool> CreateArticle(NewsArticle article);

        Task<List<UserArticle>> GetUserArticles(Status status = Status.Pending, string? topic = null);

        Task<bool> ApproveUserArticle(int articleId);

        Task<bool> RejectUserArticle(int articleId);

        Task<bool> DeleteUserArticle(int articleId);

        Task<bool> SubmitUserArticle(UserArticle article);

        User GetCurrentUser();

        Task<User> LoginAsync(string username, string password);

        void Logout();

        Task StorePreviewArticle(NewsArticle article, UserArticle userArticle);

        UserArticle GetUserArticleForPreview(int articleId);

        List<string> GetRelatedStocksForArticle(int articleId);

        void UpdateCachedArticles(List<NewsArticle> articles);

        Task<List<NewsArticle>> GetCachedArticles();
    }
}
