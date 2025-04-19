using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockApp.Models;
using StockApp.Repositories;

namespace StockApp.Repository.Tests
{
    [TestClass]
    public class TransactionRepositoryTests
    {
        private ITransactionRepository _repo;

        [TestInitialize]
        public void Init()
        {
            _repo = new FakeTransactionRepository();
        }

        [TestMethod]
        public void AddTransaction_AppendsToList()
        {
            var transaction = new TransactionLogTransaction(
                stockSymbol: "AAPL",
                stockName: "Apple",
                type: "BUY",
                amount: 10,
                pricePerStock: 15,
                date: DateTime.Now,
                author: "1234567890123");

            _repo.AddTransaction(transaction);

            Assert.AreEqual(1, _repo.Transactions.Count);
            Assert.AreEqual("Apple", _repo.Transactions[0].StockName);
        }

        [TestMethod]
        public void GetByFilterCriteria_FiltersByType()
        {
            _repo.AddTransaction(new TransactionLogTransaction(
                stockSymbol: "MSFT",
                stockName: "Microsoft",
                type: "SELL",
                amount: 20,
                pricePerStock: 50,
                date: DateTime.Today,
                author: "999"));

            var result = _repo.GetByFilterCriteria(new TransactionFilterCriteria { Type = "SELL" });

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("SELL", result[0].Type);
        }

        private class FakeTransactionRepository : ITransactionRepository
        {
            public List<TransactionLogTransaction> Transactions { get; private set; } = [];

            public void AddTransaction(TransactionLogTransaction transaction)
            {
                Transactions.Add(transaction);
            }

            public List<TransactionLogTransaction> GetByFilterCriteria(TransactionFilterCriteria criteria)
            {
                return Transactions.FindAll(t =>
                    (string.IsNullOrEmpty(criteria.Type) || t.Type == criteria.Type)
                );
            }
        }
    }
}
