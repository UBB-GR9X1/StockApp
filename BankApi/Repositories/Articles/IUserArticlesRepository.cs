namespace BankApi.Repositories.Articles
{
    using BankApi.Models;
    using BankApi.Models.Articles;
    
    public interface IUserArticlesRepository
    {
        List<UserArticle> GetAllUserArticlesAsync();

        List<UserArticle> GetUserArticlesByStatusAsync(Status status);

        List<UserArticle> GetUserArticlesByTopicAsync(string topic);

        UserArticle GetUserArticleByIdAsync(int articleId);

        void AddUserArticleAsync(UserArticle article);

        void ApproveUserArticleAsync(int articleId);

        void RejectUserArticleAsync(int articleId);

        void UpdateUserArticleAsync(UserArticle userArticle);

        void DeleteUserArticleAsync(int articleId);
    }
}
