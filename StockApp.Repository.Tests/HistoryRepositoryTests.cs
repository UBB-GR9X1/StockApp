using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankApi.Data;
using BankApi.Repositories.Impl;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace StockApp.Repository.Tests
{
    public class HistoryRepositoryTests
    {
        private readonly Mock<DbSet<CreditScoreHistory>> _mockSet;
        private readonly Mock<ApiDbContext> _mockContext;
        private readonly HistoryRepository _repository;

        public HistoryRepositoryTests()
        {
            _mockSet = new Mock<DbSet<CreditScoreHistory>>();
            _mockContext = new Mock<ApiDbContext>();
            _mockContext.Setup(m => m.CreditScoreHistories).Returns(_mockSet.Object);

            _repository = new HistoryRepository(_mockContext.Object);
        }

        [Fact]
        public async Task AddHistoryAsync_ValidHistory_AddsSuccessfully()
        {
            var mockSet = new Mock<DbSet<CreditScoreHistory>>();
            var mockContext = new Mock<ApiDbContext>();
            mockContext.Setup(m => m.CreditScoreHistories).Returns(mockSet.Object);

            var repo = new HistoryRepository(mockContext.Object);
            var history = new CreditScoreHistory { Score = 700 };

            var result = await repo.AddHistoryAsync(history);

            mockSet.Verify(m => m.AddAsync(history, default), Times.Once);
            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
            Assert.Equal(history, result);
        }

        [Fact]
        public async Task AddHistoryAsync_NullHistory_ThrowsArgumentNullException()
        {
            var repo = new HistoryRepository(new Mock<ApiDbContext>().Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => repo.AddHistoryAsync(null));
        }

        [Fact]
        public async Task AddHistoryAsync_InvalidScore_ThrowsArgumentException()
        {
            var repo = new HistoryRepository(new Mock<ApiDbContext>().Object);

            var history = new CreditScoreHistory { Score = -10 };

            await Assert.ThrowsAsync<ArgumentException>(() => repo.AddHistoryAsync(history));
        }

    }

}
