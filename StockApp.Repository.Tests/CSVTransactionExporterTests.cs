using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockApp.Models;
using StockApp.Repositories.Exporters;
using System.Collections.Generic;

namespace StockApp.Repository.Tests
{
    [TestClass]
    public class CSVTransactionExporterTests
    {
        private CSVTransactionExporter _exporter;
        private string _tempFile;

        [TestInitialize]
        public void Init()
        {
            _exporter = new CSVTransactionExporter();
            _tempFile = Path.GetTempFileName();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_tempFile))
                File.Delete(_tempFile);
        }

        [TestMethod]
        public void Export_EmptyList_WritesOnlyHeader()
        {
            _exporter.Export([], _tempFile);

            var lines = File.ReadAllLines(_tempFile);
            Assert.AreEqual(1, lines.Length, "Should only have header when no transactions supplied.");
            Assert.AreEqual(
                "StockSymbol,StockName,TransactionType,Amount,PricePerStock,TotalValue,Date,Author",
                lines[0]);
        }

        [TestMethod]
        public void Export_SingleTransaction_WritesCorrectCsv()
        {
            var dt = new DateTime(2025, 1, 2, 15, 30, 45);
            var tx = new TransactionLogTransaction(
                stockSymbol: "SYM",
                stockName: "MyStock",
                type: "BUY",
                amount: 3,
                pricePerStock: 10,
                date: dt,
                author: "Alice"
            );

            _exporter.Export([tx], _tempFile);

            var lines = File.ReadAllLines(_tempFile);
            Assert.AreEqual(2, lines.Length, "Should have header plus one data row.");

            Assert.AreEqual(
                "StockSymbol,StockName,TransactionType,Amount,PricePerStock,TotalValue,Date,Author",
                lines[0]);

            var expectedDate = dt.ToString("yyyy-MM-dd HH:mm:ss");
            var expectedRow = $"SYM,MyStock,BUY,3,10,30,{expectedDate},Alice";
            Assert.AreEqual(expectedRow, lines[1]);
        }
    }
}
