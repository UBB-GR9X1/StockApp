using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Data.SqlClient;
using StockApp.Repositories;

namespace StockApp.Repository.Tests
{
    [TestClass]
    public class GemStoreRepositoryTests
    {
        private Mock<IDbExecutor> _dbMock;
        private GemStoreRepository _repo;

        [TestInitialize]
        public void Init()
        {
            _dbMock = new Mock<IDbExecutor>(MockBehavior.Strict);
            // inject our mock executor
            _repo = new GemStoreRepository(_dbMock.Object);
        }

        [TestMethod]
        public void GetCnp_ReturnsCurrentUserCnpFromUserRepository()
        {
            // Arrange
            var userRepo = new UserRepository();
            var expectedCnp = userRepo.CurrentUserCNP;

            // Act
            var actualCnp = _repo.GetCnp();

            // Assert
            Assert.AreEqual(expectedCnp, actualCnp);
        }

        [TestMethod]
        public void GetUserGemBalance_ParsesIntFromExecutor()
        {
            _dbMock
                .Setup(d => d.ExecuteScalar(
                    "SELECT GEM_BALANCE FROM [USER] WHERE CNP = @CNP",
                    It.IsAny<Action<SqlCommand>>()))
                .Returns(42);

            var balance = _repo.GetUserGemBalance("any-cnp");

            Assert.AreEqual(42, balance);
        }

        [TestMethod]
        public void UpdateUserGemBalance_CallsExecutorOnce()
        {
            const string expectedSql = "UPDATE [USER] SET GEM_BALANCE = @NewBalance WHERE CNP = @CNP";
            _dbMock
                .Setup(d => d.ExecuteNonQuery(
                    expectedSql,
                    It.IsAny<Action<SqlCommand>>()));

            _repo.UpdateUserGemBalance("CNPX", 999);

            _dbMock.Verify(d => d.ExecuteNonQuery(
                It.Is<string>(s => s == expectedSql),
                It.IsAny<Action<SqlCommand>>()), Times.Once);
        }

        [TestMethod]
        public void IsGuest_WhenCountZero_ReturnsTrue()
        {
            _dbMock
                .Setup(d => d.ExecuteScalar(
                    "SELECT COUNT(*) FROM [USER] WHERE CNP = @CNP",
                    It.IsAny<Action<SqlCommand>>()))
                .Returns(0);

            Assert.IsTrue(_repo.IsGuest("CNP0"));
        }

        [TestMethod]
        public void IsGuest_WhenCountNonZero_ReturnsFalse()
        {
            _dbMock
                .Setup(d => d.ExecuteScalar(
                    "SELECT COUNT(*) FROM [USER] WHERE CNP = @CNP",
                    It.IsAny<Action<SqlCommand>>()))
                .Returns(5);

            Assert.IsFalse(_repo.IsGuest("CNP5"));
        }
    }
}