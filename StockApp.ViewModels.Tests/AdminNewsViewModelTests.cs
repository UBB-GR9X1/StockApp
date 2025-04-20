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
    public class AdminNewsViewModelTests
    {
        private Mock<INewsService> _newsServiceMock;
        private Mock<IDispatcher> _dispatcherMock;
        private AdminNewsViewModel _vm;

        [TestInitialize]
        public void Setup()
        {
            _newsServiceMock = new Mock<INewsService>();
            _dispatcherMock = new Mock<IDispatcher>(MockBehavior.Strict);
            _dispatcherMock
                .Setup(d => d.TryEnqueue(It.IsAny<DispatcherQueueHandler>()))
                .Callback<DispatcherQueueHandler>(cb => cb())
                .Returns(true);

            _vm = new AdminNewsViewModel(
                _newsServiceMock.Object,
                _dispatcherMock.Object
            );
        }

        private Task InvokeAsync(string methodName, params object[] args)
        {
            var m = typeof(AdminNewsViewModel)
                .GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)
             ?? throw new InvalidOperationException($"Method '{methodName}' not found");
            return (Task)m.Invoke(_vm, args)!;
        }

        [TestMethod]
        public async Task RefreshArticlesAsync_NoFilters_PopulatesUserArticles()
        {
            var sample = new List<UserArticle>
            {
                new UserArticle (
                    "123",
                    "User Article Title",
                    "User Article Summary",
                    "User Article Content",
                    new User("testUser"),
                    DateTime.Now,
                    "Pending",
                    "Finance",
                    new List<string> { "AAPL" }
                    )
            };
            _newsServiceMock
                .Setup(s => s.GetUserArticlesAsync(null, null))
                .ReturnsAsync(sample);

            await InvokeAsync("RefreshArticlesAsync");

            Assert.AreEqual(1, _vm.UserArticles.Count);
            Assert.IsFalse(_vm.IsLoading);
            Assert.IsFalse(_vm.IsEmptyState);
        }

        [TestMethod]
        public async Task RefreshArticlesAsync_EmptyList_SetsEmptyStateTrue()
        {
            _newsServiceMock
                .Setup(s => s.GetUserArticlesAsync(null, null))
                .ReturnsAsync([]);

            await InvokeAsync("RefreshArticlesAsync");

            Assert.IsTrue(_vm.IsEmptyState);
        }

        [TestMethod]
        public async Task RefreshArticlesAsync_ServiceThrows_SetsEmptyStateTrue()
        {
            _newsServiceMock
                .Setup(s => s.GetUserArticlesAsync(null, null))
                .ThrowsAsync(new Exception("fail"));

            await InvokeAsync("RefreshArticlesAsync");

            Assert.IsTrue(_vm.IsEmptyState);
            Assert.IsFalse(_vm.IsLoading);
        }

        [TestMethod]
        public void SelectedStatus_Setter_InvokesServiceWithStatus()
        {
            var called = false;
            _newsServiceMock
                .Setup(s => s.GetUserArticlesAsync("Pending", null))
                .ReturnsAsync([])
                .Callback(() => called = true);

            _vm.SelectedStatus = "Pending";

            Assert.IsTrue(called);
        }

        [TestMethod]
        public void SelectedTopic_Setter_InvokesServiceWithTopic()
        {
            var called = false;
            _newsServiceMock
                .Setup(s => s.GetUserArticlesAsync(null, "Economy"))
                .ReturnsAsync([])
                .Callback(() => called = true);

            _vm.SelectedTopic = "Economy";

            Assert.IsTrue(called);
        }
    }
}