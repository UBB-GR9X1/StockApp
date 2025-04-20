using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StockApp.Models;
using StockApp.Repositories;
using StockApp.Services;

namespace StockApp.Service.Tests
{
    [TestClass]
    public class HomepageServiceTests
    {
        private Mock<IHomepageStocksRepository> _repoMock;
        private HomepageService _service;

        [TestInitialize]
        public void Init()
        {
            _repoMock = new Mock<IHomepageStocksRepository>();

            var testStocks = new List<HomepageStock>
            {
                new HomepageStock { StockDetails = new Stock("Alpha", "ALP", "AuthorCNP", 100, 10), Change = "+2%", IsFavorite = true },
                new HomepageStock { StockDetails = new Stock("Beta", "BET", "AuthorCNP", 200, 20), Change = "+5%", IsFavorite = false },
            };

            _repoMock.Setup(r => r.LoadStocks()).Returns(testStocks);
            _repoMock.Setup(r => r.GetUserCnp()).Returns("1234567890123");
            _repoMock.Setup(r => r.IsGuestUser(It.IsAny<string>())).Returns(false);

            _service = new HomepageService(_repoMock.Object); // overload constructor
        }

        [TestMethod]
        public void Constructor_LoadsInitialStocks()
        {
            var all = _service.GetAllStocks();
            var favorites = _service.GetFavoriteStocks();

            Assert.AreEqual(2, all.Count);
            Assert.AreEqual(1, favorites.Count);
            Assert.AreEqual("Alpha", favorites[0].StockDetails.Name);
        }

        [TestMethod]
        public void FilterStocks_FiltersByQuery()
        {
            _service.FilterStocks("AlP");

            Assert.AreEqual(1, _service.FilteredAllStocks.Count);
            Assert.AreEqual("Alpha", _service.FilteredAllStocks[0].StockDetails.Name);
        }

        [TestMethod]
        public void SortStocks_ByName_SortsAlphabetically()
        {
            _service.FilterStocks(""); // setup
            _service.SortStocks("Sort by Name");

            var first = _service.FilteredAllStocks.First().StockDetails.Name;
            Assert.AreEqual("Alpha", first);
        }

        [TestMethod]
        public void IsGuestUser_ReturnsFalse()
        {
            Assert.IsFalse(_service.IsGuestUser());
        }
    }
}
