using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StockApp.Database;
using StockApp.Models;
using StockApp.Repositories;
using StockApp.Services;
using StockApp.ViewModels;

namespace StockApp.ViewModels.Tests
{
    [TestClass]
    public class ArticleCreationViewModelTests
    {
        private Mock<INewsService> _newsServiceMock;
        private Mock<IDispatcher> _dispatcherMock;
        private Mock<AppDbContext> _dbContextMock;
        private ArticleCreationViewModel _vm;

        [TestInitialize]
        public void Setup()
        {
            _newsServiceMock = new Mock<INewsService>();
            _dispatcherMock = new Mock<IDispatcher>(MockBehavior.Strict);
            _dispatcherMock
                .Setup(d => d.TryEnqueue(It.IsAny<Microsoft.UI.Dispatching.DispatcherQueueHandler>()))
                .Callback<Microsoft.UI.Dispatching.DispatcherQueueHandler>(cb => { })
                .Returns(true);

            // Set up mock DbContext
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContextMock = new Mock<AppDbContext>(options);
            
            // Mock DbSet for BaseStocks
            var mockSet = new Mock<DbSet<BaseStock>>();
            var baseStocks = new List<BaseStock>();
            
            mockSet.As<IQueryable<BaseStock>>().Setup(m => m.Provider).Returns(baseStocks.AsQueryable().Provider);
            mockSet.As<IQueryable<BaseStock>>().Setup(m => m.Expression).Returns(baseStocks.AsQueryable().Expression);
            mockSet.As<IQueryable<BaseStock>>().Setup(m => m.ElementType).Returns(baseStocks.AsQueryable().ElementType);
            mockSet.As<IQueryable<BaseStock>>().Setup(m => m.GetEnumerator()).Returns(baseStocks.GetEnumerator());
            
            _dbContextMock.Setup(c => c.BaseStocks).Returns(mockSet.Object);

            _vm = new ArticleCreationViewModel(
                _newsServiceMock.Object,
                _dispatcherMock.Object,
                _dbContextMock.Object
            );
        }

        [TestMethod]
        public void Constructor_LoadsTopicsAndDefaultSelectedTopic()
        {
            var expected = new[]
            {
                "Stock News",
                "Company News",
                "Market Analysis",
                "Economic News",
                "Functionality News"
            };
            CollectionAssert.AreEqual(expected, _vm.Topics.ToList());
            Assert.AreEqual("Stock News", _vm.SelectedTopic);
        }

        [TestMethod]
        public void ClearCommand_ResetsAllFields()
        {
            _vm.Title = "X";
            _vm.Summary = "Y";
            _vm.Content = "Z";
            _vm.SelectedTopic = "Economic News";
            _vm.RelatedStocksText = "AAPL,MSFT";
            _vm.ErrorMessage = "Err";

            _vm.ClearCommand.Execute(null);

            Assert.AreEqual(string.Empty, _vm.Title);
            Assert.AreEqual(string.Empty, _vm.Summary);
            Assert.AreEqual(string.Empty, _vm.Content);
            Assert.AreEqual("Stock News", _vm.SelectedTopic);
            Assert.AreEqual(string.Empty, _vm.RelatedStocksText);
            Assert.AreEqual(string.Empty, _vm.ErrorMessage);
            Assert.IsFalse(_vm.HasError);
        }

        private Task InvokeSubmitAsync()
        {
            var m = typeof(ArticleCreationViewModel)
                .GetMethod("SubmitArticleAsync", BindingFlags.Instance | BindingFlags.NonPublic)
             ?? throw new InvalidOperationException("SubmitArticleAsync not found");
            return (Task)m.Invoke(_vm, null)!;
        }

        [TestMethod]
        public async Task SubmitArticleAsync_EmptyTitle_SetsError()
        {
            _vm.Title = "";
            _vm.Summary = "S";
            _vm.Content = "C";
            _vm.SelectedTopic = "Stock News";

            await InvokeSubmitAsync();

            Assert.AreEqual("Title is required.", _vm.ErrorMessage);
        }

        [TestMethod]
        public async Task SubmitArticleAsync_EmptySummary_SetsError()
        {
            _vm.Title = "T";
            _vm.Summary = "";
            _vm.Content = "C";
            _vm.SelectedTopic = "Stock News";

            await InvokeSubmitAsync();

            Assert.AreEqual("Summary is required.", _vm.ErrorMessage);
        }

        [TestMethod]
        public async Task SubmitArticleAsync_EmptyContent_SetsError()
        {
            _vm.Title = "T";
            _vm.Summary = "S";
            _vm.Content = "";
            _vm.SelectedTopic = "Stock News";

            await InvokeSubmitAsync();

            Assert.AreEqual("Content is required.", _vm.ErrorMessage);
        }

        [TestMethod]
        public async Task SubmitArticleAsync_EmptyTopic_SetsError()
        {
            _vm.Title = "T";
            _vm.Summary = "S";
            _vm.Content = "C";
            _vm.SelectedTopic = "";

            await InvokeSubmitAsync();

            Assert.AreEqual("Topic is required.", _vm.ErrorMessage);
        }

        [TestMethod]
        public void ParseRelatedStocks_CommaSeparated_ReturnsList()
        {
            _vm.RelatedStocksText = "AAPL, MSFT, ,";

            var list = (List<string>)typeof(ArticleCreationViewModel)
                .GetMethod("ParseRelatedStocks", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(_vm, null)!;

            CollectionAssert.AreEqual(new[] { "AAPL", "MSFT" }, list);
        }
    }
}