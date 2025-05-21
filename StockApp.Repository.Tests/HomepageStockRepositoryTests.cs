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
        private readonly Mock<ApiDbContext> _mockContext;
        private readonly Mock<ILogger<HomepageStockRepository>> _mockLogger;
        private readonly HomepageStockRepository _repository;

        public HomepageStockRepositoryTests()
        {
            _mockContext = new Mock<ApiDbContext>();
            _mockLogger = new Mock<ILogger<HomepageStockRepository>>();

            _repository = new HomepageStockRepository(_mockContext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateAsync_ValidStock_ReturnsCreatedStock()
        {
            var stock = new HomepageStock { Id = 1, Symbol = "AAPL" };
            var dbSetMock = new Mock<DbSet<HomepageStock>>();
            _mockContext.Setup(m => m.HomepageStocks).Returns(dbSetMock.Object);

            var result = await _repository.CreateAsync(stock);

            dbSetMock.Verify(m => m.AddAsync(stock, It.IsAny<CancellationToken>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(stock, result);
        }

        [Fact]
        public async Task CreateAsync_NullStock_ThrowsException()
        {
            HomepageStock stock = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.CreateAsync(stock));
        }

        [Fact]
        public async Task DeleteAsync_NonexistentStock_ReturnsFalse()
        {
            var dbSetMock = new Mock<DbSet<HomepageStock>>();
            _mockContext.Setup(m => m.HomepageStocks).Returns(dbSetMock.Object);
            _mockContext.Setup(m => m.HomepageStocks.Include(It.IsAny<string>())).Returns(dbSetMock.Object);
            _mockContext.Setup(m => m.HomepageStocks.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<HomepageStock, bool>>>(), default))
                        .ReturnsAsync((HomepageStock)null);

            var result = await _repository.DeleteAsync(999);

            Assert.False(result);
        }
    }
}
