using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockApp.Models;
using StockApp.Repositories;

namespace StockApp.Repository.Tests
{
    [TestClass]
    public class BaseStocksRepositoryTests
    {
        private BaseStocksRepository _repo;

        [TestInitialize]
        public void Init()
        {
            _repo = (BaseStocksRepository)System.Runtime.Serialization.FormatterServices
                .GetUninitializedObject(typeof(BaseStocksRepository));

            var fld = typeof(BaseStocksRepository)
                .GetField("stocks", BindingFlags.Instance | BindingFlags.NonPublic)!;
            fld.SetValue(_repo, new List<BaseStock>());
        }

        [TestMethod]
        public void GetAllStocks_InitiallyEmpty_ReturnsEmptyList()
        {
            var all = _repo.GetAllStocks();
            Assert.IsNotNull(all);
            Assert.AreEqual(0, all.Count);
        }

        [TestMethod]
        public void GetAllStocks_ReturnsSnapshot_NotSameReference()
        {
            var list = new List<BaseStock>
            {
                new BaseStock("A", "SYM", "123"),
                new BaseStock("B", "SYM2", "456")
            };
            typeof(BaseStocksRepository)
                .GetField("stocks", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(_repo, list);

            var result = _repo.GetAllStocks();

            CollectionAssert.AreEqual(
                list,
                result,
                "Should return all items in the same order");
            Assert.AreNotSame(
                list,
                result,
                "Should return a new list instance, not the backing field");
        }
    }
}
