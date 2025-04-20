using System;
using System.Collections.Generic;
using System.Linq;
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
    public class NewsListViewModelTests
    {
        private Mock<INewsService> _svcMock;
        private Mock<IDispatcher> _dispMock;
        private Mock<IAppState> _appStateMock;
        private NewsListViewModel _vm;

        private readonly List<NewsArticle> _sampleArticles =
        [
           new NewsArticle(
               articleId: "1",
               title: "A",
               summary: "",
               content: "",
               source: "Unknown",
               publishedDate: new DateTime(2025, 1, 1),
               relatedStocks: new List<string>(),
               status: Status.Pending
           )
           {
               Category = "Stock News",
               IsWatchlistRelated = false
           },
           new NewsArticle(
               articleId: "2",
               title: "B",
               summary: "",
               content: "",
               source: "Unknown",
               publishedDate: new DateTime(2025, 2, 1),
               relatedStocks: new List<string>(),
               status: Status.Pending
           )
           {
               Category = "Company News",
               IsWatchlistRelated = true
           }
        ];

        [TestInitialize]
        public void Setup()
        {
            _svcMock = new Mock<INewsService>(MockBehavior.Strict);
            _dispMock = new Mock<IDispatcher>(MockBehavior.Strict);
            _appStateMock = new Mock<IAppState>(MockBehavior.Strict);

            _dispMock
                .Setup(d => d.TryEnqueue(It.IsAny<DispatcherQueueHandler>()))
                .Callback<DispatcherQueueHandler>(cb => cb())
                .Returns(true);

            _svcMock
                .Setup(s => s.GetCurrentUserAsync())
                .ReturnsAsync(new User { Username = "u", IsModerator = false });

            _svcMock
                .Setup(s => s.GetNewsArticlesAsync())
                .ReturnsAsync(_sampleArticles);

            _svcMock
                .Setup(s => s.UpdateCachedArticles(_sampleArticles));
            _svcMock
                .Setup(s => s.GetCachedArticles())
                .Returns(_sampleArticles);

            _appStateMock.Setup(a => a.CurrentUser).Returns((User)null);

            _vm = new NewsListViewModel(
                _svcMock.Object,
                _dispMock.Object,
                _appStateMock.Object
            );
        }

        [TestMethod]
        public void Constructor_InitializesCategoriesAndDefault()
        {
            var expected = new[]
            {
                "All",
                "Stock News",
                "Company News",
                "Market Analysis",
                "Economic News",
                "Functionality News"
            };
            CollectionAssert.AreEqual(expected, _vm.Categories.ToList());
            Assert.AreEqual("All", _vm.SelectedCategory);
        }

        private Task InvokeRefreshAsync() =>
            (Task)typeof(NewsListViewModel)
                .GetMethod("RefreshArticlesAsync", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(_vm, null)!;

        [TestMethod]
        public async Task RefreshArticlesAsync_PopulatesArticlesAndUpdatesCache()
        {
            await InvokeRefreshAsync();

            _svcMock.Verify(s => s.UpdateCachedArticles(_sampleArticles), Times.Once);
            Assert.AreEqual(2, _vm.Articles.Count);
            Assert.AreEqual("2", _vm.Articles[0].ArticleId);
            Assert.AreEqual("1", _vm.Articles[1].ArticleId);
            Assert.IsFalse(_vm.IsEmptyState);
        }

        private void InvokeFilter() =>
            typeof(NewsListViewModel)
                .GetMethod("FilterArticles", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(_vm, null);

        [TestMethod]
        public void FilterArticles_BySearchQuery_Filters()
        {
            _svcMock.Setup(s => s.GetCachedArticles()).Returns(_sampleArticles);
            _vm.SearchQuery = "A";

            Assert.AreEqual(1, _vm.Articles.Count);
            Assert.AreEqual("1", _vm.Articles[0].ArticleId);
        }

        [TestMethod]
        public void FilterArticles_ByCategory_Filters()
        {
            _svcMock.Setup(s => s.GetCachedArticles()).Returns(_sampleArticles);
            _vm.SelectedCategory = "Company News";

            Assert.AreEqual(1, _vm.Articles.Count);
            Assert.AreEqual("2", _vm.Articles[0].ArticleId);
        }

        [TestMethod]
        public void IsAdminAndIsLoggedIn_DerivedFromCurrentUser()
        {
            _vm.CurrentUser = null;
            Assert.IsFalse(_vm.IsLoggedIn);
            Assert.IsFalse(_vm.IsAdmin);

            var mod = new User { IsModerator = true };
            _vm.CurrentUser = mod;
            Assert.IsTrue(_vm.IsLoggedIn);
            Assert.IsTrue(_vm.IsAdmin);
        }
    }
}