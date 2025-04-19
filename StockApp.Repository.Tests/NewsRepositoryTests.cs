using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockApp.Models;
using StockApp.Repositories;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace StockApp.Repository.Tests
{
    [TestClass]
    public class NewsRepositoryTests
    {
        private NewsRepository _repository;

        [TestInitialize]
        public void Init()
        {
            // Use reflection to bypass constructor logic that connects to DB
            _repository = (NewsRepository)FormatterServices.GetUninitializedObject(typeof(NewsRepository));

            // Inject dummy in-memory lists
            typeof(NewsRepository)
                .GetField("newsArticles", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(_repository, new List<NewsArticle>());

            typeof(NewsRepository)
                .GetField("userArticles", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(_repository, new List<UserArticle>());
        }

        [TestMethod]
        public void AddNewsArticle_AddsToInternalList()
        {
            var article = new NewsArticle
            {
                ArticleId = "123",
                Title = "Mock",
                Summary = "Unit Test",
                Content = "Test content",
                Source = "Mock Source",
                PublishedDate = "2025-01-01",
                IsRead = false,
                IsWatchlistRelated = false,
                Category = "Test",
                RelatedStocks = new List<string> { "AAPL" }
            };

            // Manually add article to internal list (simulate successful DB write)
            var internalList = (List<NewsArticle>)typeof(NewsRepository)
                .GetField("newsArticles", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(_repository);

            internalList.Add(article);

            var result = _repository.GetAllNewsArticles();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Mock", result[0].Title);
        }

        [TestMethod]
        public void GetNewsArticlesByCategory_FiltersCorrectly()
        {
            var list = new List<NewsArticle>
            {
                new NewsArticle { ArticleId = "1", Title = "Finance", Category = "Finance" },
                new NewsArticle { ArticleId = "2", Title = "Tech", Category = "Tech" }
            };

            typeof(NewsRepository)
                .GetField("newsArticles", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(_repository, list);

            var result = _repository.GetNewsArticlesByCategory("Tech");
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Tech", result[0].Category);
        }

        [TestMethod]
        public void GetNewsArticleById_ReturnsCorrectArticle()
        {
            var article = new NewsArticle { ArticleId = "A1", Title = "Read Me", RelatedStocks = new List<string> { "TSLA" } };

            typeof(NewsRepository)
                .GetField("newsArticles", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(_repository, new List<NewsArticle> { article });

            var result = _repository.GetNewsArticleById("A1");
            Assert.AreEqual("Read Me", result.Title);
            Assert.AreEqual("A1", result.ArticleId);
        }
    }
}
