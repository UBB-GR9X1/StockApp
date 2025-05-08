namespace BankApi.Repositories.Articles
{
    using BankApi.Models.Articles;

    public interface INewsArticleRepository
    {
        Task<List<NewsArticle>> GetAllNewsArticlesAsync();
        
        Task<List<NewsArticle>> GetNewsArticlesByCategoryAsync(string category);
        
        Task<List<NewsArticle>> GetNewsArticlesByStockAsync(string stockName);

        Task<NewsArticle?> GetNewsArticleByIdAsync(int articleId);

        Task AddNewsArticleAsync(NewsArticle article);

        Task AddRelatedStocksAsync(int articleId, List<int> stockIds);

        Task UpdateNewsArticleAsync(NewsArticle newsArticle);

        Task<bool> DeleteNewsArticleAsync(int articleId);

        Task<bool> MarkNewsArticleAsReadAsync(int articleId);
    }
}
