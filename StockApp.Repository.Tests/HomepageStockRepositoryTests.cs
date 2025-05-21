using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BankApi.Data;
using BankApi.Repositories.Impl;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockApp.Repository.Tests
{
    [TestClass]
    public class HomepageStockRepositoryTests
    {
        private ApiDbContext _context;
        private IHomepageStockRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApiDbContext(options);
            _repository = new HomepageStockRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnAllStocksForUser()
        {
            // Arrange
            var stocks = new List<HomepageStock>
            {
                new HomepageStock { Id = 1, Symbol = "AAPL", UserCNP = "123", IsFavorite = true },
                new HomepageStock { Id = 2, Symbol = "GOOGL", UserCNP = "123", IsFavorite = false },
                new HomepageStock { Id = 3, Symbol = "MSFT", UserCNP = "456", IsFavorite = true }
            };
            await _context.HomepageStocks.AddRangeAsync(stocks);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync("123");

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(s => s.Symbol == "AAPL"));
            Assert.IsTrue(result.Any(s => s.Symbol == "GOOGL"));
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnCorrectStock()
        {
            // Arrange
            var stock = new HomepageStock { Id = 1, Symbol = "AAPL", UserCNP = "123", IsFavorite = true };
            await _context.HomepageStocks.AddAsync(stock);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(1, "123");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AAPL", result.Symbol);
            Assert.IsTrue(result.IsFavorite);
        }

        [TestMethod]
        public async Task GetBySymbolAsync_ShouldReturnCorrectStock()
        {
            // Arrange
            var stock = new HomepageStock { Id = 1, Symbol = "AAPL", UserCNP = "123", IsFavorite = true };
            await _context.HomepageStocks.AddAsync(stock);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetBySymbolAsync("AAPL");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("123", result.UserCNP);
            Assert.IsTrue(result.IsFavorite);
        }

        [TestMethod]
        public async Task CreateAsync_WithValidData_ShouldCreateStock()
        {
            // Arrange
            var stock = new HomepageStock { Symbol = "AAPL", UserCNP = "123", IsFavorite = true };

            // Act
            var result = await _repository.CreateAsync(stock);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AAPL", result.Symbol);
            Assert.AreEqual("123", result.UserCNP);
            Assert.IsTrue(result.IsFavorite);
            Assert.AreEqual(1, await _context.HomepageStocks.CountAsync());
        }

        [TestMethod]
        public async Task UpdateAsync_WithValidData_ShouldUpdateStock()
        {
            // Arrange
            var stock = new HomepageStock { Id = 1, Symbol = "AAPL", UserCNP = "123", IsFavorite = true };
            await _context.HomepageStocks.AddAsync(stock);
            await _context.SaveChangesAsync();

            // Act
            stock.IsFavorite = false;
            var result = await _repository.UpdateAsync(1, stock);

            // Assert
            Assert.IsTrue(result);
            var updatedStock = await _context.HomepageStocks.FindAsync(1);
            Assert.IsFalse(updatedStock.IsFavorite);
        }

        [TestMethod]
        public async Task UpdateAsync_WithNonExistentId_ShouldReturnFalse()
        {
            // Arrange
            var stock = new HomepageStock { Id = 999, Symbol = "AAPL", UserCNP = "123", IsFavorite = true };

            // Act
            var result = await _repository.UpdateAsync(999, stock);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldDeleteStock()
        {
            // Arrange
            var stock = new HomepageStock { Id = 1, Symbol = "AAPL", UserCNP = "123", IsFavorite = true };
            await _context.HomepageStocks.AddAsync(stock);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteAsync(1);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, await _context.HomepageStocks.CountAsync());
        }

        [TestMethod]
        public async Task DeleteAsync_WithNonExistentId_ShouldReturnFalse()
        {
            // Act
            var result = await _repository.DeleteAsync(999);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
