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
    /// <summary>
    /// Test implementation of IBaseStocksRepository for testing
    /// </summary>
    internal class TestBaseStocksRepository : IBaseStocksRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly Mock<DbSet<BaseStock>> _mockSet;
        private readonly List<BaseStock> _stockData;

        public TestBaseStocksRepository(AppDbContext dbContext, Mock<DbSet<BaseStock>> mockSet, List<BaseStock> stockData)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mockSet = mockSet ?? throw new ArgumentNullException(nameof(mockSet));
            _stockData = stockData ?? throw new ArgumentNullException(nameof(stockData));
        }

        public async Task<BaseStock> AddStockAsync(BaseStock stock, int initialPrice = 100)
        {
            if (stock == null)
            {
                throw new ArgumentNullException(nameof(stock));
            }

            // Check if stock already exists
            var existingStock = _stockData.FirstOrDefault(s => s.Name == stock.Name);
            if (existingStock != null)
            {
                throw new InvalidOperationException($"Stock with name '{stock.Name}' already exists.");
            }

            await _mockSet.Object.AddAsync(stock);
            await _dbContext.SaveChangesAsync();
            
            return stock;
        }

        public async Task<bool> DeleteStockAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Stock name cannot be null or empty.", nameof(name));
            }

            var stock = _stockData.FirstOrDefault(s => s.Name == name);
            if (stock == null)
            {
                return false;
            }

            _stockData.Remove(stock);
            await _dbContext.SaveChangesAsync();
            
            return true;
        }

        public async Task<List<BaseStock>> GetAllStocksAsync()
        {
            return await Task.FromResult(_stockData.ToList());
        }

        public async Task<BaseStock> GetStockByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Stock name cannot be null or empty.", nameof(name));
            }

            var stock = _stockData.FirstOrDefault(s => s.Name == name);
            if (stock == null)
            {
                throw new KeyNotFoundException($"Stock with name '{name}' not found.");
            }

            return await Task.FromResult(stock);
        }

        public async Task<BaseStock> UpdateStockAsync(BaseStock stock)
        {
            if (stock == null)
            {
                throw new ArgumentNullException(nameof(stock));
            }

            var existingStock = _stockData.FirstOrDefault(s => s.Name == stock.Name);
            if (existingStock == null)
            {
                throw new KeyNotFoundException($"Stock with name '{stock.Name}' not found.");
            }

            existingStock.Symbol = stock.Symbol;
            existingStock.AuthorCNP = stock.AuthorCNP;

            await _dbContext.SaveChangesAsync();
            
            return existingStock;
        }
    }

    [TestClass]
    public class BaseStocksRepositoryTests
    {
        private Mock<AppDbContext> _dbContextMock;
        private Mock<DbSet<BaseStock>> _mockSet;
        private List<BaseStock> _stockData;
        private TestBaseStocksRepository _repo;

        [TestInitialize]
        public void Init()
        {
            _stockData = new List<BaseStock>();
            _mockSet = CreateMockDbSet(_stockData);

            _dbContextMock = new Mock<AppDbContext>();
            _dbContextMock.Setup(m => m.BaseStocks).Returns(_mockSet.Object);
            _dbContextMock.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _repo = new TestBaseStocksRepository(_dbContextMock.Object, _mockSet, _stockData);
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
