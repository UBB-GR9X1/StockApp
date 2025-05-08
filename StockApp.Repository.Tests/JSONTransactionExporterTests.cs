using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockApp.Models;
using StockApp.Repositories.Exporters;

namespace StockApp.Repository.Tests
{
    [TestClass]
    public class JSONTransactionExporterTests
    {
        private JSONTransactionExporter _exporter;
        private string _tempFile;

        [TestInitialize]
        public void Init()
        {
            _exporter = new JSONTransactionExporter();
            _tempFile = Path.GetTempFileName();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_tempFile))
                File.Delete(_tempFile);
        }

        [TestMethod]
        public void Export_EmptyList_ProducesEmptyJsonArray()
        {
            _exporter.Export([], _tempFile);
            var json = File.ReadAllText(_tempFile);
            using var doc = JsonDocument.Parse(json);
            Assert.AreEqual(JsonValueKind.Array, doc.RootElement.ValueKind);
            Assert.AreEqual(0, doc.RootElement.GetArrayLength());
        }

        [TestMethod]
        public void Export_SingleTransaction_ProducesCorrectJsonObject()
        {
            var dt = new DateTime(2025, 1, 2, 15, 30, 45, DateTimeKind.Utc);
            var tx = new TransactionLogTransaction(
                "SYM",
                "MyStock",
                "BUY",
                3,
                10,
                dt,
                "Alice"
            );

            _exporter.Export([tx], _tempFile);
            var json = File.ReadAllText(_tempFile);
            using var doc = JsonDocument.Parse(json);
            var arr = doc.RootElement;
            Assert.AreEqual(1, arr.GetArrayLength());

            var el = arr[0];
            Assert.AreEqual("SYM", el.GetProperty("StockSymbol").GetString());
            Assert.AreEqual("MyStock", el.GetProperty("StockName").GetString());
            Assert.AreEqual("BUY", el.GetProperty("Type").GetString());
            Assert.AreEqual(3, el.GetProperty("Amount").GetInt32());
            Assert.AreEqual(10, el.GetProperty("PricePerStock").GetInt32());
            Assert.AreEqual(30, el.GetProperty("TotalValue").GetInt32());  // 3*10
            Assert.AreEqual(dt, el.GetProperty("Date").GetDateTime());
            Assert.AreEqual("Alice", el.GetProperty("Author").GetString());
        }
    }
}