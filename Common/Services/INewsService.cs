using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Services
{
    public interface INewsService
    {
        Task<bool> ApproveUserArticleAsync(string articleId);

        Task<bool> CreateArticleAsync(NewsArticle article);

        Task<bool> DeleteUserArticleAsync(string articleId);

        Task<NewsArticle> GetNewsArticleByIdAsync(string articleId);

        Task<List<NewsArticle>> GetNewsArticlesAsync();

        Task<List<Stock>> GetRelatedStocksForArticleAsync(string articleId);

        Task<List<NewsArticle>> GetUserArticlesAsync(string authorCNP, Status status = Status.All, string topic = "All");

        Task<bool> MarkArticleAsReadAsync(string articleId);

        Task<bool> RejectUserArticleAsync(string articleId);

        Task<bool> SubmitUserArticleAsync(NewsArticle article);
    }
}