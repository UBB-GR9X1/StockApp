namespace BankApi.Repositories.Articles
{
    using BankApi.Models;
    using BankApi.Models.Articles;
    
    public interface IUserArticlesRepository
    {
        Task<List<UserArticle>> GetAllUserArticlesAsync();

        Task<List<UserArticle>> GetUserArticlesByStatusAsync(Status status);

        Task<List<UserArticle>> GetUserArticlesByTopicAsync(string topic);

        Task<UserArticle?> GetUserArticleByIdAsync(int articleId);

        Task AddUserArticleAsync(UserArticle article);

        Task<bool> ApproveUserArticleAsync(int articleId);

        Task<bool> RejectUserArticleAsync(int articleId);

        Task UpdateUserArticleAsync(UserArticle userArticle);

        Task<bool> DeleteUserArticleAsync(int articleId);
    }
}
