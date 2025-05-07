namespace StockApp.Repositories
{
    using System.Collections.Generic;
    using Microsoft.Data.SqlClient;
    using StockApp.Models;

    public interface INewsRepository
    {
        void AddNewsArticle(NewsArticle newsArticle);

        void AddRelatedStocksForArticle(string articleId, List<string> stockNames, SqlConnection connection = null, SqlTransaction transaction = null);

        void AddUserArticle(UserArticle userArticle);

        void ApproveUserArticle(string articleId);

        void DeleteNewsArticle(string articleId);

        void DeleteUserArticle(string articleId);

        void EnsureUserExists(string cnp, string name, string description, bool isAdmin, bool isHidden, string profilePicture, int gemBalance = 1000);

        List<NewsArticle> GetAllNewsArticles();

        List<UserArticle> GetAllUserArticles();

        NewsArticle GetNewsArticleById(string articleId);

        List<NewsArticle> GetNewsArticlesByCategory(string category);

        List<NewsArticle> GetNewsArticlesByStock(string stockName);

        UserArticle GetUserArticleById(string articleId);

        List<UserArticle> GetUserArticlesByStatus(string status);

        List<UserArticle> GetUserArticlesByTopic(string topic);

        void LoadNewsArticles();

        void LoadUserArticles();

        void MarkArticleAsRead(string articleId);

        void RejectUserArticle(string articleId);

        void UpdateNewsArticle(NewsArticle newsArticle);

        void UpdateUserArticle(UserArticle userArticle);
    }
}