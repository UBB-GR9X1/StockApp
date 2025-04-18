using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockApp.Repositories;
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace StockApp.Repository.Tests
{
    [TestClass]
    public class ProfileRepositoryTests
    {
        private ProfileRepository _repo;

        [TestInitialize]
        public void Init()
        {
            _repo = (ProfileRepository)FormatterServices.GetUninitializedObject(typeof(ProfileRepository));

            typeof(ProfileRepository)
                .GetField("loggedInUserCNP", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(_repo, "1234567890123");
        }

        [TestMethod]
        public void GenerateUsername_ReturnsRandomName()
        {
            var method = typeof(ProfileRepository).GetMethod("GenerateUsername")!;
            var result = method.Invoke(_repo, null) as string;

            Assert.IsFalse(string.IsNullOrWhiteSpace(result));
            Assert.IsTrue(result.Contains("_") || result.Length > 5); 
        }

        [TestMethod]
        public void CnpField_IsSetCorrectly()
        {
            var field = typeof(ProfileRepository)
                .GetField("loggedInUserCNP", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(_repo);

            Assert.AreEqual("1234567890123", field);
        }
    }
}
