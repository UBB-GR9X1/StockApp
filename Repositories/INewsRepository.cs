namespace StockApp.Repository
{
    using System.Collections.Generic;
    using Microsoft.Data.SqlClient;
    using StockApp.Models;

    public interface INewsRepository
    {
        void EnsureUserExists(
            string cnp,
            string name,
            string description,
            bool isAdmin,
            bool isHidden,
            string profilePicture,
            int gemBalance = 1000);

        void LoadNewsArticles();

        IReadOnlyList<INewsArticle> GetAllNewsArticles();

        INewsArticle GetNewsArticleById(string articleId);

        IReadOnlyList<INewsArticle> GetNewsArticlesByStock(string stockName);

        IReadOnlyList<INewsArticle> GetNewsArticlesByCategory(string category);

        void AddNewsArticle(INewsArticle newsArticle);

        void UpdateNewsArticle(INewsArticle newsArticle);

        void DeleteNewsArticle(string articleId);

        void MarkArticleAsRead(string articleId);

        IReadOnlyList<string> GetRelatedStocksForArticle(string articleId);

        void AddRelatedStocksForArticle(string articleId, IReadOnlyList<string> stockNames, SqlConnection connection, SqlTransaction transaction);

        void LoadUserArticles();

        IReadOnlyList<IUserArticle> GetAllUserArticles();

        IUserArticle GetUserArticleById(string articleId);

        IReadOnlyList<IUserArticle> GetUserArticlesByStatus(string status);

        IReadOnlyList<IUserArticle> GetUserArticlesByTopic(string topic);

        IReadOnlyList<IUserArticle> GetUserArticlesByStatusAndTopic(string status, string topic);

        void AddUserArticle(IUserArticle userArticle);

        void UpdateUserArticle(IUserArticle userArticle);

        void DeleteUserArticle(string articleId);

        void ApproveUserArticle(string articleId);

        void RejectUserArticle(string articleId);
    }
}
