namespace StockApp.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Data.SqlClient;
    using StockApp.Database;
    using StockApp.Models;

    public class NewsRepository
    {
        private static readonly object LockObject = new();
        private static bool isInitialized = false;

        private readonly DatabaseHelper databaseHelper = DatabaseHelper.Instance;
        private readonly List<NewsArticle> newsArticles = [];
        private readonly List<UserArticle> userArticles = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="NewsRepository"/> class.
        /// </summary>
        public NewsRepository()
        {
            this.Initialize();
        }
        private void Initialize()
        {
            lock (LockObject)
            {
                try
                {
                    // Check if data already exists
                    bool hasData = CheckIfDataExists();

                    // Load existing data
                    this.LoadNewsArticles();
                    this.LoadUserArticles();

                    isInitialized = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error initializing NewsRepository: {ex.Message}");
                    throw;
                }
            }
        }

        private static bool CheckIfDataExists()
        {
            using var connection = DatabaseHelper.GetConnection();

            using SqlCommand command = new("SELECT COUNT(*) FROM NEWS_ARTICLE", connection);
            command.CommandTimeout = 30;

            int count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
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
            using var connection = DatabaseHelper.GetConnection();
            string checkQuery = "IF NOT EXISTS (SELECT 1 FROM [USER] WHERE CNP = @CNP) " +
                                "INSERT INTO [USER] (CNP, NAME, DESCRIPTION, IS_HIDDEN, IS_ADMIN, PROFILE_PICTURE, GEM_BALANCE) " +
                                "VALUES (@CNP, @Name, @Description, @IsHidden, @IsAdmin, @ProfilePicture, @GemBalance)";

            using SqlCommand command = new(checkQuery, connection);
            command.Parameters.AddWithValue("@CNP", cnp);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Description", description);
            command.Parameters.AddWithValue("@IsHidden", isHidden ? 1 : 0);
            command.Parameters.AddWithValue("@IsAdmin", isAdmin ? 1 : 0);
            command.Parameters.AddWithValue("@ProfilePicture", profilePicture);
            command.Parameters.AddWithValue("@GemBalance", gemBalance);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Loads all news articles from the database into memory.
        /// </summary>
        public void LoadNewsArticles()
        {
            this.newsArticles.Clear();
            using var connection = DatabaseHelper.GetConnection();
            using SqlCommand command = new("SELECT * FROM NEWS_ARTICLE", connection);

            command.CommandTimeout = 30;
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var article = new NewsArticle
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
                    RelatedStocks = this.GetRelatedStocksForArticle(reader.GetString(0)),
                };

                this.newsArticles.Add(article);
            }
        }

        /// <summary>
        /// Retrieves related stocks for a specific article.
        /// </summary>
        /// <param name="articleId">The ID of the article.</param>
        /// <returns>A list of related stock names.</returns>
        public List<string> GetRelatedStocksForArticle(string articleId)
        {
            var relatedStocks = new List<string>();

            try
            {
                using var connection = DatabaseHelper.GetConnection();
                using (SqlCommand command = new("SELECT STOCK_NAME FROM RELATED_STOCKS WHERE ARTICLE_ID = @ArticleId", connection))
                {
                    command.CommandTimeout = 30;
                    command.Parameters.AddWithValue("@ArticleId", articleId);

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        relatedStocks.Add(reader.GetString(0));
                    }
                }

                // no stocks in database, check our mock data
                if (relatedStocks.Count == 0)
                {
                    var mockArticle = this.newsArticles.FirstOrDefault(a => a.ArticleId == articleId);
                    if (mockArticle != null && mockArticle.RelatedStocks != null && mockArticle.RelatedStocks.Count > 0)
                    {
                        relatedStocks.AddRange(mockArticle.RelatedStocks);
                        System.Diagnostics.Debug.WriteLine($"Found {relatedStocks.Count} related stocks in mock data for article {articleId}");

                        try
                        {
                            this.AddRelatedStocksForArticle(articleId, relatedStocks, connection);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error adding related stocks to database: {ex.Message}");
                        }
                    }

                    // user articles too
                    var userArticle = this.userArticles.FirstOrDefault(a => a.ArticleId == articleId);
                    if (userArticle != null && userArticle.RelatedStocks != null && userArticle.RelatedStocks.Count > 0)
                    {
                        relatedStocks.AddRange(userArticle.RelatedStocks.Where(s => !relatedStocks.Contains(s)));
                        System.Diagnostics.Debug.WriteLine($"Found {userArticle.RelatedStocks.Count} related stocks in user article for {articleId}");

                        try
                        {
                            this.AddRelatedStocksForArticle(articleId, relatedStocks, connection);
                            System.Diagnostics.Debug.WriteLine($"Added related stocks to database for user article {articleId}");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error adding related stocks to database: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting related stocks: {ex.Message}");

                // fallback to in-memory data
                var mockArticle = this.newsArticles.FirstOrDefault(a => a.ArticleId == articleId);
                if (mockArticle != null && mockArticle.RelatedStocks != null)
                {
                    relatedStocks.AddRange(mockArticle.RelatedStocks);
                    System.Diagnostics.Debug.WriteLine($"Fallback: Found {relatedStocks.Count} related stocks in mock data");
                }

                var userArticle = this.userArticles.FirstOrDefault(a => a.ArticleId == articleId);
                if (userArticle != null && userArticle.RelatedStocks != null)
                {
                    relatedStocks.AddRange(userArticle.RelatedStocks.Where(s => !relatedStocks.Contains(s)));
                    System.Diagnostics.Debug.WriteLine($"Fallback: Found {userArticle.RelatedStocks.Count} related stocks in user article");
                }
            }

            return relatedStocks;
        }

        public void AddRelatedStocksForArticle(string articleId, List<string> stockNames, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            if (stockNames == null || stockNames.Count == 0)
            {
                return;
            }

            bool ownConnection = false;
            try
            {
                // no connection was provided, create our own (idk how this works but it does, no need to change it yes)
                if (connection == null)
                {
                    connection = DatabaseHelper.GetConnection();
                    ownConnection = true;
                    // new transaction if one wasn't provided
                    if (transaction == null)
                    {
                        transaction = connection.BeginTransaction();
                    }
                }

                foreach (var inputStockName in stockNames)
                {
                    // check if the stock exists in the STOCK table by name
                    string actualStockName = inputStockName;
                    bool stockExists = false;

                    using (var stockCheckCommand = new SqlCommand("SELECT STOCK_NAME FROM STOCK WHERE STOCK_NAME = @StockName", connection, transaction))
                    {
                        stockCheckCommand.CommandTimeout = 30;
                        stockCheckCommand.Parameters.AddWithValue("@StockName", inputStockName);
                        var result = stockCheckCommand.ExecuteScalar();
                        stockExists = result != null;
                    }

                    // not found by name, try to find by symbol
                    if (!stockExists)
                    {
                        using (var symbolCheckCommand = new SqlCommand("SELECT STOCK_NAME FROM STOCK WHERE STOCK_SYMBOL = @StockSymbol", connection, transaction))
                        {
                            symbolCheckCommand.CommandTimeout = 30;
                            symbolCheckCommand.Parameters.AddWithValue("@StockSymbol", inputStockName);
                            var result = symbolCheckCommand.ExecuteScalar();

                            if (result != null)
                            {
                                stockExists = true;
                                actualStockName = result.ToString(); // Use the actual stock name from the database
                            }
                        }
                    }

                    // stock doesn't exist at all, create it with default values
                    if (!stockExists)
                    {
                        string stockSymbol = inputStockName.Length <= 5 ? inputStockName.ToUpper() : inputStockName.Substring(0, 5).ToUpper();
                        string authorCnp = "1234567890123"; // Default author CNP

                        using (var createStockCommand = new SqlCommand("INSERT INTO STOCK (STOCK_NAME, STOCK_SYMBOL, AUTHOR_CNP) VALUES (@StockName, @StockSymbol, @AuthorCNP)", connection, transaction))
                        {
                            createStockCommand.CommandTimeout = 30;
                            createStockCommand.Parameters.AddWithValue("@StockName", inputStockName);
                            createStockCommand.Parameters.AddWithValue("@StockSymbol", stockSymbol);
                            createStockCommand.Parameters.AddWithValue("@AuthorCNP", authorCnp);
                            createStockCommand.ExecuteNonQuery();
                        }

                        // add an initial price in STOCK_VALUE
                        using (var valueCommand = new SqlCommand("INSERT INTO STOCK_VALUE (STOCK_NAME, PRICE) VALUES (@StockName, @Price)", connection, transaction))
                        {
                            valueCommand.CommandTimeout = 30;
                            valueCommand.Parameters.AddWithValue("@StockName", inputStockName);
                            valueCommand.Parameters.AddWithValue("@Price", 100); // Default initial price
                            valueCommand.ExecuteNonQuery();
                        }

                        actualStockName = inputStockName;
                    }

                    // check if the related stock entry already exists
                    bool relatedStockExists = false;
                    using (var checkCommand = new SqlCommand("SELECT COUNT(*) FROM RELATED_STOCKS WHERE STOCK_NAME = @StockName AND ARTICLE_ID = @ArticleId", connection, transaction))
                    {
                        checkCommand.CommandTimeout = 30;
                        checkCommand.Parameters.AddWithValue("@StockName", actualStockName);
                        checkCommand.Parameters.AddWithValue("@ArticleId", articleId);
                        relatedStockExists = Convert.ToInt32(checkCommand.ExecuteScalar()) > 0;
                    }

                    // the related stock entry doesn't exist, create it
                    if (!relatedStockExists)
                    {
                        using (var command = new SqlCommand("INSERT INTO RELATED_STOCKS (STOCK_NAME, ARTICLE_ID) VALUES (@StockName, @ArticleId)", connection, transaction))
                        {
                            command.CommandTimeout = 30;
                            command.Parameters.AddWithValue("@StockName", actualStockName);
                            command.Parameters.AddWithValue("@ArticleId", articleId);
                            command.ExecuteNonQuery();
                        }
                    }
                }

                // commit if we created the transaction
                if (ownConnection && transaction != null)
                {
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                // rollback if we created the transaction
                if (ownConnection && transaction != null)
                {
                    try { transaction.Rollback(); } catch { /* Ignore rollback errors */ }
                }
                System.Diagnostics.Debug.WriteLine($"Error adding related stocks: {ex.Message}");
                throw;
            }
            finally
            {
                // dispose if we created the connection
                if (ownConnection && connection != null)
                {
                    connection.Close();
                }
            }
        }

        public void AddNewsArticle(NewsArticle newsArticle)
        {
            lock (LockObject)
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    bool exists = false;
                    using (var checkCommand = new SqlCommand("SELECT COUNT(*) FROM NEWS_ARTICLE WHERE ARTICLE_ID = @ArticleId", connection))
                    {
                        checkCommand.CommandTimeout = 30;
                        checkCommand.Parameters.AddWithValue("@ArticleId", newsArticle.ArticleId);
                        exists = Convert.ToInt32(checkCommand.ExecuteScalar()) > 0;
                    }

                    if (exists)
                    {
                        this.UpdateNewsArticle(newsArticle);
                        return;
                    }

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string query = "INSERT INTO NEWS_ARTICLE (ARTICLE_ID, TITLE, SUMMARY, CONTENT, SOURCE, PUBLISH_DATE, IS_READ, IS_WATCHLIST_RELATED, CATEGORY) VALUES (@ArticleId, @Title, @Summary, @Content, @Source, @PublishedDate, @IsRead, @IsWatchlistRelated, @Category)";
                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.CommandTimeout = 30;
                                command.Parameters.AddWithValue("@ArticleId", newsArticle.ArticleId);
                                command.Parameters.AddWithValue("@Title", newsArticle.Title);
                                command.Parameters.AddWithValue("@Summary", newsArticle.Summary ?? "");
                                command.Parameters.AddWithValue("@Content", newsArticle.Content);
                                command.Parameters.AddWithValue("@Source", newsArticle.Source ?? "");
                                command.Parameters.AddWithValue("@PublishedDate", newsArticle.PublishedDate);
                                command.Parameters.AddWithValue("@IsRead", newsArticle.IsRead);
                                command.Parameters.AddWithValue("@IsWatchlistRelated", newsArticle.IsWatchlistRelated);
                                command.Parameters.AddWithValue("@Category", newsArticle.Category ?? "");
                                command.ExecuteNonQuery();
                            }

                            if (newsArticle.RelatedStocks != null && newsArticle.RelatedStocks.Count > 0)
                            {
                                this.AddRelatedStocksForArticle(newsArticle.ArticleId, newsArticle.RelatedStocks, connection, transaction);
                            }

                            transaction.Commit();

                            // to in-memory collection if not already there
                            if (!this.newsArticles.Any(a => a.ArticleId == newsArticle.ArticleId))
                            {
                                this.newsArticles.Add(newsArticle);
                            }
                        }
                        catch (Exception ex)
                        {
                            try { transaction.Rollback(); } catch { /* Ignore rollback errors */ }
                            System.Diagnostics.Debug.WriteLine($"Error adding news article: {ex.Message}");
                            throw;
                        }
                    }
                }
            }
        }

        public void UpdateNewsArticle(NewsArticle newsArticle)
        {
            lock (LockObject)
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string query = "UPDATE NEWS_ARTICLE SET TITLE = @Title, SUMMARY = @Summary, CONTENT = @Content, SOURCE = @Source, PUBLISH_DATE = @PublishedDate, IS_READ = @IsRead, IS_WATCHLIST_RELATED = @IsWatchlistRelated, CATEGORY = @Category WHERE ARTICLE_ID = @ArticleId";
                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.CommandTimeout = 30;
                                command.Parameters.AddWithValue("@ArticleId", newsArticle.ArticleId);
                                command.Parameters.AddWithValue("@Title", newsArticle.Title);
                                command.Parameters.AddWithValue("@Summary", newsArticle.Summary ?? "");
                                command.Parameters.AddWithValue("@Content", newsArticle.Content);
                                command.Parameters.AddWithValue("@Source", newsArticle.Source ?? "");
                                command.Parameters.AddWithValue("@PublishedDate", newsArticle.PublishedDate);
                                command.Parameters.AddWithValue("@IsRead", newsArticle.IsRead);
                                command.Parameters.AddWithValue("@IsWatchlistRelated", newsArticle.IsWatchlistRelated);
                                command.Parameters.AddWithValue("@Category", newsArticle.Category ?? "");
                                command.ExecuteNonQuery();
                            }

                            using (var command = new SqlCommand("DELETE FROM RELATED_STOCKS WHERE ARTICLE_ID = @ArticleId", connection, transaction))
                            {
                                command.CommandTimeout = 30;
                                command.Parameters.AddWithValue("@ArticleId", newsArticle.ArticleId);
                                command.ExecuteNonQuery();
                            }

                            if (newsArticle.RelatedStocks != null && newsArticle.RelatedStocks.Count > 0)
                            {
                                this.AddRelatedStocksForArticle(newsArticle.ArticleId, newsArticle.RelatedStocks, connection, transaction);
                            }

                            transaction.Commit();

                            var existingArticle = this.newsArticles.FirstOrDefault(a => a.ArticleId == newsArticle.ArticleId);
                            if (existingArticle != null)
                            {
                                int index = this.newsArticles.IndexOf(existingArticle);
                                this.newsArticles[index] = newsArticle;
                            }
                            else
                            {
                                this.newsArticles.Add(newsArticle);
                            }
                        }
                        catch (Exception ex)
                        {
                            try { transaction.Rollback(); } catch { /* Ignore rollback errors */ }
                            System.Diagnostics.Debug.WriteLine($"Error updating news article: {ex.Message}");
                            throw;
                        }
                    }
                }
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

        public NewsArticle GetNewsArticleById(string articleId)
        {
            if (string.IsNullOrWhiteSpace(articleId))
                throw new ArgumentNullException(nameof(articleId));

            var article = this.newsArticles.FirstOrDefault(a => a.ArticleId == articleId)
                ?? throw new KeyNotFoundException($"Article with ID {articleId} not found");

            if (article.RelatedStocks == null || !article.RelatedStocks.Any())
            {
                // related stocks are loaded
                article.RelatedStocks = this.GetRelatedStocksForArticle(articleId);
                System.Diagnostics.Debug.WriteLine($"GetNewsArticleById: Loaded {article.RelatedStocks?.Count ?? 0} related stocks for article {articleId}");
            }
            return article;
        }

        /// <summary>
        /// Retrieves all news articles.
        /// </summary>
        /// <returns>A list of all news articles.</returns>
        public List<NewsArticle> GetAllNewsArticles() => [.. this.newsArticles];


        public List<NewsArticle> GetNewsArticlesByStock(string stockName)
        {
            return this.newsArticles.Where(a => a.RelatedStocks.Contains(stockName)).ToList();
        }

        public List<NewsArticle> GetNewsArticlesByCategory(string category)
        {
            return this.newsArticles.Where(a => a.Category == category).ToList();
        }

        /// <summary>
        /// Marks a news article as read.
        /// </summary>
        /// <param name="articleId">The ID of the article to mark as read.</param>
        public void MarkArticleAsRead(string articleId)
        {
            if (string.IsNullOrWhiteSpace(articleId))
                throw new ArgumentNullException(nameof(articleId));

            var article = this.GetNewsArticleById(articleId);
            article.IsRead = true;
            this.UpdateNewsArticle(article);
        }


        public void LoadUserArticles()
        {
            this.userArticles.Clear();
            using (var connection = DatabaseHelper.GetConnection())
            {
                using (var command = new SqlCommand("SELECT * FROM USER_ARTICLE", connection))
                {
                    command.CommandTimeout = 30;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string authorCNP = reader["AUTHOR_CNP"].ToString();

                            User author = new()
                            {
                                CNP = authorCNP,
                                Username = reader["AUTHOR_NAME"].ToString(),
                                Description = reader["AUTHOR_DESCRIPTION"].ToString(),
                                IsModerator = reader.GetBoolean(5),
                                IsHidden = reader.GetBoolean(6),
                                Image = reader["AUTHOR_PROFILE_PICTURE"].ToString(),
                                GemBalance = reader.GetInt32(7),
                            };

                            var article = new UserArticle
                            {
                                ArticleId = reader["ARTICLE_ID"].ToString(),
                                Title = reader["TITLE"].ToString(),
                                Summary = reader["SUMMARY"].ToString(),
                                Content = reader["CONTENT"].ToString(),
                                Author = author,
                                SubmissionDate = reader["SUBMISSION_DATE"] is DateTime dateTime ? dateTime : DateTime.Now,
                                Status = reader["STATUS"].ToString(),
                                Topic = reader["TOPIC"].ToString(),
                                RelatedStocks = this.GetRelatedStocksForArticle(reader["ARTICLE_ID"].ToString()),
                            };

                            this.userArticles.Add(article);
                        }
                    }
                }
            }
        }

        public void AddUserArticle(UserArticle userArticle)
        {
            lock (LockObject)
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    bool exists = false;
                    using (var checkCommand = new SqlCommand("SELECT COUNT(*) FROM USER_ARTICLE WHERE ARTICLE_ID = @ArticleId", connection))
                    {
                        checkCommand.CommandTimeout = 30;
                        checkCommand.Parameters.AddWithValue("@ArticleId", userArticle.ArticleId);
                        exists = Convert.ToInt32(checkCommand.ExecuteScalar()) > 0;
                    }

                    if (exists)
                    {
                        this.UpdateUserArticle(userArticle);
                        return;
                    }

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // First, add the user article
                            using (var command = new SqlCommand("INSERT INTO USER_ARTICLE (ARTICLE_ID, TITLE, SUMMARY, CONTENT, AUTHOR_CNP, SUBMISSION_DATE, STATUS, TOPIC) VALUES (@ArticleId, @Title, @Summary, @Content, @AuthorCNP, @SubmissionDate, @Status, @Topic)", connection, transaction))
                            {
                                command.CommandTimeout = 30;
                                command.Parameters.AddWithValue("@ArticleId", userArticle.ArticleId);
                                command.Parameters.AddWithValue("@Title", userArticle.Title);
                                command.Parameters.AddWithValue("@Summary", userArticle.Summary ?? "");
                                command.Parameters.AddWithValue("@Content", userArticle.Content);
                                command.Parameters.AddWithValue("@AuthorCNP", userArticle.Author);
                                command.Parameters.AddWithValue("@SubmissionDate", userArticle.SubmissionDate.ToString("yyyy-MM-dd HH:mm:ss"));
                                command.Parameters.AddWithValue("@Status", userArticle.Status);
                                command.Parameters.AddWithValue("@Topic", userArticle.Topic);
                                command.ExecuteNonQuery();
                            }

                            // IMPORTANT: Always add an entry to the NEWS_ARTICLE table regardless of status
                            // This ensures we can add related stocks that reference this article
                            using (var command = new SqlCommand("INSERT INTO NEWS_ARTICLE (ARTICLE_ID, TITLE, SUMMARY, CONTENT, SOURCE, PUBLISH_DATE, IS_READ, IS_WATCHLIST_RELATED, CATEGORY) VALUES (@ArticleId, @Title, @Summary, @Content, @Source, @PublishedDate, @IsRead, @IsWatchlistRelated, @Category)", connection, transaction))
                            {
                                command.CommandTimeout = 30;
                                command.Parameters.AddWithValue("@ArticleId", userArticle.ArticleId);
                                command.Parameters.AddWithValue("@Title", userArticle.Title);
                                command.Parameters.AddWithValue("@Summary", userArticle.Summary ?? "");
                                command.Parameters.AddWithValue("@Content", userArticle.Content);
                                command.Parameters.AddWithValue("@Source", $"User: {userArticle.Author}");
                                command.Parameters.AddWithValue("@PublishedDate", userArticle.SubmissionDate.ToString("MMMM dd, yyyy"));
                                command.Parameters.AddWithValue("@IsRead", false);
                                command.Parameters.AddWithValue("@IsWatchlistRelated", false);
                                command.Parameters.AddWithValue("@Category", userArticle.Topic ?? "");
                                command.ExecuteNonQuery();
                            }

                            // Now add related stocks if there are any
                            if (userArticle.RelatedStocks != null && userArticle.RelatedStocks.Count > 0)
                            {
                                this.AddRelatedStocksForArticle(userArticle.ArticleId, userArticle.RelatedStocks, connection, transaction);
                            }

                            transaction.Commit();

                            // Add to in-memory collection if not already there
                            if (!this.userArticles.Any(a => a.ArticleId == userArticle.ArticleId))
                            {
                                this.userArticles.Add(userArticle);
                            }
                        }
                        catch (Exception ex)
                        {
                            try { transaction.Rollback(); } catch { /* Ignore rollback errors */ }
                            System.Diagnostics.Debug.WriteLine($"Error adding user article: {ex.Message}");
                            throw;
                        }
                    }
                }
            }
        }

        ///// <summary>
        ///// Adds or updates a user-submitted article in the database.
        ///// </summary>
        ///// <param name="userArticle">The user article to add or update.</param>
        //public void AddOrUpdateUserArticle(UserArticle userArticle)
        //{
        //    if (this.ArticleExists(userArticle.ArticleId, "USER_ARTICLE"))
        //    {
        //        this.UpdateArticle(userArticle, "USER_ARTICLE", MapUserArticleParameters);
        //    }
        //    else
        //    {
        //        this.AddArticle(userArticle, "USER_ARTICLE", MapUserArticleParameters);
        //    }
        //}

        public void UpdateUserArticle(UserArticle userArticle)
        {
            lock (LockObject)
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string query = "UPDATE USER_ARTICLE SET TITLE = @Title, SUMMARY = @Summary, CONTENT = @Content, AUTHOR_CNP = @AuthorCNP, SUBMISSION_DATE = @SubmissionDate, STATUS = @Status, TOPIC = @Topic WHERE ARTICLE_ID = @ArticleId";
                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.CommandTimeout = 30;
                                command.Parameters.AddWithValue("@ArticleId", userArticle.ArticleId);
                                command.Parameters.AddWithValue("@Title", userArticle.Title);
                                command.Parameters.AddWithValue("@Summary", userArticle.Summary ?? "");
                                command.Parameters.AddWithValue("@Content", userArticle.Content);
                                command.Parameters.AddWithValue("@AuthorCNP", userArticle.Author);
                                command.Parameters.AddWithValue("@SubmissionDate", userArticle.SubmissionDate.ToString("yyyy-MM-dd HH:mm:ss"));
                                command.Parameters.AddWithValue("@Status", userArticle.Status);
                                command.Parameters.AddWithValue("@Topic", userArticle.Topic);
                                command.ExecuteNonQuery();
                            }

                            using (var command = new SqlCommand("DELETE FROM RELATED_STOCKS WHERE ARTICLE_ID = @ArticleId", connection, transaction))
                            {
                                command.CommandTimeout = 30;
                                command.Parameters.AddWithValue("@ArticleId", userArticle.ArticleId);
                                command.ExecuteNonQuery();
                            }

                            if (userArticle.RelatedStocks != null && userArticle.RelatedStocks.Count > 0)
                            {
                                this.AddRelatedStocksForArticle(userArticle.ArticleId, userArticle.RelatedStocks, connection, transaction);
                            }

                            transaction.Commit();

                            var existingArticle = this.userArticles.FirstOrDefault(a => a.ArticleId == userArticle.ArticleId);
                            if (existingArticle != null)
                            {
                                int index = this.userArticles.IndexOf(existingArticle);
                                this.userArticles[index] = userArticle;
                            }
                            else
                            {
                                this.userArticles.Add(userArticle);
                            }
                        }
                        catch (Exception ex)
                        {
                            try { transaction.Rollback(); } catch { /* Ignore rollback errors */ }
                            System.Diagnostics.Debug.WriteLine($"Error updating user article: {ex.Message}");
                            throw;
                        }
                    }
                }
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
            return this.userArticles.FirstOrDefault(a => a.ArticleId == articleId);
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
            if (article != null)
            {
                article.Status = "Approved";
                this.UpdateUserArticle(article);

                // Create a news article from the approved user article
                var newsArticle = new NewsArticle
                {
                    ArticleId = article.ArticleId,
                    Title = article.Title,
                    Summary = article.Summary ?? "",
                    Content = article.Content,
                    Source = $"User: {article.Author}",
                    PublishedDate = article.SubmissionDate.ToString("MMMM dd, yyyy"),
                    IsRead = false,
                    IsWatchlistRelated = false,
                    Category = article.Topic,
                    RelatedStocks = article.RelatedStocks
                };

                // Check if the news article already exists
                var existingNewsArticle = this.GetNewsArticleById(article.ArticleId);
                if (existingNewsArticle == null)
                {
                    this.AddNewsArticle(newsArticle);
                }
                else
                {
                    this.UpdateNewsArticle(newsArticle);
                }
            }
        }

        /// <summary>
        /// Rejects a user-submitted article and removes it from the news articles if it exists.
        /// </summary>
        /// <param name="articleId">The ID of the article to reject.</param>
        public void RejectUserArticle(string articleId)
        {
            if (string.IsNullOrWhiteSpace(articleId))
                throw new ArgumentNullException(nameof(articleId));

            var article = this.GetUserArticleById(articleId)
                ?? throw new KeyNotFoundException($"User article with ID {articleId} not found");

            article.Status = "Rejected";
            this.UpdateUserArticle(article);

            try
            {
                var existingNewsArticle = this.GetNewsArticleById(article.ArticleId);
                this.DeleteNewsArticle(article.ArticleId);
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