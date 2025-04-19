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
        private Mock<INewsRepository> _mockRepo;
        private Mock<IBaseStocksRepository> _mockStocks;
        private NewsService _service;

        [TestInitialize]
        public void Init()
        {
            _mockRepo = new Mock<INewsRepository>();
            _mockStocks = new Mock<IBaseStocksRepository>();
            _service = new NewsService(_mockRepo.Object, _mockStocks.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GetNewsArticleByIdAsync_Throws_WhenNullId()
        {
            await _service.GetNewsArticleByIdAsync(null);
        }

        [TestMethod]
        public async Task GetNewsArticleByIdAsync_ReturnsArticle_WhenFound()
        {
            // Arrange
            var article = new NewsArticle { ArticleId = "a1", Title = "Test", Content = "..." };
            _mockRepo.Setup(r => r.GetNewsArticleById("a1")).Returns(article);

            // Act
            var result = await _service.GetNewsArticleByIdAsync("a1");

            // Assert
            Assert.AreEqual("Test", result.Title);
        }

        [TestMethod]
        public async Task MarkArticleAsReadAsync_CallsRepository()
        {
            // Arrange
            string articleId = "a2";
            _mockRepo.Setup(r => r.MarkArticleAsRead(articleId));

            // Act
            var result = await _service.MarkArticleAsReadAsync(articleId);

            // Assert
            Assert.IsTrue(result);
            _mockRepo.Verify(r => r.MarkArticleAsRead(articleId), Times.Once);
        }

        [TestMethod]
        public async Task GetNewsArticlesAsync_ReturnsArticles()
        {
            // Arrange
            var articles = new List<NewsArticle>
            {
                new NewsArticle { ArticleId = "1", Title = "First" },
                new NewsArticle { ArticleId = "2", Title = "Second" }
            };
            _mockRepo.Setup(r => r.GetAllNewsArticles()).Returns(articles);

            // Act
            var result = await _service.GetNewsArticlesAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("First", result[0].Title);
        }
    }
}
