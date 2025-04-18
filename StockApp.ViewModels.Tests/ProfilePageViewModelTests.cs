using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.UI.Xaml.Media.Imaging;
using StockApp.Models;
using StockApp.Services;
using StockApp.ViewModels;

namespace StockApp.ViewModels.Tests
{
    [TestClass]
    public class ProfilePageViewModelTests
    {
        private Mock<IProfileService> _svcMock;
        private ProfilePageViewModel _vm;

        [TestInitialize]
        public void SetUp()
        {
            _svcMock = new Mock<IProfileService>(MockBehavior.Strict);
            _vm = new ProfilePageViewModel(_svcMock.Object);
        }

        [TestMethod]
        public void Getters_DelegateToService()
        {
            _svcMock.Setup(s => s.GetLoggedInUserCnp()).Returns("CNP");
            _svcMock.Setup(s => s.GetUsername()).Returns("User");
            _svcMock.Setup(s => s.GetDescription()).Returns("Desc");
            _svcMock.Setup(s => s.IsHidden()).Returns(true);
            _svcMock.Setup(s => s.IsAdmin()).Returns(false);
            var stocks = new List<Stock> { new Stock("A", "A", "", 0, 0) };
            _svcMock.Setup(s => s.GetUserStocks()).Returns(stocks);

            Assert.AreEqual("CNP", _vm.GetLoggedInUserCnp());
            Assert.AreEqual("User", _vm.GetUsername());
            Assert.AreEqual("Desc", _vm.GetDescription());
            Assert.IsTrue(_vm.IsHidden());
            Assert.IsFalse(_vm.IsAdmin());
            CollectionAssert.AreEqual(stocks, _vm.GetUserStocks());
        }

        [TestMethod]
        public void UpdateAdminMode_DelegatesToService()
        {
            _svcMock.Setup(s => s.UpdateIsAdmin(true));
            _vm.UpdateAdminMode(true);
            _svcMock.Verify(s => s.UpdateIsAdmin(true), Times.Once);
        }

        [TestMethod]
        public void LoadProfileImage_EmptyUrl_DoesNothing()
        {
            _svcMock.Setup(s => s.GetImage()).Returns(string.Empty);

            _vm.ImageSource = null!;
            _vm.LoadProfileImage();

            Assert.IsNull(_vm.ImageSource);
        }

        [TestMethod]
        public void LoadProfileImage_InvalidUri_DoesNotThrow()
        {
            _svcMock.Setup(s => s.GetImage()).Returns("not a uri");

            _vm.ImageSource = null!;
            _vm.LoadProfileImage();

            Assert.IsNull(_vm.ImageSource);
        }
    }
}