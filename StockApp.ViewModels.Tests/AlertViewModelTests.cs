using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StockApp.Models;
using StockApp.Services;
using StockApp.ViewModels;
using System.Windows.Input;

namespace StockApp.ViewModels.Tests
{
    [TestClass]
    public class AlertViewModelTests
    {
        private Mock<IAlertService> _alertServiceMock;
        private Mock<IDialogService> _dialogServiceMock;
        private AlertViewModel _vm;

        [TestInitialize]
        public void SetUp()
        {
            _alertServiceMock = new Mock<IAlertService>();
            _dialogServiceMock = new Mock<IDialogService>();

            _alertServiceMock
                .Setup(s => s.GetAllAlerts())
                .Returns(new List<Alert>());

            _vm = new AlertViewModel(
                _alertServiceMock.Object,
                _dialogServiceMock.Object
            );
        }

        [TestMethod]
        public void LoadAlerts_Constructor_PopulatesAlertsCollection()
        {
            var initial = new List<Alert> {
                new Alert { AlertId = 1, Name = "A", UpperBound = 10, LowerBound = 0 }
            };
            _alertServiceMock
                .Setup(s => s.GetAllAlerts())
                .Returns(initial);

            _vm = new AlertViewModel(
                _alertServiceMock.Object,
                _dialogServiceMock.Object
            );

            CollectionAssert.AreEquivalent(initial, _vm.Alerts.ToList());
        }

        [TestMethod]
        public async Task CreateAlert_Success_AddsAlertAndShowsSuccess()
        {
            var created = new Alert { AlertId = 99, Name = "X", UpperBound = 5, LowerBound = 1, StockName = "Tesla", ToggleOnOff = true };
            _alertServiceMock
                .Setup(s => s.CreateAlert("Tesla", "New Alert", 100m, 0m, true))
                .Returns(created);

            await _vm.CreateAlertCommand.ExecuteAsync(null);

            Assert.AreEqual(1, _vm.Alerts.Count);
            Assert.AreSame(created, _vm.Alerts[0]);
            _dialogServiceMock.Verify(d =>
                d.ShowMessageAsync("Success", "Alert created successfully!"),
                Times.Once);
        }

        [TestMethod]
        public async Task CreateAlert_ServiceThrows_ShowsError()
        {
            _alertServiceMock
                .Setup(s => s.CreateAlert(It.IsAny<string>(), It.IsAny<string>(),
                                          It.IsAny<decimal>(), It.IsAny<decimal>(),
                                          It.IsAny<bool>()))
                .Throws(new InvalidOperationException("fail"));

            await _vm.CreateAlertCommand.ExecuteAsync(null);

            Assert.AreEqual(0, _vm.Alerts.Count);
            _dialogServiceMock.Verify(d =>
                d.ShowMessageAsync("Error", "fail"),
                Times.Once);
        }

        [TestMethod]
        public async Task SaveAlerts_AllValid_CallsUpdateAndShowsSuccess()
        {
            var a = new Alert { AlertId = 5, StockName = "S", Name = "N", UpperBound = 10, LowerBound = 0, ToggleOnOff = true };
            _vm.Alerts.Add(a);

            await _vm.SaveAlertsCommand.ExecuteAsync(null);

            _alertServiceMock.Verify(s =>
                s.UpdateAlert(5, "S", "N", 10m, 0m, true),
                Times.Once);
            _dialogServiceMock.Verify(d =>
                d.ShowMessageAsync("Success", "All alerts saved successfully!"),
                Times.Once);
        }

        [TestMethod]
        public async Task SaveAlerts_InvalidAlert_ShowsError()
        {
            var bad = new Alert { AlertId = 7, StockName = "B", Name = "B", UpperBound = 1, LowerBound = 2, ToggleOnOff = true };
            _vm.Alerts.Add(bad);

            await _vm.SaveAlertsCommand.ExecuteAsync(null);

            _alertServiceMock.Verify(s =>
                s.UpdateAlert(It.IsAny<int>(),
                              It.IsAny<string>(),
                              It.IsAny<string>(),
                              It.IsAny<decimal>(),
                              It.IsAny<decimal>(),
                              It.IsAny<bool>()),
                Times.Never);
            _dialogServiceMock.Verify(d =>
                d.ShowMessageAsync("Error", "Lower bound cannot be greater than upper bound."),
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteAlert_ValidAlert_RemovesAndShowsSuccess()
        {
            var a = new Alert { AlertId = 2, StockName = "D", Name = "D", UpperBound = 5, LowerBound = 0, ToggleOnOff = true };
            _vm.Alerts.Add(a);

            await _vm.DeleteAlertCommand.ExecuteAsync(a);

            Assert.IsFalse(_vm.Alerts.Contains(a));
            _alertServiceMock.Verify(s => s.RemoveAlert(2), Times.Once);
            _dialogServiceMock.Verify(d =>
                d.ShowMessageAsync("Success", "Alert deleted successfully!"),
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteAlert_NoParameter_ShowsError()
        {
            await _vm.DeleteAlertCommand.ExecuteAsync(null);

            _dialogServiceMock.Verify(d =>
                d.ShowMessageAsync("Error", "Please select an alert to delete."),
                Times.Once);
        }
    }

    internal static class CommandExtensions
    {
        public static Task ExecuteAsync(this ICommand cmd, object? param)
        {
            cmd.Execute(param);
            return Task.CompletedTask;
        }
    }
}