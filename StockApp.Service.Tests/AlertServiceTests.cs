using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockApp.Models;
using StockApp.Services;
using System.Collections.Generic;

namespace StockApp.Service.Tests
{
    [TestClass]
    public class AlertServiceTests
    {
        private IAlertService _service;

        [TestInitialize]
        public void Init()
        {
            _service = new InMemoryAlertService();
        }

        [TestMethod]
        public void CreateAlert_AddsAlert()
        {
            var alert = _service.CreateAlert("AAPL", "Test Alert", 200, 100, true);

            Assert.IsNotNull(alert);
            Assert.AreEqual("AAPL", alert.StockName);
            Assert.AreEqual("Test Alert", alert.Name);
            Assert.AreEqual(200, alert.UpperBound);
            Assert.AreEqual(100, alert.LowerBound);
            Assert.IsTrue(alert.ToggleOnOff);
        }

        [TestMethod]
        public void UpdateAlert_ChangesFields()
        {
            var alert = _service.CreateAlert("AAPL", "Old Alert", 300, 150, true);
            _service.UpdateAlert(alert.AlertId, "AAPL", "Updated Alert", 400, 200, false);

            var updated = _service.GetAlertById(alert.AlertId);

            Assert.AreEqual("Updated Alert", updated.Name);
            Assert.AreEqual(400, updated.UpperBound);
            Assert.IsFalse(updated.ToggleOnOff);
        }

        [TestMethod]
        public void RemoveAlert_DeletesSuccessfully()
        {
            var alert = _service.CreateAlert("TSLA", "Temp Alert", 800, 600, true);
            _service.RemoveAlert(alert.AlertId);

            var found = _service.GetAlertById(alert.AlertId);
            Assert.IsNull(found);
        }
    }

    internal class InMemoryAlertService : IAlertService
    {
        private readonly List<Alert> _alerts = [];
        private int _nextId = 1;

        public List<Alert> GetAllAlerts() => [.. _alerts];

        public List<Alert> GetAllAlertsOn() => _alerts.FindAll(a => a.ToggleOnOff);

        public Alert? GetAlertById(int alertId) => _alerts.Find(a => a.AlertId == alertId);

        public Alert CreateAlert(string stockName, string name, decimal upperBound, decimal lowerBound, bool toggleOnOff)
        {
            var alert = new Alert
            {
                AlertId = _nextId++,
                StockName = stockName,
                Name = name,
                UpperBound = upperBound,
                LowerBound = lowerBound,
                ToggleOnOff = toggleOnOff
            };
            _alerts.Add(alert);
            return alert;
        }

        public void UpdateAlert(int alertId, string stockName, string name, decimal upperBound, decimal lowerBound, bool toggleOnOff)
        {
            var alert = GetAlertById(alertId);
            if (alert == null) return;

            alert.StockName = stockName;
            alert.Name = name;
            alert.UpperBound = upperBound;
            alert.LowerBound = lowerBound;
            alert.ToggleOnOff = toggleOnOff;
        }

        public void UpdateAlert(Alert alert) =>
            UpdateAlert(alert.AlertId, alert.StockName, alert.Name, alert.UpperBound, alert.LowerBound, alert.ToggleOnOff);

        public void RemoveAlert(int alertId)
        {
            _alerts.RemoveAll(a => a.AlertId == alertId);
        }
    }
}
