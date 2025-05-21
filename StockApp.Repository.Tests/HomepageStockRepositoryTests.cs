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

namespace StockApp.Repository.Tests.Repositories
{
    public class HomepageStockRepositoryTests
    {
        private readonly Mock<DbSet<HomepageStock>> _mockSet;
        private readonly Mock<ApiDbContext> _mockContext;
        private readonly Mock<ILogger<HomepageStockRepository>> _mockLogger;
        private readonly HomepageStockRepository _repository;

        public HomepageStockRepositoryTests()
        {
            _mockSet = new Mock<DbSet<HomepageStock>>();
            _mockContext = new Mock<ApiDbContext>();
            _mockLogger = new Mock<ILogger<HomepageStockRepository>>();
            _mockContext.Setup(c => c.HomepageStocks).Returns(_mockSet.Object);

            _repository = new HomepageStockRepository(_mockContext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateAsync_ValidStock_AddsSuccessfully()
        {
            var stock = new HomepageStock { Id = 1, Symbol = "AAPL" };

            var result = await _repository.CreateAsync(stock);

            _mockSet.Verify(s => s.AddAsync(stock, It.IsAny<CancellationToken>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(stock, result);
        }

        [Fact]
        public async Task CreateAsync_NullStock_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.CreateAsync(null));
        }

        [Fact]
        public async Task DeleteAsync_NonExistentStock_ReturnsFalse()
        {
            _mockContext.Setup(c => c.HomepageStocks.FindAsync(It.IsAny<int>())).ReturnsAsync((HomepageStock)null);

            var result = await _repository.DeleteAsync(123);

            Assert.False(result);
        }
    }
}
