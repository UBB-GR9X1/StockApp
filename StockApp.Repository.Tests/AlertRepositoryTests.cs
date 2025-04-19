using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockApp.Models;
using StockApp.Repositories;

namespace StockApp.Repository.Tests
{
    [TestClass]
    public class AlertRepositoryTests
    {
        private AlertRepository _repo;

        [TestInitialize]
        public void Init()
        {
            // Create instance without running LoadAlerts/DB logic
            _repo = (AlertRepository)FormatterServices.GetUninitializedObject(typeof(AlertRepository));

            // Initialize the backing fields for Alerts and TriggeredAlerts
            var repoType = typeof(AlertRepository);
            var fldAlerts = repoType.GetField("<Alerts>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!;
            var fldTriggered = repoType.GetField("<TriggeredAlerts>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!;

            fldAlerts.SetValue(_repo, new List<Alert>());
            fldTriggered.SetValue(_repo, new List<TriggeredAlert>());
        }

        [TestMethod]
        public void IsAlertTriggered_NoAlerts_ReturnsFalse()
        {
            Assert.IsFalse(_repo.IsAlertTriggered("FOO", 100m));
        }

        [TestMethod]
        public void IsAlertTriggered_AboveUpper_ReturnsTrue()
        {
            _repo.Alerts.Add(new Alert
            {
                AlertId = 1,
                StockName = "FOO",
                Name = "A",
                LowerBound = 10m,
                UpperBound = 50m,
                ToggleOnOff = true
            });

            Assert.IsTrue(_repo.IsAlertTriggered("FOO", 60m));
        }

        [TestMethod]
        public void IsAlertTriggered_BelowLower_ReturnsTrue()
        {
            _repo.Alerts.Add(new Alert
            {
                AlertId = 2,
                StockName = "BAR",
                Name = "B",
                LowerBound = 20m,
                UpperBound = 80m,
                ToggleOnOff = true
            });

            Assert.IsTrue(_repo.IsAlertTriggered("BAR", 10m));
        }

        [TestMethod]
        public void IsAlertTriggered_ToggleOff_ReturnsFalse()
        {
            _repo.Alerts.Add(new Alert
            {
                AlertId = 3,
                StockName = "BAZ",
                Name = "C",
                LowerBound = 5m,
                UpperBound = 15m,
                ToggleOnOff = false
            });

            Assert.IsFalse(_repo.IsAlertTriggered("BAZ", 100m));
            Assert.IsFalse(_repo.IsAlertTriggered("BAZ", 0m));
        }

        [TestMethod]
        public void TriggerAlert_AddsTriggeredAlert_WhenTriggered()
        {
            _repo.Alerts.Add(new Alert
            {
                AlertId = 4,
                StockName = "QUX",
                Name = "D",
                LowerBound = 30m,
                UpperBound = 70m,
                ToggleOnOff = true
            });

            _repo.TriggerAlert("QUX", 80m);

            Assert.AreEqual(1, _repo.TriggeredAlerts.Count);
            var ta = _repo.TriggeredAlerts[0];
            StringAssert.Contains(ta.Message, "Alert triggered for QUX");
        }

        [TestMethod]
        public void TriggerAlert_DoesNothing_WhenNotTriggered()
        {
            _repo.Alerts.Add(new Alert
            {
                AlertId = 5,
                StockName = "QUUX",
                Name = "E",
                LowerBound = 100m,
                UpperBound = 200m,
                ToggleOnOff = true
            });

            _repo.TriggerAlert("QUUX", 150m);
            Assert.AreEqual(0, _repo.TriggeredAlerts.Count);
        }

        [TestMethod]
        public void ClearTriggeredAlerts_EmptiesList()
        {
            _repo.TriggeredAlerts.Add(new TriggeredAlert { StockName = "A", Message = "m" });
            _repo.ClearTriggeredAlerts();
            Assert.AreEqual(0, _repo.TriggeredAlerts.Count);
        }
    }
}
