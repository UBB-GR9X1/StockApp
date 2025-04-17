namespace StockApp.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Data.SqlClient;
    using StockApp.Database;
    using StockApp.Models;

    /// <summary>
    /// Repository for managing news articles and user-submitted articles.
    /// </summary>
    public class NewsRepository
    {
        private readonly List<NewsArticle> newsArticles = [];
        private readonly List<UserArticle> userArticles = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="NewsRepository"/> class.
        /// </summary>
        public NewsRepository()
        {

            this.LoadNewsArticles();
            this.LoadUserArticles();
        }

        /// <summary>
        /// Ensures a user exists in the database. If not, creates a new user.
        /// </summary>
        /// <param name="cnp">The user's unique identifier.</param>
        /// <param name="name">The user's name.</param>
        /// <param name="description">The user's description.</param>
        /// <param name="isAdmin">Indicates if the user is an admin.</param>
        /// <param name="isHidden">Indicates if the user is hidden.</param>
        /// <param name="profilePicture">The user's profile picture.</param>
        /// <param name="gemBalance">The user's gem balance. Default is 1000.</param>
        public void EnsureUserExists(string cnp, string name, string description, bool isAdmin, bool isHidden, string profilePicture, int gemBalance = 1000)
        {
            this.ExecuteNonQuery(
                "IF NOT EXISTS (SELECT 1 FROM [USER] WHERE CNP = @CNP) " +
                "INSERT INTO [USER] (CNP, NAME, DESCRIPTION, IS_HIDDEN, IS_ADMIN, PROFILE_PICTURE, GEM_BALANCE) " +
                "VALUES (@CNP, @Name, @Description, @IsHidden, @IsAdmin, @ProfilePicture, @GemBalance)",
                new Dictionary<string, object>
                {
                            { "@CNP", cnp },
                            { "@Name", name },
                            { "@Description", description },
                            { "@IsHidden", isHidden ? 1 : 0 },
                            { "@IsAdmin", isAdmin ? 1 : 0 },
                            { "@ProfilePicture", profilePicture },
                            { "@GemBalance", gemBalance },
                });
        }

        /// <summary>
        /// Loads all news articles from the database into memory.
        /// </summary>
        public void LoadNewsArticles()
        {
            this.newsArticles.Clear();
            this.newsArticles.AddRange(this.LoadArticles<NewsArticle>("SELECT * FROM NEWS_ARTICLE", MapNewsArticle));
        }

        /// <summary>
        /// Retrieves related stocks for a specific article.
        /// </summary>
        /// <param name="articleId">The ID of the article.</param>
        /// <returns>A list of related stock names.</returns>
        public List<string> GetRelatedStocksForArticle(string articleId)
        {
            return this.ExecuteReader(
                "SELECT STOCK_NAME FROM RELATED_STOCKS WHERE ARTICLE_ID = @ArticleId",
                new Dictionary<string, object> { { "@ArticleId", articleId } },
                reader => reader.GetString(0));
        }

        /// <summary>
        /// Adds or updates a news article in the database.
        /// </summary>
        /// <param name="newsArticle">The news article to add or update.</param>
        public void AddOrUpdateNewsArticle(NewsArticle newsArticle)
        {
            if (this.ArticleExists(newsArticle.ArticleId, "NEWS_ARTICLE"))
            {
                this.UpdateArticle(newsArticle, "NEWS_ARTICLE", MapNewsArticleParameters);
            }
            else
            {
                this.AddArticle(newsArticle, "NEWS_ARTICLE", MapNewsArticleParameters);
            }
        }

        /// <summary>
        /// Deletes a news article from the database.
        /// </summary>
        /// <param name="articleId">The ID of the article to delete.</param>
        public void DeleteNewsArticle(string articleId)
        {
            this.DeleteArticle(articleId, "NEWS_ARTICLE");
        }

        /// <summary>
        /// Retrieves a news article by its ID.
        /// </summary>
        /// <param name="articleId">The ID of the article.</param>
        /// <returns>The news article with the specified ID.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the article is not found.</exception>
        public NewsArticle GetNewsArticleById(string articleId)
        {
            return this.newsArticles.FirstOrDefault(a => a.ArticleId == articleId)
                ?? throw new KeyNotFoundException($"Article with ID {articleId} not found");
        }

        /// <summary>
        /// Retrieves all news articles.
        /// </summary>
        /// <returns>A list of all news articles.</returns>
        public List<NewsArticle> GetAllNewsArticles() => [.. this.newsArticles];

        /// <summary>
        /// Retrieves news articles related to a specific stock.
        /// </summary>
        /// <param name="stockName">The name of the stock.</param>
        /// <returns>A list of news articles related to the stock.</returns>
        public List<NewsArticle> GetNewsArticlesByStock(string stockName) =>
            [.. this.newsArticles.Where(a => a.RelatedStocks.Contains(stockName))];

        /// <summary>
        /// Retrieves news articles by category.
        /// </summary>
        /// <param name="category">The category of the articles.</param>
        /// <returns>A list of news articles in the specified category.</returns>
        public List<NewsArticle> GetNewsArticlesByCategory(string category) =>
            [.. this.newsArticles.Where(a => a.Category == category)];

        /// <summary>
        /// Marks a news article as read.
        /// </summary>
        /// <param name="articleId">The ID of the article to mark as read.</param>
        public void MarkArticleAsRead(string articleId)
        {
            var article = this.GetNewsArticleById(articleId);
            article.IsRead = true;
            this.AddOrUpdateNewsArticle(article);
        }

        /// <summary>
        /// Loads all user-submitted articles from the database into memory.
        /// </summary>
        public void LoadUserArticles()
        {
            this.userArticles.Clear();
            this.userArticles.AddRange(this.LoadArticles<UserArticle>("SELECT * FROM USER_ARTICLE ua INNER JOIN [USER] u ON ua.AUTHOR_CNP = u.CNP", MapUserArticle));
        }

        /// <summary>
        /// Adds or updates a user-submitted article in the database.
        /// </summary>
        /// <param name="userArticle">The user article to add or update.</param>
        public void AddOrUpdateUserArticle(UserArticle userArticle)
        {
            if (this.ArticleExists(userArticle.ArticleId, "USER_ARTICLE"))
            {
                this.UpdateArticle(userArticle, "USER_ARTICLE", MapUserArticleParameters);
            }
            else
            {
                this.AddArticle(userArticle, "USER_ARTICLE", MapUserArticleParameters);
            }
        }

        /// <summary>
        /// Deletes a user-submitted article from the database.
        /// </summary>
        /// <param name="articleId">The ID of the article to delete.</param>
        public void DeleteUserArticle(string articleId)
        {
            this.DeleteArticle(articleId, "USER_ARTICLE");
        }

        /// <summary>
        /// Retrieves a user-submitted article by its ID.
        /// </summary>
        /// <param name="articleId">The ID of the article.</param>
        /// <returns>The user article with the specified ID.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the article is not found.</exception>
        public UserArticle GetUserArticleById(string articleId)
        {
            return this.userArticles.FirstOrDefault(a => a.ArticleId == articleId)
                ?? throw new KeyNotFoundException($"User article with ID {articleId} not found");
        }

        /// <summary>
        /// Retrieves all user-submitted articles.
        /// </summary>
        /// <returns>A list of all user articles.</returns>
        public List<UserArticle> GetAllUserArticles() => [.. this.userArticles];

        /// <summary>
        /// Retrieves user-submitted articles by their status.
        /// </summary>
        /// <param name="status">The status of the articles.</param>
        /// <returns>A list of user articles with the specified status.</returns>
        public List<UserArticle> GetUserArticlesByStatus(string status) =>
            [.. this.userArticles.Where(a => a.Status == status)];

        /// <summary>
        /// Retrieves user-submitted articles by their topic.
        /// </summary>
        /// <param name="topic">The topic of the articles.</param>
        /// <returns>A list of user articles with the specified topic.</returns>
        public List<UserArticle> GetUserArticlesByTopic(string topic) =>
            [.. this.userArticles.Where(a => a.Topic == topic)];

        /// <summary>
        /// Approves a user-submitted article and adds it to the news articles.
        /// </summary>
        /// <param name="articleId">The ID of the article to approve.</param>
        public void ApproveUserArticle(string articleId)
        {
            var article = this.GetUserArticleById(articleId);
            article.Status = "Approved";
            this.AddOrUpdateUserArticle(article);

            var newsArticle = new NewsArticle
            {
                ArticleId = article.ArticleId,
                Title = article.Title,
                Summary = article.Summary,
                Content = article.Content,
                Source = $"User: {article.Author.Username}",
                PublishedDate = article.SubmissionDate.ToString("MMMM dd, yyyy"),
                IsRead = false,
                IsWatchlistRelated = false,
                Category = article.Topic,
                RelatedStocks = article.RelatedStocks,
            };

            this.AddOrUpdateNewsArticle(newsArticle);
        }

        /// <summary>
        /// Rejects a user-submitted article and removes it from the news articles if it exists.
        /// </summary>
        /// <param name="articleId">The ID of the article to reject.</param>
        public void RejectUserArticle(string articleId)
        {
            var article = this.GetUserArticleById(articleId);
            article.Status = "Rejected";
            this.AddOrUpdateUserArticle(article);

            try
            {
                this.DeleteNewsArticle(articleId);
            }
            catch (KeyNotFoundException)
            {
                // Article doesn't exist in news articles, which is fine
            }
        }

        /// <summary>
        /// Checks if an article exists in the specified table.
        /// </summary>
        /// <param name="articleId">The ID of the article.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>True if the article exists, otherwise false.</returns>
        private bool ArticleExists(string articleId, string tableName)
        {
            return this.ExecuteScalar<int>(
                $"SELECT COUNT(*) FROM {tableName} WHERE ARTICLE_ID = @ArticleId",
                new Dictionary<string, object> { { "@ArticleId", articleId } }) > 0;
        }

        /// <summary>
        /// Adds a new article to the specified table.
        /// </summary>
        /// <typeparam name="T">The type of the article.</typeparam>
        /// <param name="article">The article to add.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="mapParameters">The function to map article properties to SQL parameters.</param>
        private void AddArticle<T>(T article, string tableName, Action<SqlCommand, T> mapParameters)
        {
            this.ExecuteNonQuery(
                $"INSERT INTO {tableName} VALUES ({GetColumnPlaceholders(tableName)})",
                command => mapParameters(command, article));
        }

        /// <summary>
        /// Updates an existing article in the specified table.
        /// </summary>
        /// <typeparam name="T">The type of the article.</typeparam>
        /// <param name="article">The article to update.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="mapParameters">The function to map article properties to SQL parameters.</param>
        private void UpdateArticle<T>(T article, string tableName, Action<SqlCommand, T> mapParameters)
        {
            this.ExecuteNonQuery(
                $"UPDATE {tableName} SET {GetUpdatePlaceholders(tableName)} WHERE ARTICLE_ID = @ArticleId",
                command => mapParameters(command, article));
        }

        /// <summary>
        /// Deletes an article from the specified table.
        /// </summary>
        /// <param name="articleId">The ID of the article to delete.</param>
        /// <param name="tableName">The name of the table.</param>
        private void DeleteArticle(string articleId, string tableName)
        {
            this.ExecuteNonQuery(
                $"DELETE FROM {tableName} WHERE ARTICLE_ID = @ArticleId",
                new Dictionary<string, object> { { "@ArticleId", articleId } });
        }

        /// <summary>
        /// Loads articles from the database using the specified query and mapping function.
        /// </summary>
        /// <typeparam name="T">The type of the articles.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="map">The function to map SQL data to article objects.</param>
        /// <returns>A list of articles.</returns>
        private List<T> LoadArticles<T>(string query, Func<SqlDataReader, T> map)
        {
            return this.ExecuteReader(query, [], map);
        }

        /// <summary>
        /// Adds mock data to the database.
        /// </summary>
        private void AddMockData()
        {
            // Add mock data logic here
        }

        /// <summary>
        /// Maps a <see cref="NewsArticle"/> object to SQL parameters.
        /// </summary>
        /// <param name="command">The SQL command.</param>
        /// <param name="article">The news article to map.</param>
        private static void MapNewsArticleParameters(SqlCommand command, NewsArticle article)
        {
            command.Parameters.AddWithValue("@ArticleId", article.ArticleId);
            command.Parameters.AddWithValue("@Title", article.Title);
            command.Parameters.AddWithValue("@Summary", article.Summary ?? string.Empty);
            command.Parameters.AddWithValue("@Content", article.Content);
            command.Parameters.AddWithValue("@Source", article.Source ?? string.Empty);
            command.Parameters.AddWithValue("@PublishedDate", article.PublishedDate);
            command.Parameters.AddWithValue("@IsRead", article.IsRead);
            command.Parameters.AddWithValue("@IsWatchlistRelated", article.IsWatchlistRelated);
            command.Parameters.AddWithValue("@Category", article.Category ?? string.Empty);
        }

        /// <summary>
        /// Maps a <see cref="UserArticle"/> object to SQL parameters.
        /// </summary>
        /// <param name="command">The SQL command.</param>
        /// <param name="article">The user article to map.</param>
        private static void MapUserArticleParameters(SqlCommand command, UserArticle article)
        {
            command.Parameters.AddWithValue("@ArticleId", article.ArticleId);
            command.Parameters.AddWithValue("@Title", article.Title);
            command.Parameters.AddWithValue("@Summary", article.Summary ?? string.Empty);
            command.Parameters.AddWithValue("@Content", article.Content);
            command.Parameters.AddWithValue("@Author", article.Author.CNP);
            command.Parameters.AddWithValue("@SubmissionDate", article.SubmissionDate);
            command.Parameters.AddWithValue("@Status", article.Status);
            command.Parameters.AddWithValue("@Topic", article.Topic ?? string.Empty);
        }

        /// <summary>
        /// Maps a SQL data reader to a <see cref="NewsArticle"/> object.
        /// </summary>
        /// <param name="reader">The SQL data reader.</param>
        /// <returns>A <see cref="NewsArticle"/> object.</returns>
        private static NewsArticle MapNewsArticle(SqlDataReader reader)
        {
            return new NewsArticle
            {
                ArticleId = reader.GetString(0),
                Title = reader.GetString(1),
                Summary = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Content = reader.GetString(3),
                Source = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                PublishedDate = reader.GetString(5),
                IsRead = reader.GetBoolean(6),
                IsWatchlistRelated = reader.GetBoolean(7),
                Category = reader.GetString(8),
                RelatedStocks = [], // Load related stocks separately
            };
        }

        /// <summary>
        /// Maps a SQL data reader to a <see cref="UserArticle"/> object.
        /// </summary>
        /// <param name="reader">The SQL data reader.</param>
        /// <returns>A <see cref="UserArticle"/> object.</returns>
        private static UserArticle MapUserArticle(SqlDataReader reader)
        {
            string AuthorCNP = reader["AUTHOR_CNP"]?.ToString() ?? throw new Exception("Author CNP is null");
            var author = new User
            {
                CNP = AuthorCNP,
                Username = reader["NAME"]?.ToString() ?? throw new Exception("Author name is null"),
                Description = reader["DESCRIPTION"]?.ToString() ?? string.Empty,
                IsHidden = reader["IS_HIDDEN"] != DBNull.Value && (bool)reader["IS_HIDDEN"],
                Image = reader["PROFILE_PICTURE"]?.ToString() ?? string.Empty,
            };
            return new UserArticle
            {
                ArticleId = reader["ARTICLE_ID"]?.ToString() ?? throw new Exception("Article ID is null"),
                Title = reader["TITLE"]?.ToString() ?? throw new Exception("Title is null"),
                Summary = reader["SUMMARY"]?.ToString() ?? string.Empty,
                Content = reader["CONTENT"]?.ToString() ?? throw new Exception("Content is null"),
                Author = author,
                SubmissionDate = DateTime.TryParse(reader["SUBMISSION_DATE"]?.ToString(), out var date) ? date : throw new Exception("Submission date is null"),
                Status = reader["STATUS"]?.ToString() ?? throw new Exception("Status is null"),
                Topic = reader["TOPIC"] != DBNull.Value ? (string)reader["TOPIC"] : string.Empty,
                RelatedStocks = [], // Load related stocks separately
            };
        }

        /// <summary>
        /// Gets column placeholders for the specified table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>A string of column placeholders.</returns>
        private static string GetColumnPlaceholders(string tableName)
        {
            // Return column placeholders for the table
            return string.Join(", ", ["@ArticleId", "@Title", "@Summary", "@Content", "@Source", "@PublishedDate", "@IsRead", "@IsWatchlistRelated", "@Category"]);
        }

        /// <summary>
        /// Gets update placeholders for the specified table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>A string of update placeholders.</returns>
        private static string GetUpdatePlaceholders(string tableName)
        {
            // Return update placeholders for the table
            return string.Join(", ", ["TITLE = @Title", "SUMMARY = @Summary", "CONTENT = @Content", "SOURCE = @Source", "PUBLISH_DATE = @PublishedDate", "IS_READ = @IsRead", "IS_WATCHLIST_RELATED = @IsWatchlistRelated", "CATEGORY = @Category"]);
        }

        /// <summary>
        /// Executes a non-query SQL command.
        /// </summary>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        private void ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            using var connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand(query, connection);
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Executes a non-query SQL command with a custom configuration.
        /// </summary>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="configureCommand">The action to configure the SQL command.</param>
        private void ExecuteNonQuery(string query, Action<SqlCommand> configureCommand)
        {
            using var connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand(query, connection);
            configureCommand(command);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Executes a scalar SQL query and returns the result.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <returns>The result of the query.</returns>
        private T ExecuteScalar<T>(string query, Dictionary<string, object> parameters)
        {
            using var connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand(query, connection);
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }

            return (T)command.ExecuteScalar();
        }

        /// <summary>
        /// Executes a SQL query and maps the results using the specified function.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="map">The function to map SQL data to objects.</param>
        /// <returns>A list of results.</returns>
        private List<T> ExecuteReader<T>(string query, Dictionary<string, object> parameters, Func<SqlDataReader, T> map)
        {
            var results = new List<T>();
            using var connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand(query, connection);
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                results.Add(map(reader));
            }

            return results;
        }

        public void AddNewsArticle(NewsArticle article)
        {
            this.newsArticles.Add(article);
            this.AddOrUpdateNewsArticle(article);
        }

        internal void AddUserArticle(UserArticle article)
        {
            this.userArticles.Add(article);
            this.AddOrUpdateUserArticle(article);
        }

        internal void AddRelatedStocksForArticle(string articleId, List<string> relatedStocks, object value)
        {
            using var connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand("INSERT INTO RELATED_STOCKS (ARTICLE_ID, STOCK_NAME) VALUES (@ArticleId, @StockName)", connection);
            command.Parameters.AddWithValue("@ArticleId", articleId);
            foreach (var stock in relatedStocks)
            {
                command.Parameters.AddWithValue("@StockName", stock);
                command.ExecuteNonQuery();
            }
        }
    }
}