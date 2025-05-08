using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StockApp.Models;
using StockApp.Services;
using StockApp.ViewModels;

namespace StockApp.ViewModels.Tests
{
    [TestClass]
    public class UpdateProfilePageViewModelTests
    {
        private Mock<IProfileService> _serviceMock;
        private UpdateProfilePageViewModel _vm;
        private List<Stock> _stockList;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IProfileService>(MockBehavior.Strict);

            _serviceMock.Setup(s => s.GetImage()).Returns("http://example.com/me.png");
            _serviceMock.Setup(s => s.GetUsername()).Returns("testuser");
            _serviceMock.Setup(s => s.GetDescription()).Returns("Hello!");
            _serviceMock.Setup(s => s.IsHidden()).Returns(true);
            _serviceMock.Setup(s => s.IsAdmin()).Returns(false);

            _stockList =
            [
                new Stock("AAPL","AAPL","Apple",0,0),
                new Stock("MSFT","MSFT","Microsoft",0,0)
            ];
            _serviceMock.Setup(s => s.GetUserStocks()).Returns(_stockList);

            _serviceMock
                .Setup(s => s.UpdateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));
            _serviceMock.Setup(s => s.UpdateIsAdmin(It.IsAny<bool>()));

            _vm = new UpdateProfilePageViewModel(_serviceMock.Object);
        }

        [TestMethod]
        public void GetImage_DelegatesToService()
        {
            Assert.AreEqual("http://example.com/me.png", _vm.GetImage());
            _serviceMock.Verify(s => s.GetImage(), Times.Once);
        }

        [TestMethod]
        public void GetUsername_DelegatesToService()
        {
            Assert.AreEqual("testuser", _vm.GetUsername());
            _serviceMock.Verify(s => s.GetUsername(), Times.Once);
        }

        [TestMethod]
        public void GetDescription_DelegatesToService()
        {
            Assert.AreEqual("Hello!", _vm.GetDescription());
            _serviceMock.Verify(s => s.GetDescription(), Times.Once);
        }

        [TestMethod]
        public void IsHidden_DelegatesToService()
        {
            Assert.IsTrue(_vm.IsHidden());
            _serviceMock.Verify(s => s.IsHidden(), Times.Once);
        }

        [TestMethod]
        public void IsAdmin_DelegatesToService()
        {
            Assert.IsFalse(_vm.IsAdmin());
            _serviceMock.Verify(s => s.IsAdmin(), Times.Once);
        }

        [TestMethod]
        public void GetUserStocks_DelegatesToServiceAndReturnsSameInstance()
        {
            var result = _vm.GetUserStocks();
            Assert.AreSame(_stockList, result);
            _serviceMock.Verify(s => s.GetUserStocks(), Times.Once);
        }

        [TestMethod]
        public void UpdateAll_DelegatesToService()
        {
            _vm.UpdateAll("bob", "url.png", "desc", true);
            _serviceMock.Verify(s => s.UpdateUser("bob", "url.png", "desc", true), Times.Once);
        }

        [TestMethod]
        public void UpdateAdminMode_DelegatesToService()
        {
            _vm.UpdateAdminMode(true);
            _serviceMock.Verify(s => s.UpdateIsAdmin(true), Times.Once);
        }
    }
}