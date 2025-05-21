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
            _mockContext.Setup(c => c.CreditScoreHistories).Returns(_mockSet.Object);

            _repository = new HistoryRepository(_mockContext.Object);
        }

        [Fact]
        public async Task AddHistoryAsync_ValidHistory_AddsSuccessfully()
        {
            var history = new CreditScoreHistory { Score = 700 };

            var result = await _repository.AddHistoryAsync(history);

            _mockSet.Verify(s => s.AddAsync(history, It.IsAny<CancellationToken>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(history, result);
        }

        [Fact]
        public async Task AddHistoryAsync_NullHistory_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddHistoryAsync(null));
        }

        [Fact]
        public async Task AddHistoryAsync_InvalidScore_ThrowsArgumentException()
        {
            var history = new CreditScoreHistory { Score = -10 };
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.AddHistoryAsync(history));
        }
    }
}
