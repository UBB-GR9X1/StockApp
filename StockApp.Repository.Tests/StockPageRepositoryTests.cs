using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockApp.Repositories;
using System.Reflection;
using System.Runtime.Serialization;

namespace StockApp.Repository.Tests
{
    [TestClass]
    public class StockPageRepositoryTests
    {
        private StockPageRepository _repo;

        [TestInitialize]
        public void Init()
        {
            _repo = (StockPageRepository)FormatterServices.GetUninitializedObject(typeof(StockPageRepository));

            typeof(StockPageRepository)
                .GetField("cnp", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(_repo, "1234567890123");

            typeof(StockPageRepository)
                .GetField("<IsGuest>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(_repo, true);
        }

        [TestMethod]
        public void Repo_IsGuest_CanBeSet()
        {
            var isGuestProp = typeof(StockPageRepository).GetProperty("IsGuest");
            Assert.IsNotNull(isGuestProp);
            Assert.IsTrue((bool)isGuestProp.GetValue(_repo));
        }

        [TestMethod]
        public void UserField_CanBeSetAndRetrieved()
        {
            var userType = typeof(StockApp.Models.User);
            var user = (StockApp.Models.User)FormatterServices.GetUninitializedObject(userType);

            typeof(StockPageRepository)
                .GetProperty("User")
                ?.SetValue(_repo, user);

            Assert.AreEqual(user, _repo.User);
        }
    }
}
