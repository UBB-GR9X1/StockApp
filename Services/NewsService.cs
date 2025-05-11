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
        private readonly IUserRepository userRepository;
        private readonly INewsRepository newsRepository;
        private readonly IBaseStocksService stocksService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewsService"/> class
        /// using the specified repositories.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="stocksService">The stocks service.</param>
        public NewsService(IUserRepository userRepository, IBaseStocksService baseStocksService, INewsRepository newsRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.stocksService = baseStocksService ?? throw new ArgumentNullException(nameof(baseStocksService));
            this.newsRepository = newsRepository ?? throw new ArgumentNullException(nameof(newsRepository));
        }

        /// <summary>
        /// Retrieves and caches all news articles.
        /// </summary>
        /// <returns>A list of <see cref="NewsArticle"/> instances.</returns>
        /// <exception cref="NewsPersistenceException">Thrown if retrieval fails.</exception>
        public async Task<List<NewsArticle>> GetNewsArticlesAsync()
        {
            try
            {
                // Fetch articles in a background thread
                List<NewsArticle> articles = await this.newsRepository.GetAllNewsArticlesAsync();

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
        public async Task<NewsArticle> GetNewsArticleByIdAsync(string articleId)
        {
            if (string.IsNullOrWhiteSpace(articleId))
            {
                throw new ArgumentNullException(nameof(articleId));
            }
            try
            {
                NewsArticle article = await this.newsRepository.GetNewsArticleByIdAsync(articleId);
                return article;
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
        public async Task<bool> MarkArticleAsReadAsync(string articleId)
        {
            if (string.IsNullOrWhiteSpace(articleId))
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            try
            {
                await this.newsRepository.MarkArticleAsReadAsync(articleId);
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
        public async Task<bool> CreateArticleAsync(NewsArticle article)
        {
            if (IUserRepository.IsGuest)
            {
                throw new UnauthorizedAccessException("User must be logged in to create an article");
            }

            try
            {
                await this.newsRepository.AddNewsArticleAsync(article);
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
        /// <param name="authorCNP">Author's CNP to filter by.</param>
        /// <param name="status">The status to filter by (or null for all).</param>
        /// <param name="topic">The topic to filter by (or null for all).</param>
        /// <returns>A list of <see cref="NewsArticle"/> instances.</returns>
        /// <exception cref="UnauthorizedAccessException">If current user is not an admin.</exception>
        /// <exception cref="NewsPersistenceException">If loading fails.</exception>
        public async Task<List<NewsArticle>> GetUserArticlesAsync(string authorCNP, Status status = Status.All, string topic = "All")
        {
            if (IUserRepository.IsGuest)
            {
                throw new UnauthorizedAccessException("User must be logged in to access user articles");
            }

            User user = await this.userRepository.GetByCnpAsync(authorCNP) ?? throw new Exception("User not found");
            if (!user.IsModerator)
            {
                throw new UnauthorizedAccessException("User must be an admin to access user articles");
            }

            try
            {
                List<NewsArticle> userArticles = await this.newsRepository.GetNewsArticlesByAuthorCNPAsync(authorCNP);

                // Inline: apply status filter
                if (status != Status.All)
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
        public async Task<bool> ApproveUserArticleAsync(string articleId)
        {
            if (IUserRepository.IsGuest)
            {
                throw new UnauthorizedAccessException("User must be logged in to access user articles");
            }

            User user = await this.userRepository.GetByCnpAsync(IUserRepository.CurrentUserCNP) ?? throw new Exception("User not found");
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
                NewsArticle newsArticle = await this.newsRepository.GetNewsArticleByIdAsync(articleId);
                newsArticle.Status = Status.Approved;
                await this.newsRepository.UpdateNewsArticleAsync(newsArticle);
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
        public async Task<bool> RejectUserArticleAsync(string articleId)
        {
            if (IUserRepository.IsGuest)
            {
                throw new UnauthorizedAccessException("User must be logged in to access user articles");
            }

            User user = await this.userRepository.GetByCnpAsync(IUserRepository.CurrentUserCNP) ?? throw new Exception("User not found");
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
                NewsArticle newsArticle = await this.newsRepository.GetNewsArticleByIdAsync(articleId);
                newsArticle.Status = Status.Rejected;
                await this.newsRepository.UpdateNewsArticleAsync(newsArticle);
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
        public async Task<bool> DeleteUserArticleAsync(string articleId)
        {
            if (IUserRepository.IsGuest)
            {
                throw new UnauthorizedAccessException("User must be logged in to access user articles");
            }

            User user = await this.userRepository.GetByCnpAsync(IUserRepository.CurrentUserCNP) ?? throw new Exception("User not found");
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
                await this.newsRepository.DeleteNewsArticleAsync(articleId);
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
        public async Task<bool> SubmitUserArticleAsync(NewsArticle article)
        {
            if (IUserRepository.IsGuest)
            {
                throw new UnauthorizedAccessException("User must be logged in to access user articles");
            }

            User user = await this.userRepository.GetByCnpAsync(IUserRepository.CurrentUserCNP) ?? throw new Exception("User not found");
            if (user.IsModerator)
            {
                throw new UnauthorizedAccessException("User must be an admin to access user articles");
            }

            // Inline: set author and metadata
            article.Author = user;
            article.PublishedDate = DateTime.Now;
            article.Status = Status.Pending;

            try
            {
                await this.newsRepository.AddNewsArticleAsync(article);
                return true;
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException("Failed to submit user article.", ex);
            }
        }

        /// <summary>
        /// Gets related stock symbols for the specified article.
        /// </summary>
        /// <param name="articleId">The ID of the article.</param>
        /// <returns>A list of related stock symbols.</returns>
        /// <exception cref="NewsPersistenceException">If retrieval fails.</exception>
        public async Task<List<Stock>> GetRelatedStocksForArticleAsync(string articleId)
        {
            try
            {
                NewsArticle article = await this.newsRepository.GetNewsArticleByIdAsync(articleId);
                return article.RelatedStocks;
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException(
                    $"Failed to retrieve related stocks for article '{articleId}'.", ex);
            }
        }
    }
}
