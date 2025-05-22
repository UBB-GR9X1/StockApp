using BankApi.Services;
using BankApi.Repositories;
using Common.Exceptions;
using Common.Models;
using Common.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace StockApp.Service.Tests
{
    [TestClass]
    public class StoreServiceTests
    {
        private Mock<IGemStoreRepository> _storeRepoMock;
        private Mock<IUserRepository> _userRepoMock;

        private const string ValidUserCNP = "1234567890123";
        private const string ValidAccountId = "acc-001";

        [TestInitialize]
        public void Setup()
        {
            _storeRepoMock = new Mock<IGemStoreRepository>();
            _userRepoMock = new Mock<IUserRepository>();
        }

        private StoreService CreateService(bool transactionShouldSucceed = true)
        {
            return new TestableStoreService(_storeRepoMock.Object, _userRepoMock.Object, transactionShouldSucceed);
        }

        private class TestableStoreService : StoreService
        {
            private readonly bool _transactionResult;

            public TestableStoreService(IGemStoreRepository repo, IUserRepository userRepo, bool transactionResult)
                : base(repo, userRepo)
            {
                _transactionResult = transactionResult;
            }

            protected override Task<bool> ProcessBankTransaction(string accountId, double amount)
            {
                return Task.FromResult(_transactionResult);
            }
        }

        [TestMethod]
        public async Task GetUserGemBalanceAsync_ReturnsCorrectBalance()
        {
            // Arrange
            _storeRepoMock.Setup(r => r.GetUserGemBalanceAsync(ValidUserCNP)).ReturnsAsync(150);
            var service = CreateService();

            // Act
            var balance = await service.GetUserGemBalanceAsync(ValidUserCNP);

            // Assert
            Assert.AreEqual(150, balance);
        }

        [TestMethod]
        public async Task UpdateUserGemBalanceAsync_CallsRepository()
        {
            // Arrange
            var service = CreateService();

            // Act
            await service.UpdateUserGemBalanceAsync(200, ValidUserCNP);

            // Assert
            _storeRepoMock.Verify(r => r.UpdateUserGemBalanceAsync(ValidUserCNP, 200), Times.Once);
        }

        [TestMethod]
        public async Task BuyGems_HappyPath_UpdatesGemBalance()
        {
            // Arrange
            var deal = new GemDeal("Gold Pack", 50, 10.0, false);
            _storeRepoMock.Setup(r => r.GetUserGemBalanceAsync(ValidUserCNP)).ReturnsAsync(100);
            var service = CreateService();

            // Act
            var result = await service.BuyGems(deal, ValidAccountId, ValidUserCNP);

            // Assert
            _storeRepoMock.Verify(r => r.UpdateUserGemBalanceAsync(ValidUserCNP, 150), Times.Once);
            Assert.AreEqual("Successfully purchased 50 gems for 10€", result);
        }

        [TestMethod]
        [ExpectedException(typeof(GemTransactionFailedException))]
        public async Task BuyGems_WhenBankTransactionFails_ThrowsGemTransactionFailedException()
        {
            // Arrange
            var deal = new GemDeal("Gold Pack", 50, 10.0, false);
            var service = CreateService(transactionShouldSucceed: false);

            // Act
            await service.BuyGems(deal, ValidAccountId, ValidUserCNP);

            // Assert: exception is expected
        }

        [TestMethod]
        public async Task SellGems_HappyPath_UpdatesGemBalance()
        {
            // Arrange
            _storeRepoMock.Setup(r => r.GetUserGemBalanceAsync(ValidUserCNP)).ReturnsAsync(100);
            var service = CreateService();

            // Act
            var result = await service.SellGems(50, ValidAccountId, ValidUserCNP);

            // Assert
            _storeRepoMock.Verify(r => r.UpdateUserGemBalanceAsync(ValidUserCNP, 50), Times.Once);
            Assert.AreEqual("Successfully sold 50 gems for 0.5€", result);
        }

        [TestMethod]
        [ExpectedException(typeof(InsufficientGemsException))]
        public async Task SellGems_WhenNotEnoughGems_ThrowsInsufficientGemsException()
        {
            // Arrange
            _storeRepoMock.Setup(r => r.GetUserGemBalanceAsync(ValidUserCNP)).ReturnsAsync(30);
            var service = CreateService();

            // Act
            await service.SellGems(50, ValidAccountId, ValidUserCNP);

            // Assert: exception is expected
        }

        [TestMethod]
        [ExpectedException(typeof(GemTransactionFailedException))]
        public async Task SellGems_WhenBankTransactionFails_ThrowsGemTransactionFailedException()
        {
            // Arrange
            _storeRepoMock.Setup(r => r.GetUserGemBalanceAsync(ValidUserCNP)).ReturnsAsync(100);
            var service = CreateService(transactionShouldSucceed: false);

            // Act
            await service.SellGems(50, ValidAccountId, ValidUserCNP);

            // Assert: exception is expected
        }
    }
}
