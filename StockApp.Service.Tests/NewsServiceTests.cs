using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StockApp.Models;
using StockApp.Repositories;
using StockApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Service.Tests
{
    [TestClass]
    public class NewsServiceTests
    {
        private Mock<NewsRepository> _mockRepo;
        private Mock<IBaseStocksRepository> _mockStocks;
        private NewsService _service;

        [TestInitialize]
        public void Init()
        {
            _mockRepo = new Mock<NewsRepository>(MockBehavior.Strict);
            _mockRepo.As<INewsRepository>(); // Ensure the mock implements INewsRepository  

            // Setup for GetAllUserArticles to avoid strict mock exception  
            _mockRepo.As<INewsRepository>().Setup(r => r.GetAllUserArticles()).Returns(new List<UserArticle>());

            _mockStocks = new Mock<IBaseStocksRepository>(MockBehavior.Strict);
            _service = new NewsService(_mockRepo.Object, _mockStocks.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GetNewsArticleByIdAsync_Throws_WhenNullId()
        {
            _service.GetNewsArticleById(null);
        }

        [TestMethod]
        public async Task GetNewsArticleByIdAsync_ReturnsArticle_WhenFound()
        {
            // Arrange  
            var article = new NewsArticle(articleId: "a1", title: "Test", summary: "Summary", content: "Content", source: "Source", publishedDate: DateTime.Now, relatedStocks: new List<string>(), status: Status.Pending);
            _mockRepo.As<INewsRepository>().Setup(r => r.GetNewsArticleById(It.IsAny<string>())).Returns(article);

            // Act  
            var result = _service.GetNewsArticleById("a1");

            // Assert  
            Assert.AreEqual(article.Title, result.Title);
            Assert.AreEqual(article.ArticleId, result.ArticleId);
            Assert.AreEqual(article.Content, result.Content);
        }

        [TestMethod]
        public async Task MarkArticleAsReadAsync_CallsRepository()
        {
            // Arrange
            string articleId = "a2";
            _mockRepo.As<INewsRepository>().Setup(r => r.MarkArticleAsRead(articleId));

            // Act
            var result = _service.MarkArticleAsRead(articleId);

            // Assert
            Assert.IsTrue(result);
            _mockRepo.As<INewsRepository>().Verify(r => r.MarkArticleAsRead(articleId), Times.Once);
        }

        [TestMethod]
        public async Task GetNewsArticlesAsync_ReturnsArticles()
        {
            // Arrange
            var articles = new List<NewsArticle>
           {
               new NewsArticle(articleId: "1", title: "First", summary: "Summary", content: "Content", source: "Source", publishedDate: DateTime.Now, relatedStocks: new List<string>(), status: Status.Pending),
               new NewsArticle(articleId: "2", title: "Second", summary: "Summary", content: "Content", source: "Source", publishedDate: DateTime.Now, relatedStocks: new List<string>(), status: Status.Pending)
           };

            _mockRepo.As<INewsRepository>().Setup(r => r.GetAllNewsArticles()).Returns(articles);

            // Act
            var result = _service.GetNewsArticles();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("First", result[0].Title);
            Assert.AreEqual("Second", result[1].Title);
        }
    }
}
