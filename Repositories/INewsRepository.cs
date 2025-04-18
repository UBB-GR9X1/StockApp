namespace StockApp.Repositories
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

        List<NewsArticle> GetAllNewsArticles();

        NewsArticle GetNewsArticleById(string articleId);

        List<NewsArticle> GetNewsArticlesByStock(string stockName);

        List<NewsArticle> GetNewsArticlesByCategory(string category);

        void AddNewsArticle(NewsArticle newsArticle);

        void UpdateNewsArticle(NewsArticle newsArticle);

        void DeleteNewsArticle(string articleId);

        void MarkArticleAsRead(string articleId);

        List<string> GetRelatedStocksForArticle(string articleId);

        void AddRelatedStocksForArticle(string articleId, List<string> stockNames, SqlConnection connection, SqlTransaction transaction);

        void LoadUserArticles();

        List<UserArticle> GetAllUserArticles();

        UserArticle GetUserArticleById(string articleId);

        List<UserArticle> GetUserArticlesByStatus(string status);

        List<UserArticle> GetUserArticlesByTopic(string topic);

        List<UserArticle> GetUserArticlesByStatusAndTopic(string status, string topic);

        void AddUserArticle(UserArticle userArticle);

        void UpdateUserArticle(UserArticle userArticle);

        void DeleteUserArticle(string articleId);

        void ApproveUserArticle(string articleId);

        void RejectUserArticle(string articleId);
    }
}
