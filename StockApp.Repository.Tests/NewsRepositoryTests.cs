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
            var article = new NewsArticle(
                articleId: "A1",
                title: "Mock",
                summary: "Summary",
                content: "Content",
                source: "Source",
                publishedDate: DateTime.Now,
                relatedStocks: new List<string>(),
                status: Status.Pending
            );

            // Use the repository's AddNewsArticle method to add the article  
            _repository.AddNewsArticle(article);

            // Retrieve the internal list to verify the addition  
            var internalList = (List<NewsArticle>)typeof(NewsRepository)
                .GetField("newsArticles", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(_repository);

            Assert.AreEqual(1, internalList.Count);
            Assert.AreEqual("Mock", internalList[0].Title);
        }

        [TestMethod]
        public void GetNewsArticlesByCategory_FiltersCorrectly()
        {
            var list = new List<NewsArticle>
           {
               new NewsArticle (
                   articleId: "1",
                   title: "Finance",
                   summary: "Summary",
                   content: "Content",
                   source: "Source",
                   publishedDate: DateTime.Now,
                   relatedStocks: new List<string>(),
                   status: Status.Pending
               ) { Category = "Finance" },
               new NewsArticle (
                   articleId: "2",
                   title: "Tech",
                   summary: "Summary",
                   content: "Content",
                   source: "Source",
                   publishedDate: DateTime.Now,
                   relatedStocks: new List<string>(),
                   status: Status.Pending
               ) { Category = "Tech" }
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
            var article = new NewsArticle(
                articleId: "A1",
                title: "Read Me",
                summary: "Summary",
                content: "Content",
                source: "Source",
                publishedDate: DateTime.Now,
                relatedStocks: new List<string>(),
                status: Status.Pending
            );

            typeof(NewsRepository)
                .GetField("newsArticles", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(_repository, new List<NewsArticle> { article });

            var result = _repository.GetNewsArticleById("A1");
            Assert.AreEqual("Read Me", result.Title);
            Assert.AreEqual("A1", result.ArticleId);
        }
    }
}
