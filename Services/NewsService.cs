namespace StockApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using StockApp.Exceptions;
    using StockApp.Models;
    using StockApp.Repositories;

    public class NewsService : INewsService
    {
        private readonly AppState _appState;
        private static readonly Dictionary<string, NewsArticle> _previewArticles = new();
        private static readonly Dictionary<string, UserArticle> _previewUserArticles = new();
        private readonly List<NewsArticle> _cachedArticles = new();
        private static readonly List<UserArticle> _userArticles = new();
        private static bool _isInitialized = false;
        private INewsRepository _repository = new NewsRepository();
        private IBaseStocksRepository _stocksRepository;

        public NewsService()
        {
            _appState = AppState.Instance;
            _stocksRepository = new BaseStocksRepository();

            if (!_isInitialized)
            {
                _userArticles.AddRange(_repository.GetAllUserArticles().Cast<UserArticle>());
                _isInitialized = true;
            }
        }

        public async Task<List<NewsArticle>> GetNewsArticlesAsync()
        {
            await Task.Delay(200);

            try
            {
                return await Task.Run(() => _repository.GetAllNewsArticles());
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException("Failed to retrieve news articles.", ex);
            }
        }

        public async Task<NewsArticle> GetNewsArticleByIdAsync(string articleId)
        {
            if (string.IsNullOrWhiteSpace(articleId))
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            if (_previewArticles.TryGetValue(articleId, out var previewArticle))
            {
                return previewArticle;
            }

            await Task.Delay(200);

            try
            {
                var article = await Task.Run(() => _repository.GetNewsArticleById(articleId));
                return article ?? throw new KeyNotFoundException($"Article with ID {articleId} not found");
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException($"Failed to load article with ID {articleId}.", ex);
            }
        }

        public async Task<bool> MarkArticleAsReadAsync(string articleId)
        {
            if (string.IsNullOrWhiteSpace(articleId))
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            await Task.Delay(100);

            try
            {
                await Task.Run(() => _repository.MarkArticleAsRead(articleId));
                return true;
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException($"Failed to mark article {articleId} as read.", ex);
            }
        }

        public async Task<bool> CreateArticleAsync(NewsArticle article)
        {
            if (_appState.CurrentUser == null)
            {
                throw new UnauthorizedAccessException("User must be logged in to create an article");
            }

            await Task.Delay(300);

            try
            {
                await Task.Run(() => _repository.AddNewsArticle(article));
                return true;
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException("Failed to create news article.", ex);
            }
        }

        public async Task<List<UserArticle>> GetUserArticlesAsync(string status = null, string topic = null)
        {
            if (_appState.CurrentUser == null || !_appState.CurrentUser.IsModerator)
            {
                throw new UnauthorizedAccessException("User must be an admin to access user articles");
            }

            await Task.Delay(300);

            try
            {
                var userArticles = (await Task.Run(() => _repository.GetAllUserArticles())).Cast<UserArticle>().ToList();

                if (!string.IsNullOrEmpty(status) && status != "All")
                {
                    userArticles = userArticles.Where(a => a.Status == status).ToList();
                }

                if (!string.IsNullOrEmpty(topic) && topic != "All")
                {
                    userArticles = userArticles.Where(a => a.Topic == topic).ToList();
                }

                return userArticles;
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException("Failed to load user articles.", ex);
            }
        }

        public async Task<bool> ApproveUserArticleAsync(string articleId)
        {
            if (_appState.CurrentUser == null || !_appState.CurrentUser.IsModerator)
            {
                throw new UnauthorizedAccessException("User must be an admin to approve articles");
            }

            if (string.IsNullOrWhiteSpace(articleId))
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            await Task.Delay(300);

            try
            {
                await Task.Run(() => _repository.ApproveUserArticle(articleId));
                _cachedArticles.Clear();
                return true;
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException($"Failed to approve article {articleId}.", ex);
            }
        }

        public async Task<bool> RejectUserArticleAsync(string articleId)
        {
            if (_appState.CurrentUser == null || !_appState.CurrentUser.IsModerator)
            {
                throw new UnauthorizedAccessException("User must be an admin to reject articles");
            }

            if (string.IsNullOrWhiteSpace(articleId))
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            await Task.Delay(300);

            try
            {
                await Task.Run(() => _repository.RejectUserArticle(articleId));
                _cachedArticles.Clear();
                return true;
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException($"Failed to reject article {articleId}.", ex);
            }
        }

        public async Task<bool> DeleteUserArticleAsync(string articleId)
        {
            if (_appState.CurrentUser == null || !_appState.CurrentUser.IsModerator)
            {
                throw new UnauthorizedAccessException("User must be an admin to delete articles");
            }

            if (string.IsNullOrWhiteSpace(articleId))
            {
                throw new ArgumentNullException(nameof(articleId));
            }

            await Task.Delay(300);

            try
            {
                await Task.Run(() => _repository.DeleteUserArticle(articleId));
                await Task.Run(() => _repository.DeleteNewsArticle(articleId));
                _cachedArticles.Clear();
                return true;
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException($"Failed to delete article {articleId}.", ex);
            }
        }

        public async Task<bool> SubmitUserArticleAsync(UserArticle article)
        {
            if (_appState.CurrentUser == null)
            {
                throw new UnauthorizedAccessException("User must be logged in to submit an article");
            }

            article.Author = _appState.CurrentUser.CNP;
            article.SubmissionDate = DateTime.Now;
            article.Status = "Pending";

            await Task.Delay(300);

            try
            {
                await Task.Run(() => _repository.AddUserArticle(article));
                _cachedArticles.Clear();
                return true;
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException("Failed to submit user article.", ex);
            }
        }

        public async Task<User> GetCurrentUserAsync()
        {
            if (_appState.CurrentUser != null)
            {
                return _appState.CurrentUser;
            }

            await Task.Delay(200);
            throw new InvalidOperationException("No user is currently logged in");
        }

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

            await Task.Delay(300);

            if (username == "admin" && password == "admin")
            {
                string adminCnp = "6666666666666";

                try
                {
                    _repository.EnsureUserExists(adminCnp, "admin", "Administrator Account", true, false, "img.jpg");
                }
                catch (NewsPersistenceException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error ensuring admin user exists: {ex.Message}");
                }

                return new User(adminCnp, "admin", "Administrator Account", true, "img.jpg", false);
            }
            else if (username == "user" && password == "user")
            {
                return new User("1234567890123", "Caramel", "asdf", false, "imagine", false);
            }

            throw new UnauthorizedAccessException("Invalid username or password");
        }

        public void Logout()
        {
            _appState.CurrentUser = null;
            _previewArticles.Clear();
            _previewUserArticles.Clear();
        }

        public void StorePreviewArticle(NewsArticle article, UserArticle userArticle)
        {
            string articleId = article.ArticleId.StartsWith("preview:") ? article.ArticleId[8..] : article.ArticleId;
            article.ArticleId = articleId;

            article.RelatedStocks = userArticle.RelatedStocks != null
                ? new List<string>(userArticle.RelatedStocks)
                : new List<string>();

            _previewArticles[articleId] = article;
            _previewUserArticles[articleId] = userArticle;

            if (article.RelatedStocks?.Count > 0)
            {
                try
                {
                    _repository.AddRelatedStocksForArticle(articleId, article.RelatedStocks, null, null);
                }
                catch (NewsPersistenceException ex)
                {
                    throw new NewsPersistenceException(
                        $"Failed to persist related stocks for preview article '{articleId}'.", ex);
                }
            }
        }


        public UserArticle GetUserArticleForPreview(string articleId)
        {
            if (_previewUserArticles.TryGetValue(articleId, out var previewArticle))
            {
                return previewArticle;
            }

            return _userArticles.FirstOrDefault(a => a.ArticleId == articleId);
        }

        public List<string> GetRelatedStocksForArticle(string articleId)
        {
            string actualId = articleId.StartsWith("preview:") ? articleId[8..] : articleId;

            if (_previewUserArticles.TryGetValue(actualId, out var previewUserArticle) &&
                previewUserArticle.RelatedStocks?.Any() == true)
            {
                return previewUserArticle.RelatedStocks;
            }

            try
            {
                return _repository.GetRelatedStocksForArticle(actualId);
            }
            catch (NewsPersistenceException ex)
            {
                throw new NewsPersistenceException(
                    $"Failed to retrieve related stocks for article '{actualId}'.", ex);
            }
        }


        public void UpdateCachedArticles(List<NewsArticle> articles)
        {
            _cachedArticles.Clear();
            if (articles != null)
            {
                _cachedArticles.AddRange(articles);
            }
        }

        public List<NewsArticle> GetCachedArticles()
        {
            return _cachedArticles.Count > 0 ? _cachedArticles : _repository.GetAllNewsArticles();
        }
    }
}
