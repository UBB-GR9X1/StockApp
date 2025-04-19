namespace StockApp.Repository.Tests
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StockApp.Models;
    using StockApp.Repositories.Exporters;

    [TestClass]
    public class HTMLTransactionExporterTests
    {
        private HTMLTransactionExporter _exporter;
        private string _tempFile;

        [TestInitialize]
        public void Init()
        {
            _exporter = new HTMLTransactionExporter();
            _tempFile = Path.GetTempFileName();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_tempFile))
                File.Delete(_tempFile);
        }

        [TestMethod]
        public void Export_EmptyList_ProducesTableWithHeaderOnly()
        {
            _exporter.Export([], _tempFile);

            var html = File.ReadAllText(_tempFile);

            StringAssert.StartsWith(html, "<html>");
            StringAssert.Contains(html, "<h1>Transaction Log</h1>");
            StringAssert.Contains(html, "<table border='1'>");

            const string headerRow =
                "<tr>" +
                "<th>Stock Name</th>" +
                "<th>Type</th>" +
                "<th>Amount</th>" +
                "<th>Total Value</th>" +
                "<th>Date</th>" +
                "<th>Author</th>" +
                "</tr>";
            StringAssert.Contains(html, headerRow);

            Assert.IsFalse(html.Contains("<td>"), "No <td> elements should appear for an empty list");
        }

        [TestMethod]
        public void Export_SingleTransaction_RendersAllFields()
        {
            var dt = new DateTime(2025, 4, 20, 13, 45, 0);
            var tx = new TransactionLogTransaction(
                stockSymbol: "ABC",
                stockName: "MyCompany",
                type: "SELL",
                amount: 5,
                pricePerStock: 20,
                date: dt,
                author: "Bob"
            );

            _exporter.Export([tx], _tempFile);

            var html = File.ReadAllText(_tempFile);

            StringAssert.Contains(html, "<h1>Transaction Log</h1>");

            StringAssert.Contains(html, "<td>MyCompany</td>");
            StringAssert.Contains(html, "<td>SELL</td>");
            StringAssert.Contains(html, "<td>5</td>");
            StringAssert.Contains(html, "<td>100</td>");
            StringAssert.Contains(html, $"<td>{dt}</td>");
            StringAssert.Contains(html, "<td>Bob</td>");
        }
    }
}