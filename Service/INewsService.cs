namespace StockApp.Service
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StockApp.Models;

    public interface INewsService
    {
        Task<IReadOnlyList<INewsArticle>> GetNewsArticlesAsync();

        Task<INewsArticle> GetNewsArticleByIdAsync(string articleId);

        Task<bool> MarkArticleAsReadAsync(string articleId);

        Task<bool> CreateArticleAsync(INewsArticle article);

        Task<IReadOnlyList<IUserArticle>> GetUserArticlesAsync(string status = null, string topic = null);

        Task<bool> ApproveUserArticleAsync(string articleId);

        Task<bool> RejectUserArticleAsync(string articleId);

        Task<bool> DeleteUserArticleAsync(string articleId);

        Task<bool> SubmitUserArticleAsync(IUserArticle article);

        Task<IUser> GetCurrentUserAsync();

        Task<IUser> LoginAsync(string username, string password);

        void Logout();

        void StorePreviewArticle(INewsArticle article, IUserArticle userArticle);

        IUserArticle GetUserArticleForPreview(string articleId);

        IReadOnlyList<string> GetRelatedStocksForArticle(string articleId);

        void UpdateCachedArticles(IReadOnlyList<INewsArticle> articles);

        IReadOnlyList<INewsArticle> GetCachedArticles();
    }
}
