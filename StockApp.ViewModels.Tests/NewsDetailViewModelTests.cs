using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StockApp.Models;
using StockApp.Services;
using StockApp.ViewModels;

namespace StockApp.ViewModels.Tests
{
    [TestClass]
    public class NewsDetailViewModelTests
    {
        private Mock<INewsService> _svcMock;
        private Mock<IDispatcher> _dispMock;
        private NewsDetailViewModel _vm;

        [TestInitialize]
        public void Setup()
        {
            _svcMock = new Mock<INewsService>(MockBehavior.Strict);
            _dispMock = new Mock<IDispatcher>(MockBehavior.Strict);

            _dispMock
                .Setup(d => d.TryEnqueue(It.IsAny<DispatcherQueueHandler>()))
                .Callback<DispatcherQueueHandler>(cb => cb())
                .Returns(true);

            _vm = new NewsDetailViewModel(_svcMock.Object, _dispMock.Object);
        }

        [TestMethod]
        public void LoadArticle_PreviewMode_UserFound_SetsState()
        {
            var userArt = new UserArticle(
                articleId: "123",
                title: "User Article Title",
                summary: "User Article Summary",
                content: "User Article Content",
                author: new User(username: "testUser"),
                submissionDate: DateTime.Now,
                status: "Pending",
                topic: "Finance",
                relatedStocks: new List<string> { "AAPL" }
            );

            var newsArt = new NewsArticle(
                articleId: "preview:123",
                title: "News Article Title",
                summary: "News Article Summary",
                content: "News Article Content",
                source: "News Source",
                publishedDate: DateTime.Now,
                relatedStocks: new List<string> { "AAPL" },
                status: Status.Pending
            );

            _svcMock.Setup(s => s.GetUserArticleForPreview("123")).Returns(userArt);
            _svcMock.Setup(s => s.GetNewsArticleById("preview:123"))
                    .Returns(newsArt);

            _vm.LoadArticle("preview:123");

            Assert.IsTrue(_vm.IsAdminPreview);
            Assert.AreEqual("Pending", _vm.ArticleStatus);
            Assert.IsTrue(_vm.CanApprove);
            Assert.IsTrue(_vm.CanReject);
            Assert.AreSame(newsArt, _vm.Article);
            Assert.IsTrue(_vm.HasRelatedStocks);
        }

        [TestMethod]
        public void LoadArticle_PreviewMode_NotFound_ShowsNotFound()
        {
            _svcMock.Setup(s => s.GetUserArticleForPreview("id")).Returns((UserArticle?)null);
            _svcMock.Setup(s => s.GetNewsArticleById("preview:id"))
                    .Returns((NewsArticle?)null);

            _vm.LoadArticle("preview:id");

            Assert.AreEqual("Article Not Found", _vm.Article.Title);
            Assert.IsFalse(_vm.HasRelatedStocks);
        }

        [TestMethod]
        public void LoadArticle_RegularMode_Found_MarksRead()
        {
            var newsArt = new NewsArticle(
                "42",
                "Article Title",
                "Article Summary",
                "Article Content",
                "News Source",
                DateTime.Now,
                new List<string> { "AAPL" },
                Status.Pending
                );

            _svcMock.Setup(s => s.GetNewsArticleById("42"))
                    .Returns(newsArt);
            _svcMock.Setup(s => s.MarkArticleAsRead("42"))
                    .Returns(true);

            _vm.LoadArticle("42");

            Assert.IsFalse(_vm.IsAdminPreview);
            Assert.AreSame(newsArt, _vm.Article);
            _svcMock.Verify(s => s.MarkArticleAsRead("42"), Times.Once);
        }

        [TestMethod]
        public void LoadArticle_RegularMode_NotFound_ShowsNotFound()
        {
            _svcMock.Setup(s => s.GetNewsArticleById("x"))
                    .Returns((NewsArticle?)null);

            _vm.LoadArticle("x");

            Assert.AreEqual("Article Not Found", _vm.Article.Title);
        }
    }
}