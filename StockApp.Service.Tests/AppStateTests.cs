using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockApp.Models;
using StockApp.Services;

namespace StockApp.Service.Tests
{
    [TestClass]
    public class AppStateTests
    {
        [TestMethod]
        public void Singleton_Instance_IsSame()
        {
            var a = AppState.Instance;
            var b = AppState.Instance;

            Assert.AreSame(a, b);
        }

        [TestMethod]
        public void CurrentUser_SetAndGet_Works()
        {
            var user = new User
            {
                CNP = "9999999999999",
                Username = "TestUser",
                Description = "Tester",
                IsModerator = true,
                Image = "pic.png",
                IsHidden = false,
                GemBalance = 1000
            };

            AppState.Instance.CurrentUser = user;

            Assert.AreEqual("TestUser", AppState.Instance.CurrentUser.Username);
            Assert.AreEqual("9999999999999", AppState.Instance.CurrentUser.CNP);
            Assert.IsTrue(AppState.Instance.CurrentUser.IsModerator);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void CurrentUser_SetToNull_Throws()
        {
            AppState.Instance.CurrentUser = null!;
        }
    }
}
