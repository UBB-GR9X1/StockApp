using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StockApp.Database;
using StockApp.Models;
using StockApp.Repositories;
using StockApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApp.Service.Tests
{
    [TestClass]
    public class NewsServiceTests
    {
        private Mock<NewsRepository> _mockRepo;
        private Mock<AppDbContext> _mockDbContext;
        private Mock<DbSet<BaseStock>> _mockStockSet;
        private NewsService _service;

        [TestInitialize]
        public void Init()
        {
            _mockRepo = new Mock<NewsRepository>(MockBehavior.Strict);
            _mockRepo.As<INewsRepository>(); // Ensure the mock implements INewsRepository  

            // Setup for GetAllUserArticles to avoid strict mock exception  
            _mockRepo.As<INewsRepository>().Setup(r => r.GetAllUserArticles()).Returns(new List<UserArticle>());

            // Setup mock DbContext and DbSet
            var stockData = new List<BaseStock>();
            _mockStockSet = CreateMockDbSet(stockData);

            _mockDbContext = new Mock<AppDbContext>();
            _mockDbContext.Setup(m => m.BaseStocks).Returns(_mockStockSet.Object);

            // Mock INewsRepository implementation for NewsService
            var mockNewsRepo = new Mock<INewsRepository>();
            mockNewsRepo.Setup(r => r.GetAllUserArticles()).Returns(new List<UserArticle>());
            
            // Create service with DbContext
            _service = new NewsService(_mockDbContext.Object);

            // Set the newsRepository field using reflection (since it's created internally)
            var field = typeof(NewsService).GetField("newsRepository", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(_service, _mockRepo.Object);
        }

        private static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return mockSet;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNewsArticleById_Throws_WhenNullId()
        {
            _service.GetNewsArticleById(null);
        }

        [TestMethod]
        public void GetNewsArticleById_ReturnsArticle_WhenFound()
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
        public void MarkArticleAsRead_CallsRepository()
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
        public void GetNewsArticles_ReturnsArticles()
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
