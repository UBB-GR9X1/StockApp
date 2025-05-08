using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using StockApp.Services;

namespace StockApp.Service.Tests
{
    [TestClass]
    public class DialogServiceTests
    {
        [TestMethod]
        public async Task ShowMessageAsync_CalledWithCorrectParams()
        {
            var mockDialogService = new Mock<IDialogService>();
            var called = false;

            mockDialogService
                .Setup(s => s.ShowMessageAsync("Info", "Saved successfully"))
                .Callback(() => called = true)
                .Returns(Task.CompletedTask);

            await mockDialogService.Object.ShowMessageAsync("Info", "Saved successfully");

            Assert.IsTrue(called);
            mockDialogService.Verify(s => s.ShowMessageAsync("Info", "Saved successfully"), Times.Once);
        }
    }
}
