using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StockApp.Services;

namespace StockApp.Service.Tests
{
    [TestClass]
    public class NavigationServiceTests
    {
        private Mock<INavigationFrame> _frameMock;

        [TestInitialize]
        public void Init()
        {
            _frameMock = new Mock<INavigationFrame>();
            NavigationService.Initialize(_frameMock.Object);
        }

        [TestMethod]
        public void CanGoBack_ReturnsTrue_WhenFrameSaysSo()
        {
            _frameMock.Setup(f => f.CanGoBack).Returns(true);
            Assert.IsTrue(NavigationService.Instance.CanGoBack);
        }

        [TestMethod]
        public void CanGoBack_ReturnsFalse_WhenFrameSaysFalse()
        {
            _frameMock.Setup(f => f.CanGoBack).Returns(false);
            Assert.IsFalse(NavigationService.Instance.CanGoBack);
        }

        [TestMethod]
        public void Navigate_CallsFrameNavigate()
        {
            var dummyPage = typeof(object);
            var dummyParam = "param";

            _frameMock.Setup(f => f.Navigate(dummyPage, dummyParam)).Returns(true);

            bool result = NavigationService.Instance.Navigate(dummyPage, dummyParam);

            Assert.IsTrue(result);
            _frameMock.Verify(f => f.Navigate(dummyPage, dummyParam), Times.Once);
        }

        [TestMethod]
        public void GoBack_CallsFrameGoBack_WhenCanGoBackIsTrue()
        {
            _frameMock.Setup(f => f.CanGoBack).Returns(true);

            NavigationService.Instance.GoBack();

            _frameMock.Verify(f => f.GoBack(), Times.Once);
        }

        [TestMethod]
        public void GoBack_DoesNothing_WhenCanGoBackIsFalse()
        {
            _frameMock.Setup(f => f.CanGoBack).Returns(false);

            NavigationService.Instance.GoBack();

            _frameMock.Verify(f => f.GoBack(), Times.Never);
        }
    }
}
