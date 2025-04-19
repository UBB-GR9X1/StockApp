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
            var userArt = new UserArticle { Status = "Pending", RelatedStocks = ["AAPL"] };
            var newsArt = new NewsArticle { RelatedStocks = ["AAPL"] };

            _svcMock.Setup(s => s.GetUserArticleForPreview("123")).Returns(userArt);
            _svcMock.Setup(s => s.GetNewsArticleByIdAsync("preview:123"))
                    .ReturnsAsync(newsArt);

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
            _svcMock.Setup(s => s.GetNewsArticleByIdAsync("preview:id"))
                    .ReturnsAsync((NewsArticle?)null);

            _vm.LoadArticle("preview:id");

            Assert.AreEqual("Article Not Found", _vm.Article.Title);
            Assert.IsFalse(_vm.HasRelatedStocks);
        }

        [TestMethod]
        public void LoadArticle_RegularMode_Found_MarksRead()
        {
            var newsArt = new NewsArticle { RelatedStocks = [] };
            _svcMock.Setup(s => s.GetNewsArticleByIdAsync("42"))
                    .ReturnsAsync(newsArt);
            _svcMock.Setup(s => s.MarkArticleAsReadAsync("42"))
                    .ReturnsAsync(true);

            _vm.LoadArticle("42");

            Assert.IsFalse(_vm.IsAdminPreview);
            Assert.AreSame(newsArt, _vm.Article);
            _svcMock.Verify(s => s.MarkArticleAsReadAsync("42"), Times.Once);
        }

        [TestMethod]
        public void LoadArticle_RegularMode_NotFound_ShowsNotFound()
        {
            _svcMock.Setup(s => s.GetNewsArticleByIdAsync("x"))
                    .ReturnsAsync((NewsArticle?)null);

            _vm.LoadArticle("x");

            Assert.AreEqual("Article Not Found", _vm.Article.Title);
        }
    }
}