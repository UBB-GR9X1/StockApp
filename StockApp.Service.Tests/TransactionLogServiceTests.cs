using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankApi.Repositories;
using BankApi.Repositories.Exporters;
using BankApi.Services;
using Common.Exceptions;
using Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace StockApp.Service.Tests
{
    [TestClass]
    public class TransactionLogServiceTests
    {
        private Mock<ITransactionRepository> _mockRepo;
        private TransactionLogService _service;

        [TestInitialize]
        public void Init()
        {
            _mockRepo = new Mock<ITransactionRepository>();
            _service = new TransactionLogService(_mockRepo.Object);
        }

        [TestMethod]
        public async Task GetFilteredTransactions_HappyCase_ReturnsList()
        {
            var criteria = new TransactionFilterCriteria();
            var transactions = new List<TransactionLogTransaction> { new TransactionLogTransaction() };
            var validateCalled = false;
            var criteriaMock = new Mock<TransactionFilterCriteria>();
            criteriaMock.Setup(c => c.Validate()).Callback(() => validateCalled = true);
            _mockRepo.Setup(r => r.GetByFilterCriteriaAsync(criteriaMock.Object)).ReturnsAsync(transactions);
            var result = await _service.GetFilteredTransactions(criteriaMock.Object);
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(validateCalled);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetFilteredTransactions_CriteriaInvalid_Throws()
        {
            var criteriaMock = new Mock<TransactionFilterCriteria>();
            criteriaMock.Setup(c => c.Validate()).Throws(new ArgumentException());
            await _service.GetFilteredTransactions(criteriaMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task GetFilteredTransactions_RepositoryThrows_Throws()
        {
            var criteriaMock = new Mock<TransactionFilterCriteria>();
            criteriaMock.Setup(c => c.Validate());
            _mockRepo.Setup(r => r.GetByFilterCriteriaAsync(criteriaMock.Object)).ThrowsAsync(new Exception());
            await _service.GetFilteredTransactions(criteriaMock.Object);
        }

        [TestMethod]
        public void SortTransactions_HappyCase_SortsByDateAscending()
        {
            var t1 = new TransactionLogTransaction { Date = DateTime.Now.AddDays(-1) };
            var t2 = new TransactionLogTransaction { Date = DateTime.Now };
            var list = new List<TransactionLogTransaction> { t2, t1 };
            var sorted = _service.SortTransactions(list, "Date", true);
            Assert.AreEqual(t1, sorted[0]);
        }

        [TestMethod]
        public void SortTransactions_HappyCase_SortsByStockNameDescending()
        {
            var t1 = new TransactionLogTransaction { StockName = "A" };
            var t2 = new TransactionLogTransaction { StockName = "B" };
            var list = new List<TransactionLogTransaction> { t1, t2 };
            var sorted = _service.SortTransactions(list, "Stock Name", false);
            Assert.AreEqual(t2, sorted[0]);
        }

        [TestMethod]
        public void SortTransactions_HappyCase_SortsByTotalValueAscending()
        {
            // TotalValue is read-only, so set Amount and PricePerStock
            var t1 = new TransactionLogTransaction { Amount = 1, PricePerStock = 1 };
            var t2 = new TransactionLogTransaction { Amount = 1, PricePerStock = 2 };
            var list = new List<TransactionLogTransaction> { t2, t1 };
            var sorted = _service.SortTransactions(list, "Total Value", true);
            Assert.AreEqual(t1, sorted[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSortTypeException))]
        public void SortTransactions_InvalidSortType_Throws()
        {
            var list = new List<TransactionLogTransaction>();
            _service.SortTransactions(list, "InvalidType");
        }

        [TestMethod]
        [ExpectedException(typeof(ExportFormatNotSupportedException))]
        public void ExportTransactions_UnsupportedFormat_Throws()
        {
            var transactions = new List<TransactionLogTransaction>();
            _service.ExportTransactions(transactions, "file.csv", "xml");
        }
    }
}
