using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StockApp.Database;
using StockApp.Models;
using StockApp.Repositories;

namespace StockApp.Repository.Tests
{
    [TestClass]
    public class BaseStocksRepositoryTests
    {
        private Mock<AppDbContext> _dbContextMock;
        private Mock<DbSet<BaseStock>> _mockSet;
        private List<BaseStock> _stockData;
        private BaseStocksRepository _repo;

        [TestInitialize]
        public void Init()
        {
            _stockData = new List<BaseStock>();
            _mockSet = CreateMockDbSet(_stockData);

            _dbContextMock = new Mock<AppDbContext>();
            _dbContextMock.Setup(m => m.BaseStocks).Returns(_mockSet.Object);

            _repo = new BaseStocksRepository(_dbContextMock.Object);
        }

        private static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            mockSet.Setup(m => m.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
                .Callback<T, CancellationToken>((entity, token) => data.Add(entity))
                .ReturnsAsync((T entity, CancellationToken token) => null!);

            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] keys) => data.FirstOrDefault(d => d is BaseStock bs && bs.Name == (string)keys[0]));

            return mockSet;
        }

        [TestMethod]
        public async Task GetAllStocksAsync_InitiallyEmpty_ReturnsEmptyList()
        {
            var all = await _repo.GetAllStocksAsync();
            Assert.IsNotNull(all);
            Assert.AreEqual(0, all.Count);
        }

        [TestMethod]
        public async Task GetAllStocksAsync_WithData_ReturnsAllData()
        {
            // Add test data
            _stockData.Add(new BaseStock("A", "SYM", "123"));
            _stockData.Add(new BaseStock("B", "SYM2", "456"));

            var result = await _repo.GetAllStocksAsync();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("A", result[0].Name);
            Assert.AreEqual("B", result[1].Name);
        }

        [TestMethod]
        public async Task AddStockAsync_AddsStockToRepository()
        {
            // Setup DbContext to simulate SaveChanges
            _dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var stock = new BaseStock("TestStock", "TS", "12345");
            
            // Act
            var result = await _repo.AddStockAsync(stock);
            
            // Assert
            // Verify the stock was added to DbSet
            _mockSet.Verify(m => m.AddAsync(It.IsAny<BaseStock>(), It.IsAny<CancellationToken>()), Times.Once);
            
            // Verify SaveChanges was called
            _dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            
            // Verify returned stock
            Assert.AreEqual("TestStock", result.Name);
            Assert.AreEqual("TS", result.Symbol);
        }
    }
}
