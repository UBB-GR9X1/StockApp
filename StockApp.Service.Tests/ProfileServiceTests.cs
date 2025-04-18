using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StockApp.Models;
using StockApp.Repositories;
using StockApp.Services;
using System.Collections.Generic;

namespace StockApp.Service.Tests
{
    [TestClass]
    public class ProfileServiceTests
    {
        private Mock<IUserRepository> _userRepoMock;
        private Mock<IProfileRepository> _profileRepoMock;
        private ProfileService _service;

        [TestInitialize]
        public void Init()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _profileRepoMock = new Mock<IProfileRepository>();

            var testUser = new User("123", "Test", "Description", true, "img.png", false, 500);

            _profileRepoMock.Setup(r => r.CurrentUser()).Returns(testUser);

            _profileRepoMock.Setup(r => r.UserStocks()).Returns(new List<Stock>
            {
                new Stock("Apple", "AAPL", "123", 150, 10),
                new Stock("Tesla", "TSLA", "123", 200, 5)
            });

            _service = new ProfileService(_userRepoMock.Object, _profileRepoMock.Object);
        }

        [TestMethod]
        public void GetImage_ReturnsCorrectImage()
        {
            var result = _service.GetImage();
            Assert.AreEqual("img.png", result);
        }

        [TestMethod]
        public void IsAdmin_ReturnsTrue()
        {
            var result = _service.IsAdmin();
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetUsername_ReturnsCorrectUsername()
        {
            var result = _service.GetUsername();
            Assert.AreEqual("Test", result);
        }

        [TestMethod]
        public void GetDescription_ReturnsCorrectDescription()
        {
            var result = _service.GetDescription();
            Assert.AreEqual("Description", result);
        }

        [TestMethod]
        public void IsHidden_ReturnsFalse()
        {
            var result = _service.IsHidden();
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetUserStocks_ReturnsExpectedStocks()
        {
            var stocks = _service.GetUserStocks();
            Assert.AreEqual(2, stocks.Count);
            Assert.AreEqual("Apple", stocks[0].Name);
            Assert.AreEqual("Tesla", stocks[1].Name);
        }

        [TestMethod]
        public void UpdateUser_DelegatesToRepository()
        {
            _service.UpdateUser("NewName", "newimg.png", "NewDescription", true);
            _profileRepoMock.Verify(r => r.UpdateMyUser("NewName", "newimg.png", "NewDescription", true), Times.Once);
        }

        [TestMethod]
        public void UpdateIsAdmin_DelegatesToRepository()
        {
            _service.UpdateIsAdmin(true);
            _profileRepoMock.Verify(r => r.UpdateRepoIsAdmin(true), Times.Once);
        }

        [TestMethod]
        public void GetLoggedInUserCnp_ReturnsCorrectCnp()
        {
            var result = _service.GetLoggedInUserCnp();
            Assert.AreEqual("123", result);
        }
    }
}
