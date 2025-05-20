using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StockApp.Exceptions;
using StockApp.Models;
using StockApp.Repositories;
using StockApp.Repositories.Exporters;
using StockApp.Services;

namespace StockApp.Service.Tests
{
    [TestClass]
    public class TransactionLogServiceTests
    {
        private Mock<ITransactionRepository> _repoMock;
        private TransactionLogService _service;

        [TestInitialize]
        public void Init()
        {
            _repoMock = new Mock<ITransactionRepository>();
            _service = new TransactionLogService(_repoMock.Object);
        }

        [TestMethod]
        public void GetFilteredTransactions_ReturnsWhatRepoProvides()
        {
            var criteria = new TransactionFilterCriteria
            {
                StockName = "Foo",
                MinTotalValue = 10,
                MaxTotalValue = 100,
                StartDate = DateTime.Today.AddDays(-5),
                EndDate = DateTime.Today
            };

            var list = new List<TransactionLogTransaction>
            {
                new TransactionLogTransaction("SYM","Foo","BUY",1,50,DateTime.Now,"u")
            };
            _repoMock.Setup(r => r.GetByFilterCriteria(criteria)).Returns(list);

            var result = _service.GetFilteredTransactions(criteria);

            Assert.AreSame(list, result);
        }

        [TestMethod]
        public void SortTransactions_ByDateAscending_Works()
        {
            var old = DateTime.Today.AddDays(-1);
            var txns = new List<TransactionLogTransaction>
            {
                new("SYM","A","BUY",1,10,DateTime.Now,"u"),
                new("SYM","B","SELL",1,10,old,"u")
            };

            var sorted = _service.SortTransactions(txns, "Date", ascending: true);
            Assert.AreEqual(old, sorted[0].Date);
        }

        [TestMethod]
        public void SortTransactions_ByStockNameDescending_Works()
        {
            var txns = new List<TransactionLogTransaction>
            {
                new("SYM","A","BUY",1,10,DateTime.Now,"u"),
                new("SYM","Z","SELL",1,10,DateTime.Now,"u")
            };

            var sorted = _service.SortTransactions(txns, "Stock Name", ascending: false);
            Assert.AreEqual("Z", sorted[0].StockName);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSortTypeException))]
        public void SortTransactions_InvalidKey_Throws()
        {
            _service.SortTransactions([], "NotAKey", true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExportTransactions_NullTransactions_Throws()
        {
            _service.ExportTransactions(null, "p.csv", "csv");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExportTransactions_EmptyPath_Throws()
        {
            _service.ExportTransactions([], "", "csv");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ExportTransactions_EmptyFormat_Throws()
        {
            _service.ExportTransactions([], "p.csv", "");
        }

        [TestMethod]
        [ExpectedException(typeof(ExportFormatNotSupportedException))]
        public void ExportTransactions_UnsupportedFormat_Throws()
        {
            _service.ExportTransactions(
                [
                    new("SYM","Foo","BUY",1,10,DateTime.Now,"u")
                ],
                "p.abc",
                "abc");
        }

        [TestMethod]
        public void ExportTransactions_ValidFormats_DoNotThrow()
        {
            var dummy = new List<TransactionLogTransaction>
            {
                new("SYM","Foo","BUY",1,10,DateTime.Now,"u")
            };

            _service.ExportTransactions(dummy, "x.csv", "csv");
            _service.ExportTransactions(dummy, "x.json", "json");
            _service.ExportTransactions(dummy, "x.html", "html");
        }
    }
}
