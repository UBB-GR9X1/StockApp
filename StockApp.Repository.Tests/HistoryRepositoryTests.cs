using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BankApi.Data;
using BankApi.Repositories.Impl;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockApp.Repository.Tests
{
    [TestClass]
    public class HistoryRepositoryTests
    {
        private ApiDbContext _context;
        private IHistoryRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApiDbContext(options);
            _repository = new HistoryRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetAllHistoryAsync_ShouldReturnAllHistory()
        {
            // Arrange
            var histories = new List<CreditScoreHistory>
            {
                new CreditScoreHistory { Id = 1, UserCnp = "123", Score = 750, Date = DateTime.UtcNow },
                new CreditScoreHistory { Id = 2, UserCnp = "456", Score = 800, Date = DateTime.UtcNow }
            };
            await _context.CreditScoreHistories.AddRangeAsync(histories);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllHistoryAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(h => h.UserCnp == "123"));
            Assert.IsTrue(result.Any(h => h.UserCnp == "456"));
        }

        [TestMethod]
        public async Task GetHistoryByIdAsync_ShouldReturnCorrectHistory()
        {
            // Arrange
            var history = new CreditScoreHistory { Id = 1, UserCnp = "123", Score = 750, Date = DateTime.UtcNow };
            await _context.CreditScoreHistories.AddAsync(history);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetHistoryByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("123", result.UserCnp);
            Assert.AreEqual(750, result.Score);
        }

        [TestMethod]
        public async Task AddHistoryAsync_WithValidData_ShouldAddHistory()
        {
            // Arrange
            var history = new CreditScoreHistory { UserCnp = "123", Score = 750, Date = DateTime.UtcNow };

            // Act
            var result = await _repository.AddHistoryAsync(history);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("123", result.UserCnp);
            Assert.AreEqual(750, result.Score);
            Assert.AreEqual(1, await _context.CreditScoreHistories.CountAsync());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task AddHistoryAsync_WithInvalidScore_ShouldThrowException()
        {
            // Arrange
            var history = new CreditScoreHistory { UserCnp = "123", Score = 1500, Date = DateTime.UtcNow };

            // Act
            await _repository.AddHistoryAsync(history);
        }

        [TestMethod]
        public async Task UpdateHistoryAsync_WithValidData_ShouldUpdateHistory()
        {
            // Arrange
            var history = new CreditScoreHistory { Id = 1, UserCnp = "123", Score = 750, Date = DateTime.UtcNow };
            await _context.CreditScoreHistories.AddAsync(history);
            await _context.SaveChangesAsync();

            // Act
            history.Score = 800;
            var result = await _repository.UpdateHistoryAsync(history);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(800, result.Score);
            var updatedHistory = await _context.CreditScoreHistories.FindAsync(1);
            Assert.AreEqual(800, updatedHistory.Score);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task UpdateHistoryAsync_WithNonExistentId_ShouldThrowException()
        {
            // Arrange
            var history = new CreditScoreHistory { Id = 999, UserCnp = "123", Score = 750, Date = DateTime.UtcNow };

            // Act
            await _repository.UpdateHistoryAsync(history);
        }

        [TestMethod]
        public async Task DeleteHistoryAsync_ShouldDeleteHistory()
        {
            // Arrange
            var history = new CreditScoreHistory { Id = 1, UserCnp = "123", Score = 750, Date = DateTime.UtcNow };
            await _context.CreditScoreHistories.AddAsync(history);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteHistoryAsync(1);

            // Assert
            Assert.AreEqual(0, await _context.CreditScoreHistories.CountAsync());
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task DeleteHistoryAsync_WithNonExistentId_ShouldThrowException()
        {
            // Act
            await _repository.DeleteHistoryAsync(999);
        }

        [TestMethod]
        public async Task GetHistoryForUserAsync_ShouldReturnUserHistory()
        {
            // Arrange
            var histories = new List<CreditScoreHistory>
            {
                new CreditScoreHistory { Id = 1, UserCnp = "123", Score = 750, Date = DateTime.UtcNow },
                new CreditScoreHistory { Id = 2, UserCnp = "123", Score = 800, Date = DateTime.UtcNow },
                new CreditScoreHistory { Id = 3, UserCnp = "456", Score = 700, Date = DateTime.UtcNow }
            };
            await _context.CreditScoreHistories.AddRangeAsync(histories);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetHistoryForUserAsync("123");

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(h => h.UserCnp == "123"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetHistoryForUserAsync_WithEmptyCnp_ShouldThrowException()
        {
            // Act
            await _repository.GetHistoryForUserAsync("");
        }

        [TestMethod]
        public async Task GetHistoryMonthlyAsync_ShouldReturnMonthlyHistory()
        {
            // Arrange
            var histories = new List<CreditScoreHistory>
            {
                new CreditScoreHistory { Id = 1, UserCnp = "123", Score = 750, Date = DateTime.UtcNow },
                new CreditScoreHistory { Id = 2, UserCnp = "123", Score = 800, Date = DateTime.UtcNow.AddMonths(-2) }
            };
            await _context.CreditScoreHistories.AddRangeAsync(histories);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetHistoryMonthlyAsync("123");

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(750, result[0].Score);
        }

        [TestMethod]
        public async Task GetHistoryYearlyAsync_ShouldReturnYearlyHistory()
        {
            // Arrange
            var histories = new List<CreditScoreHistory>
            {
                new CreditScoreHistory { Id = 1, UserCnp = "123", Score = 750, Date = DateTime.UtcNow },
                new CreditScoreHistory { Id = 2, UserCnp = "123", Score = 800, Date = DateTime.UtcNow.AddYears(-2) }
            };
            await _context.CreditScoreHistories.AddRangeAsync(histories);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetHistoryYearlyAsync("123");

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(750, result[0].Score);
        }
    }
}
