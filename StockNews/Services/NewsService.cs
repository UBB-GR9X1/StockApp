﻿using StockNewsPage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System.Profile;
using StockApp.Repositories;
using StocksApp.Services;
using StockApp.Model;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace StockNewsPage.Services
{
    public class NewsService
    {
        private readonly AppState _appState;
        private static readonly Dictionary<string, NewsArticle> _previewArticles = new();
        private static readonly Dictionary<string, UserArticle> _previewUserArticles = new();
        private readonly List<NewsArticle> _cachedArticles = new();
        private static readonly List<UserArticle> _userArticles = new();
        private static bool _isInitialized = false;
        private NewsRepository _repository = new NewsRepository();

        public NewsService()
        {
            _appState = AppState.Instance;
            if (!_isInitialized)
            {
                _userArticles.AddRange(_repository.GetAllUserArticles());
                _isInitialized = true;
            }
        }

        // Article Methods
        public async Task<List<NewsArticle>> GetNewsArticlesAsync()
        {
            await Task.Delay(200);

            try
            {
                return await Task.Run(() => _repository.GetAllNewsArticles());
            }
            catch
            {
                return _repository.GetAllNewsArticles();
            }
        }

        public async Task<NewsArticle> GetNewsArticleByIdAsync(string articleId)
        {
            // check if this is a preview article
            bool isPreview = articleId.StartsWith("preview:");
            string actualId = isPreview ? articleId.Substring(8) : articleId;

            // check preview dictionary regardless of prefix
            if (_previewArticles.TryGetValue(actualId, out var previewArticle))
            {
                return previewArticle;
            }

            // if marked as preview but not found, don't continue to database
            if (isPreview)
            {
                return null;
            }

            await Task.Delay(200);

            try
            {
                return await Task.Run(() => _repository.GetNewsArticleById(actualId));
            }
            catch // fallback
            {
                var cachedArticle = _cachedArticles.FirstOrDefault(a => a.ArticleId == actualId);
                if (cachedArticle != null)
                {
                    return cachedArticle;
                }

                var mockArticles = _repository.GetAllNewsArticles();
                return mockArticles.FirstOrDefault(a => a.ArticleId == actualId);
            }
        }



        public async Task<bool> MarkArticleAsReadAsync(string articleId)
        {
            await Task.Delay(100);

            try
            {
                await Task.Run(() => _repository.MarkArticleAsRead(articleId));
                return true;
            }
            catch
            {
                // mark as read in cached articles
                var article = _cachedArticles.FirstOrDefault(a => a.ArticleId == articleId);
                if (article != null)
                {
                    article.IsRead = true;
                }
                // rn, return success
                return true;
            }
        }

        public async Task<bool> CreateArticleAsync(NewsArticle article)
        {
            // ensure user is logged in
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
            catch
            {
                _cachedArticles.Add(article);
                return true;
            }
        }

        // User Article Methods
        public async Task<List<UserArticle>> GetUserArticlesAsync(string status = null, string topic = null)
        {
            // ensure the user is admin
            if (_appState.CurrentUser == null || !_appState.CurrentUser.IsModerator)
            {
                throw new UnauthorizedAccessException("User must be an admin to access user articles");
            }

            await Task.Delay(300);
            var userArticles = new List<UserArticle>();
            try
            {
                userArticles = await Task.Run(() => _repository.GetAllUserArticles());

            }
            catch
            {
                userArticles = new List<UserArticle>(_userArticles);
            }

            // filters
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

        public async Task<bool> ApproveUserArticleAsync(string articleId)
        {
            // ensure the user is admin
            if (_appState.CurrentUser == null || !_appState.CurrentUser.IsModerator)
            {
                throw new UnauthorizedAccessException("User must be an admin to approve articles");
            }

            await Task.Delay(300);

            try
            {
                await Task.Run(() => _repository.ApproveUserArticle(articleId));
                _cachedArticles.Clear();
                return true;
            }
            catch
            {
                var article = _userArticles.FirstOrDefault(a => a.ArticleId == articleId);
                if (article != null)
                {
                    article.Status = "Approved";
                    _cachedArticles.Clear();
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> RejectUserArticleAsync(string articleId)
        {
            // ensure the user is admin
            if (_appState.CurrentUser == null || !_appState.CurrentUser.IsModerator)
            {
                throw new UnauthorizedAccessException("User must be an admin to reject articles");
            }

            await Task.Delay(300);

            try
            {
                await Task.Run(() => _repository.RejectUserArticle(articleId));
                _cachedArticles.Clear();
                return true;
            }
            catch
            {
                var article = _userArticles.FirstOrDefault(a => a.ArticleId == articleId);
                if (article != null)
                {
                    article.Status = "Rejected";
                    _cachedArticles.Clear();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> DeleteUserArticleAsync(string articleId)
        {
            // ensure the user is admin
            if (_appState.CurrentUser == null || !_appState.CurrentUser.IsModerator)
            {
                throw new UnauthorizedAccessException("User must be an admin to delete articles");
            }

            await Task.Delay(300);

            try
            {
                await Task.Run(() => _repository.DeleteUserArticle(articleId));
                _cachedArticles.Clear();
                return true;
            }
            catch
            {
                var article = _userArticles.FirstOrDefault(a => a.ArticleId == articleId);
                if (article != null)
                {
                    _userArticles.Remove(article);
                    _cachedArticles.Clear();
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> SubmitUserArticleAsync(UserArticle article)
        {
            // ensure user is logged in
            if (_appState.CurrentUser == null)
            {
                throw new UnauthorizedAccessException("User must be logged in to submit an article");
            }

            // set author and submission date
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
            catch
            {
                // rn, return success and add to local data
                _userArticles.Add(article);
                _cachedArticles.Clear();
                return true;
            }

        }

        // User Authentication Methods
        public async Task<StockApp.Model.User> GetCurrentUserAsync()
        {
            // checks if user is already in app state
            if (_appState.CurrentUser != null)
            {
                return _appState.CurrentUser;
            }

            await Task.Delay(200);
            // this supposed to be DIFFERENT BUT AINT NO WAY IT COULD BE CHANGED WITH THE CURRENT CODEBASE

            return null;
        }

        public async Task<StockApp.Model.User> LoginAsync(string username, string password)
        {
            await Task.Delay(300);

            if (username == "admin" && password == "admin")
            {
                string adminCnp = "6666666666666";

                try
                {
                    _repository.EnsureUserExists(
                        adminCnp,
                        "admin",
                        "Administrator Account",
                        true, // isAdmin
                        false, // isHidden
                        "img.jpg"
                    );
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error ensuring admin user exists: {ex.Message}");
                }

                return new StockApp.Model.User(
                    adminCnp,
                    "admin",
                    "Administrator Account",
                    true,
                    "img.jpg", false);
            }
            else if (username == "user" && password == "user")
            {
                return new StockApp.Model.User(
                    "1234567890123",
                    "Caramel",
                    "asdf",
                    false,
                    "imagine", false);
            }

            return null;
        }

        public void Logout()
        {
            // this supposed to be DIFFERENT BUT AINT NO WAY IT COULD BE CHANGED WITH THE CURRENT CODEBASE
            _appState.CurrentUser = null;
            // clear preview articles
            _previewArticles.Clear();
            _previewUserArticles.Clear();
        }

        // Preview Methods
        public void StorePreviewArticle(NewsArticle article, UserArticle userArticle)
        {
            _previewArticles[userArticle.ArticleId] = article;
            _previewUserArticles[userArticle.ArticleId] = userArticle;
        }

        public UserArticle GetUserArticleForPreview(string articleId)
        {
            if (_previewUserArticles.TryGetValue(articleId, out var previewArticle))
            {
                return previewArticle;
            }

            // if not in preview cache, check the regular user articles
            return _userArticles.FirstOrDefault(a => a.ArticleId == articleId);
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

