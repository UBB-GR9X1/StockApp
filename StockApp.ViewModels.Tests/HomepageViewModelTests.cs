using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StockApp.Models;
using StockApp.Services;

namespace StockApp.ViewModels.Tests
{
    [TestClass]
    public class HomepageViewModelTests
    {
        private Mock<IHomepageService> _serviceMock;
        private HomepageViewModel _vm;

        private readonly List<HomepageStock> _allStocks =
        [
            new HomepageStock { StockDetails = new Stock("AAPL", "AAPL", "AuthorCNP", 100, 10), IsFavorite = false },
            new HomepageStock { StockDetails = new Stock("GOOG", "GOOG", "AuthorCNP", 100, 10), IsFavorite = true  },
        ];

        [TestInitialize]
        public void SetUp()
        {
            _serviceMock = new Mock<IHomepageService>(MockBehavior.Strict);

            _serviceMock.Setup(s => s.IsGuestUser()).Returns(true);

            _serviceMock.Setup(s => s.GetUserCNP()).Returns("CNP123");

            var all = new ObservableCollection<HomepageStock>(_allStocks);
            var fav = new ObservableCollection<HomepageStock>(_allStocks.Where(st => st.IsFavorite));

            _serviceMock.SetupGet(s => s.AllStocks).Returns(all);
            _serviceMock.SetupGet(s => s.FavoriteStocks).Returns(fav);
            _serviceMock.Setup(s => s.GetAllStocks()).Returns(all);
            _serviceMock.Setup(s => s.GetFavoriteStocks()).Returns(fav);

            _serviceMock.Setup(s => s.FilterStocks(It.IsAny<string>()));
            _serviceMock.Setup(s => s.SortStocks(It.IsAny<string>()));
            _serviceMock.Setup(s => s.CreateUserProfile());
            _serviceMock.Setup(s => s.AddToFavorites(It.IsAny<HomepageStock>()));
            _serviceMock.Setup(s => s.RemoveFromFavorites(It.IsAny<HomepageStock>()));

            _vm = new HomepageViewModel(_serviceMock.Object);
        }

        [TestMethod]
        public void Constructor_SetsGuestUserAndVisibilities()
        {
            Assert.IsTrue(_vm.IsGuestUser);
            Assert.AreEqual(Visibility.Visible, _vm.GuestButtonVisibility);
            Assert.AreEqual(Visibility.Collapsed, _vm.ProfileButtonVisibility);
            Assert.IsFalse(_vm.CanModifyFavorites);
        }

        [TestMethod]
        public void Constructor_LoadsStocksIntoCollections()
        {
            CollectionAssert.AreEqual(
                _allStocks,
                _vm.FilteredAllStocks.ToList()
            );
            CollectionAssert.AreEqual(
                _allStocks.Where(s => s.IsFavorite).ToList(),
                _vm.FilteredFavoriteStocks.ToList()
            );
        }

        [TestMethod]
        public void GetUserCNP_ReturnsServiceValue()
        {
            Assert.AreEqual("CNP123", _vm.GetUserCNP);
        }

        [TestMethod]
        public void SearchCommand_FiltersAndReloads()
        {
            _vm.SearchQuery = "foo";
            _serviceMock.Verify(s => s.FilterStocks("foo"), Times.Once);
            _serviceMock.Verify(s => s.GetAllStocks(), Times.AtLeastOnce);
        }

        [TestMethod]
        public void SortCommand_SortsAndReloads()
        {
            _vm.SelectedSortOption = "NameAsc";
            _serviceMock.Verify(s => s.SortStocks("NameAsc"), Times.Once);
            _serviceMock.Verify(s => s.GetAllStocks(), Times.AtLeastOnce);
        }

        [TestMethod]
        public void CreateProfileCommand_CreatesProfileAndUpdatesState()
        {
            Assert.IsTrue(_vm.IsGuestUser);

            _vm.CreateProfileCommand.Execute(null);

            _serviceMock.Verify(s => s.CreateUserProfile(), Times.Once);
            Assert.IsFalse(_vm.IsGuestUser);
            Assert.IsTrue(_vm.CanModifyFavorites);
        }

        [TestMethod]
        public void ToggleFavorite_Null_DoesNothing()
        {
            _vm.ToggleFavorite(null);
            _serviceMock.Verify(s => s.AddToFavorites(It.IsAny<HomepageStock>()), Times.Never);
            _serviceMock.Verify(s => s.RemoveFromFavorites(It.IsAny<HomepageStock>()), Times.Never);
        }

        [TestMethod]
        public void ToggleFavorite_AddAndReload()
        {
            var stock = new HomepageStock { StockDetails = new Stock("XYZ", "XYZ", "AuthorCNP", 100, 10), IsFavorite = false };
            var all2 = new ObservableCollection<HomepageStock> { stock };
            var fav2 = new ObservableCollection<HomepageStock>();

            _serviceMock.Setup(s => s.GetAllStocks()).Returns(all2);
            _serviceMock.Setup(s => s.GetFavoriteStocks()).Returns(fav2);

            _vm.ToggleFavorite(stock);

            _serviceMock.Verify(s => s.AddToFavorites(stock), Times.Once);
            _serviceMock.Verify(s => s.GetAllStocks(), Times.AtLeastOnce);
        }

        [TestMethod]
        public void ToggleFavorite_RemoveAndReload()
        {
            var stock = new HomepageStock { StockDetails = new Stock("XYZ", "XYZ", "AuthorCNP", 100, 10), IsFavorite = true };
            var all2 = new ObservableCollection<HomepageStock> { stock };
            var fav2 = new ObservableCollection<HomepageStock> { stock };

            _serviceMock.Setup(s => s.GetAllStocks()).Returns(all2);
            _serviceMock.Setup(s => s.GetFavoriteStocks()).Returns(fav2);

            _vm.ToggleFavorite(stock);

            _serviceMock.Verify(s => s.RemoveFromFavorites(stock), Times.Once);
            _serviceMock.Verify(s => s.GetAllStocks(), Times.AtLeastOnce);
        }

    }
}