namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using StockApp.Exceptions;
    using StockApp.Models;
    using StockApp.Repositories;

    /// <summary>
    /// Provides business logic for managing news and user-submitted articles.
    /// </summary>
    public class NewsService : INewsService
    {
        private static readonly IUserRepository UserRepo = new UserRepository();
        private static readonly Dictionary<string, NewsArticle> previewArticles = [];
        private static readonly Dictionary<string, UserArticle> previewUserArticles = [];
        private readonly List<NewsArticle> cachedArticles = [];
        private static List<UserArticle> userArticles = [];
        private static bool isInitialized = false;
        private readonly INewsRepository repository;
        private readonly IBaseStocksRepository stocksRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewsService"/> class
        /// with default repository implementations.
        /// </summary>
        public NewsService() : this(new NewsRepository(), new BaseStocksRepository())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewsService"/> class
        /// using the specified repositories.
        /// </summary>
        /// <param name="repository">The news repository.</param>
        /// <param name="stocksRepository">The base stocks repository.</param>
        public NewsService(INewsRepository repository, IBaseStocksRepository stocksRepository)
        {
            this.repository = repository;
            this.stocksRepository = stocksRepository; // FIXME: stocksRepository is never used in this class.

            if (!isInitialized)
            {
                // Load initial user-submitted articles into memory
                var initialUserArticles = this.repository.GetAllUserArticles() ?? [];
                userArticles.AddRange(initialUserArticles);
                isInitialized = true;
            }
        }

        /// <summary>
        /// Retrieves and caches all news articles.
        /// </summary>
        /// <returns>A list of <see cref="NewsArticle"/> instances.</returns>
        /// <exception cref="NewsPersistenceException">Thrown if retrieval fails.</exception>
        public List<NewsArticle> GetNewsArticles()
        {
            try
            {
                // Fetch articles in a background thread
                var articles = this.repository.GetAllNewsArticles();

                // Refresh cache
                this.cachedArticles.Clear();
                this.cachedArticles.AddRange(articles);

                // Inline: For each article, populate its related stocks
                foreach (var article in articles)
                {
                    article.RelatedStocks = NewsRepository.GetRelatedStocksForArticle(article.ArticleId);
                }

                return articles;
            }
            catch (NewsPersistenceException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to retrieve news articles: {ex.Message}");
                throw new NewsPersistenceException("Failed to retrieve news articles.", ex);
            }
        }

        /// <summary>
        /// Retrieves a news article by ID, checking preview cache first.
        /// </summary>
        /// <param name="articleId">The ID of the article to fetch.</param>
        /// <returns>The requested <see cref="NewsArticle"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="articleId"/> is null or empty.</exception>
        /// <exception cref="KeyNotFoundException">If no article matches the ID.</exception>
        /// <exception cref="NewsPersistenceException">If retrieval fails.</exception>
        public NewsArticle GetNewsArticleById(string articleId)
        {
            if (string.IsNullOrWhiteSpace(articleId))
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            // Return from preview cache if available
            if (previewArticles.TryGetValue(articleId, out var previewArticle))
            {
                return previewArticle;
            }

            try
            {
                var article = this.repository.GetNewsArticleById(articleId);
                return article ?? throw new KeyNotFoundException($"Article with ID {articleId} not found");
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException($"Failed to load article with ID {articleId}.", ex);
            }
        }

        /// <summary>
        /// Marks the specified article as read.
        /// </summary>
        /// <param name="articleId">The ID of the article to mark.</param>
        /// <returns>True if successful.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="articleId"/> is null or empty.</exception>
        /// <exception cref="NewsPersistenceException">If marking fails.</exception>
        public bool MarkArticleAsRead(string articleId)
        {
            if (string.IsNullOrWhiteSpace(articleId))
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            try
            {
                this.repository.MarkArticleAsRead(articleId);
                return true;
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException($"Failed to mark article {articleId} as read.", ex);
            }
        }

        /// <summary>
        /// Creates a new news article.
        /// </summary>
        /// <param name="article">The article to create.</param>
        /// <returns>True if creation succeeded.</returns>
        /// <exception cref="UnauthorizedAccessException">If no user is logged in.</exception>
        /// <exception cref="NewsPersistenceException">If creation fails.</exception>
        public bool CreateArticle(NewsArticle article)
        {
            if (UserRepo.CurrentUserCNP == null)
            {
                throw new UnauthorizedAccessException("User must be logged in to create an article");
            }

            try
            {
                this.repository.AddNewsArticle(article);
                return true;
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException("Failed to create news article.", ex);
            }
        }

        /// <summary>
        /// Retrieves user-submitted articles, optionally filtering by status and topic.
        /// </summary>
        /// <param name="status">The status to filter by (or null for all).</param>
        /// <param name="topic">The topic to filter by (or null for all).</param>
        /// <returns>A list of <see cref="UserArticle"/> instances.</returns>
        /// <exception cref="UnauthorizedAccessException">If current user is not an admin.</exception>
        /// <exception cref="NewsPersistenceException">If loading fails.</exception>
        public List<UserArticle> GetUserArticles(string status = null, string topic = null)
        {
            if (UserRepo.CurrentUserCNP == null)
            {
                throw new UnauthorizedAccessException("User must be logged in to access user articles");
            }

            User user = UserRepo.GetUserByCnpAsync(UserRepo.CurrentUserCNP).Result;
            if (user.IsModerator)
            {
                throw new UnauthorizedAccessException("User must be an admin to access user articles");
            }

            try
            {
                // Reload from repository
                userArticles = this.repository.GetAllUserArticles();

                // Inline: apply status filter
                if (!string.IsNullOrEmpty(status) && status != "All")
                {
                    userArticles = [.. userArticles.Where(a => a.Status == status)];
                }

                // Inline: apply topic filter
                if (!string.IsNullOrEmpty(topic) && topic != "All")
                {
                    userArticles = [.. userArticles.Where(a => a.Topic == topic)];
                }

                return userArticles;
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException("Failed to load user articles.", ex);
            }
        }

        /// <summary>
        /// Approves a pending user article.
        /// </summary>
        /// <param name="articleId">The ID of the article to approve.</param>
        /// <returns>True if approval succeeded.</returns>
        /// <exception cref="UnauthorizedAccessException">If current user is not an admin.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="articleId"/> is null or empty.</exception>
        /// <exception cref="NewsPersistenceException">If approval fails.</exception>
        public bool ApproveUserArticle(string articleId)
        {
            if (UserRepo.CurrentUserCNP == null)
            {
                throw new UnauthorizedAccessException("User must be logged in to access user articles");
            }

            User user = UserRepo.GetUserByCnpAsync(UserRepo.CurrentUserCNP).Result;
            if (user.IsModerator)
            {
                throw new UnauthorizedAccessException("User must be an admin to access user articles");
            }

            if (string.IsNullOrWhiteSpace(articleId))
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            try
            {
                this.repository.ApproveUserArticle(articleId);
                this.cachedArticles.Clear(); // Invalidate cache after approval
                return true;
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException($"Failed to approve article {articleId}.", ex);
            }
        }

        /// <summary>
        /// Rejects a pending user article.
        /// </summary>
        /// <param name="articleId">The ID of the article to reject.</param>
        /// <returns>True if rejection succeeded.</returns>
        /// <exception cref="UnauthorizedAccessException">If current user is not an admin.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="articleId"/> is null or empty.</exception>
        /// <exception cref="NewsPersistenceException">If rejection fails.</exception>
        public bool RejectUserArticle(string articleId)
        {
            if (UserRepo.CurrentUserCNP == null)
            {
                throw new UnauthorizedAccessException("User must be logged in to access user articles");
            }

            User user = UserRepo.GetUserByCnpAsync(UserRepo.CurrentUserCNP).Result;
            if (user.IsModerator)
            {
                throw new UnauthorizedAccessException("User must be an admin to access user articles");
            }

            if (string.IsNullOrWhiteSpace(articleId))
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            try
            {
                this.repository.RejectUserArticle(articleId);
                this.cachedArticles.Clear(); // Invalidate cache after rejection
                return true;
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException($"Failed to reject article {articleId}.", ex);
            }
        }

        /// <summary>
        /// Deletes both a user-submitted article and its corresponding news article.
        /// </summary>
        /// <param name="articleId">The ID of the article to delete.</param>
        /// <returns>True if deletion succeeded.</returns>
        /// <exception cref="UnauthorizedAccessException">If current user is not an admin.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="articleId"/> is null or empty.</exception>
        /// <exception cref="NewsPersistenceException">If deletion fails.</exception>
        public bool DeleteUserArticle(string articleId)
        {
            if (UserRepo.CurrentUserCNP == null)
            {
                throw new UnauthorizedAccessException("User must be logged in to access user articles");
            }

            User user = UserRepo.GetUserByCnpAsync(UserRepo.CurrentUserCNP).Result;
            if (user.IsModerator)
            {
                throw new UnauthorizedAccessException("User must be an admin to access user articles");
            }

            if (string.IsNullOrWhiteSpace(articleId))
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            try
            {
                // Remove user article and its published counterpart
                this.repository.DeleteUserArticle(articleId);
                this.repository.DeleteNewsArticle(articleId);
                this.cachedArticles.Clear(); // Invalidate cache after deletion
                return true;
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException($"Failed to delete article {articleId}.", ex);
            }
        }

        /// <summary>
        /// Submits a user article for review.
        /// </summary>
        /// <param name="article">The user article to submit.</param>
        /// <returns>True if submission succeeded.</returns>
        /// <exception cref="UnauthorizedAccessException">If no user is logged in.</exception>
        /// <exception cref="NewsPersistenceException">If submission fails.</exception>
        public bool SubmitUserArticle(UserArticle article)
        {
            if (UserRepo.CurrentUserCNP == null)
            {
                throw new UnauthorizedAccessException("User must be logged in to access user articles");
            }

            User user = UserRepo.GetUserByCnpAsync(UserRepo.CurrentUserCNP).Result;
            if (user.IsModerator)
            {
                throw new UnauthorizedAccessException("User must be an admin to access user articles");
            }

            // Inline: set author and metadata
            article.Author = user;
            article.SubmissionDate = DateTime.Now;
            article.Status = "Pending";

            try
            {
                this.repository.AddUserArticle(article);
                this.cachedArticles.Clear(); // Invalidate cache after submission
                return true;
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException("Failed to submit user article.", ex);
            }
        }

        /// <summary>
        /// Gets the currently logged-in user from application state.
        /// </summary>
        /// <returns>The current <see cref="User"/>.</returns>
        /// <exception cref="InvalidOperationException">If no user is logged in.</exception>
        public User GetCurrentUser()
        {
            if (UserRepo.CurrentUserCNP == null)
            {
                throw new UnauthorizedAccessException("User must be logged in to access user articles");
            }

            User user = UserRepo.GetUserByCnpAsync(UserRepo.CurrentUserCNP).Result;
            if (user.IsModerator)
            {
                throw new UnauthorizedAccessException("User must be an admin to access user articles");
            }

            return user;
        }

        /// <summary>
        /// Authenticates a user by username and password.
        /// </summary>
        /// <param name="username">The username to authenticate.</param>
        /// <param name="password">The password to verify.</param>
        /// <returns>The authenticated <see cref="User"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="username"/> or <paramref name="password"/> is null or empty.</exception>
        /// <exception cref="UnauthorizedAccessException">If credentials are invalid.</exception>
        public async Task<User> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            await Task.Delay(300); // TODO: Replace with real authentication call

            if (username == "admin" && password == "admin")
            {
                throw new Exception("UNIMPLEMENTED FIX THIS");
                string adminCnp = "6666666666666";
                try
                {
                    this.repository.EnsureUserExists(
                        adminCnp,
                        "admin",
                        "Administrator Account",
                        true,  // isAdmin
                        false, // isHidden
                        "img.jpg");
                }
                catch (NewsPersistenceException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error ensuring admin user exists: {ex.Message}");
                }

                //return new User(
                //    adminCnp,
                //    "admin",
                //    "Administrator Account",
                //    true,
                //    "img.jpg",
                //    false,
                //    0);
            }
            else if (password == "user")
            {
                User user = UserRepo.GetUserByUsernameAsync(username).Result;
                if (user == null)
                {
                    throw new UnauthorizedAccessException("Invalid username or password");
                }

                UserRepo.CurrentUserCNP = user.CNP;
                return user;
            }

            throw new UnauthorizedAccessException("Invalid username or password");
        }

        /// <summary>
        /// Logs out the current user and clears preview caches.
        /// </summary>
        public void Logout()
        {
            previewArticles.Clear();
            previewUserArticles.Clear();
        }

        /// <summary>
        /// Stores a preview of a news article and its corresponding user article.
        /// </summary>
        /// <param name="article">The news article preview.</param>
        /// <param name="userArticle">The user article preview.</param>
        /// <remarks>
        /// Strips any "preview:" prefix from <c>ArticleId</c> before storage.
        /// </remarks>
        /// <exception cref="NewsPersistenceException">If persisting related stocks fails.</exception>
        public void StorePreviewArticle(NewsArticle article, UserArticle userArticle)
        {
            // Inline: normalize ID by removing "preview:" prefix if present
            string articleId = article.ArticleId.StartsWith("preview:") ? article.ArticleId[8..] : article.ArticleId;
            article.ArticleId = articleId;

            // Inline: copy related stocks list if provided
            article.RelatedStocks = userArticle.RelatedStocks != null
                ? [.. userArticle.RelatedStocks]
                : [];

            previewArticles[articleId] = article;
            previewUserArticles[articleId] = userArticle;

            if (article.RelatedStocks?.Count > 0)
            {
                try
                {
                    this.repository.AddRelatedStocksForArticle(articleId, article.RelatedStocks, null, null);
                }
                catch (NewsPersistenceException ex)
                {
                    throw new NewsPersistenceException(
                        $"Failed to persist related stocks for preview article '{articleId}'.", ex);
                }
            }
        }

        /// <summary>
        /// Retrieves a user article for preview by its ID.
        /// </summary>
        /// <param name="articleId">The ID of the preview article.</param>
        /// <returns>The <see cref="UserArticle"/> if found; otherwise null.</returns>
        public UserArticle GetUserArticleForPreview(string articleId)
        {
            if (previewUserArticles.TryGetValue(articleId, out var previewArticle))
            {
                return previewArticle;
            }

            // Inline: fallback to main list
            return userArticles.FirstOrDefault(a => a.ArticleId == articleId);
        }

        /// <summary>
        /// Gets related stock symbols for the specified article.
        /// </summary>
        /// <param name="articleId">The ID of the article.</param>
        /// <returns>A list of related stock symbols.</returns>
        /// <exception cref="NewsPersistenceException">If retrieval fails.</exception>
        public List<string> GetRelatedStocksForArticle(string articleId)
        {
            // Inline: normalize ID
            string actualId = articleId.StartsWith("preview:") ? articleId[8..] : articleId;

            // Return preview stocks if available
            if (previewUserArticles.TryGetValue(actualId, out var previewUserArticle) &&
                previewUserArticle.RelatedStocks != null &&
                previewUserArticle.RelatedStocks.Any())
            {
                return previewUserArticle.RelatedStocks;
            }

            try
            {
                var stocks = NewsRepository.GetRelatedStocksForArticle(actualId);
                System.Diagnostics.Debug.WriteLine($"GetRelatedStocksForArticle: Found {stocks.Count} stocks");
                return stocks;
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException(
                    $"Failed to retrieve related stocks for article '{actualId}'.", ex);
            }
        }

        /// <summary>
        /// Updates the internal cache of news articles.
        /// </summary>
        /// <param name="articles">The new list of articles to cache.</param>
        public void UpdateCachedArticles(List<NewsArticle> articles)
        {
            this.cachedArticles.Clear();
            if (articles != null)
            {
                this.cachedArticles.AddRange(articles);
            }
        }

        /// <summary>
        /// Gets the current cache of news articles, or fetches from repository if empty.
        /// </summary>
        /// <returns>A list of <see cref="NewsArticle"/>.</returns>
        public List<NewsArticle> GetCachedArticles()
        {
            return this.cachedArticles.Count > 0
                ? this.cachedArticles
                : this.repository.GetAllNewsArticles();
        }
    }
}
