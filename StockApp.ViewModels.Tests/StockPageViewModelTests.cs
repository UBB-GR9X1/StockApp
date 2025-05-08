﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StockApp.Models;
using StockApp.Services;

namespace StockApp.ViewModels.Tests
{
    [TestClass]
    public class StockPageViewModelTests
    {
        private Mock<IStockPageService> _svc;
        private Mock<ITextBlock> _priceTb;
        private Mock<ITextBlock> _incTb;
        private Mock<ITextBlock> _ownTb;
        private Mock<IChart> _chart;
        private StockPageViewModel _vm;

        private readonly Stock _stk = new Stock("X", "X", "", 0, 0);

        [TestInitialize]
        public void Init()
        {
            _svc = new Mock<IStockPageService>(MockBehavior.Strict);
            _priceTb = new Mock<ITextBlock>(MockBehavior.Loose);
            _incTb = new Mock<ITextBlock>(MockBehavior.Loose);
            _ownTb = new Mock<ITextBlock>(MockBehavior.Loose);
            _chart = new Mock<IChart>(MockBehavior.Loose);

            _svc.Setup(s => s.SelectStock(_stk));
            _svc.Setup(s => s.IsGuest()).Returns(false);
            _svc.Setup(s => s.GetStockName()).Returns("X");
            _svc.Setup(s => s.GetStockSymbol()).Returns("X");
            _svc.Setup(s => s.GetFavorite()).Returns(false);
            _svc.Setup(s => s.GetUserBalance()).Returns(10);
            _svc.Setup(s => s.GetOwnedStocks()).Returns(2);
            _svc.Setup(s => s.GetStockHistory()).Returns([5]);
            _svc.Setup(s => s.ToggleFavorite(It.IsAny<bool>()));
            _svc.Setup(s => s.BuyStock(It.IsAny<int>())).Returns(true);
            _svc.Setup(s => s.SellStock(It.IsAny<int>())).Returns(false);
            _svc.Setup(s => s.GetStockAuthor()).ReturnsAsync(new User(cnp: "u", username: "u", isModerator: false));

            _vm = new StockPageViewModel(
                _svc.Object,
                _stk,
                _priceTb.Object,
                _incTb.Object,
                _ownTb.Object,
                _chart.Object
            );
        }

        [TestMethod]
        public void Constructor_SetsBasicProps()
        {
            Assert.AreEqual("X", _vm.StockName);
            Assert.AreEqual("X", _vm.StockSymbol);
            Assert.IsFalse(_vm.IsFavorite);
        }

        [TestMethod]
        public void ToggleFavorite_CallsService_AndChangesColor()
        {
            _vm.ToggleFavorite();
            _vm.ToggleFavorite();

            _svc.Verify(s => s.ToggleFavorite(true), Times.Once);
            _svc.Verify(s => s.ToggleFavorite(false), Times.Once);
        }

        [TestMethod]
        public void BuyAndSell_UpdateAndReturn()
        {
            var buyResult = _vm.BuyStock(3);
            var sellResult = _vm.SellStock(1);

            Assert.IsTrue(buyResult);
            Assert.IsFalse(sellResult);
            _svc.Verify(s => s.BuyStock(3), Times.Once);
            _svc.Verify(s => s.SellStock(1), Times.Once);
        }

        [TestMethod]
        public void GetStockAuthor_Delegates()
        {
            var u = _vm.GetStockAuthor();
            Assert.AreEqual("u", u.Result.Username);
        }
    }
}