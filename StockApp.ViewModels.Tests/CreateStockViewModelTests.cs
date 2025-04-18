using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StockApp.Services;
using StockApp.ViewModels;

namespace StockApp.ViewModels.Tests
{
    [TestClass]
    public class CreateStockViewModelTests
    {
        private Mock<ICreateStockService> _serviceMock;
        private CreateStockViewModel _vm;

        [TestInitialize]
        public void SetUp()
        {
            _serviceMock = new Mock<ICreateStockService>(MockBehavior.Strict);
            _serviceMock.Setup(s => s.CheckIfUserIsGuest()).Returns(false);
            _vm = new CreateStockViewModel(_serviceMock.Object);
        }

        [TestMethod]
        public void Initial_IsAdminTrue_CommandCannotExecuteUntilValid()
        {
            Assert.IsTrue(_vm.IsAdmin);
            Assert.IsFalse(_vm.CreateStockCommand.CanExecute(null));
        }

        [TestMethod]
        public void StockName_Empty_ShowsRequiredMessage()
        {
            _vm.StockName = "";
            _vm.StockSymbol = "SYM";
            _vm.AuthorCnp = "1234567890123";

            Assert.IsFalse(_vm.IsInputValid);
            Assert.AreEqual("Stock Name is required!", _vm.Message);
        }

        [TestMethod]
        public void StockName_InvalidChars_ShowsFormatMessage()
        {
            _vm.StockName = "Bad#Name";
            _vm.StockSymbol = "SYM";
            _vm.AuthorCnp = "1234567890123";

            Assert.IsFalse(_vm.IsInputValid);
            StringAssert.StartsWith(_vm.Message, "Stock Name must be max 20 characters");
        }

        [TestMethod]
        public void StockSymbol_Empty_ShowsRequiredMessage()
        {
            _vm.StockName = "Valid";
            _vm.StockSymbol = "";
            _vm.AuthorCnp = "1234567890123";

            Assert.IsFalse(_vm.IsInputValid);
            Assert.AreEqual("Stock Symbol is required!", _vm.Message);
        }

        [TestMethod]
        public void StockSymbol_InvalidFormat_ShowsFormatMessage()
        {
            _vm.StockName = "Valid";
            _vm.StockSymbol = "TOOLONG";
            _vm.AuthorCnp = "1234567890123";

            Assert.IsFalse(_vm.IsInputValid);
            StringAssert.StartsWith(_vm.Message, "Stock Symbol must be alphanumeric");
        }

        [TestMethod]
        public void AuthorCnp_Empty_ShowsRequiredMessage()
        {
            _vm.StockName = "Valid";
            _vm.StockSymbol = "SYM";
            _vm.AuthorCnp = "";

            Assert.IsFalse(_vm.IsInputValid);
            Assert.AreEqual("Author CNP is required!", _vm.Message);
        }

        [TestMethod]
        public void AuthorCnp_WrongLength_ShowsFormatMessage()
        {
            _vm.StockName = "Valid";
            _vm.StockSymbol = "SYM";
            _vm.AuthorCnp = "123";

            Assert.IsFalse(_vm.IsInputValid);
            Assert.AreEqual("Author CNP must be exactly 13 digits!", _vm.Message);
        }

        [TestMethod]
        public void ValidInputs_EnableCommand()
        {
            _vm.StockName = "Valid Stock";
            _vm.StockSymbol = "VS";
            _vm.AuthorCnp = "1234567890123";

            Assert.IsTrue(_vm.IsInputValid);
            Assert.IsTrue(_vm.CreateStockCommand.CanExecute(null));
        }

        [TestMethod]
        public void CreateStock_ServiceReturnsSuccess_ResetsFields()
        {
            _vm.StockName = "Valid";
            _vm.StockSymbol = "SY";
            _vm.AuthorCnp = "1234567890123";

            _serviceMock
                .Setup(s => s.AddStock("Valid", "SY", "1234567890123"))
                .Returns("Stock added successfully with initial value!");

            _vm.CreateStockCommand.Execute(null);

            Assert.AreEqual("Stock added successfully with initial value!", _vm.Message);
            Assert.AreEqual("", _vm.StockName);
            Assert.AreEqual("", _vm.StockSymbol);
            Assert.AreEqual("", _vm.AuthorCnp);
        }

        [TestMethod]
        public void CreateStock_ServiceReturnsError_DoesNotReset()
        {
            _vm.StockName = "Valid";
            _vm.StockSymbol = "SY";
            _vm.AuthorCnp = "1234567890123";

            _serviceMock
                .Setup(s => s.AddStock("Valid", "SY", "1234567890123"))
                .Returns("Error occurred");

            _vm.CreateStockCommand.Execute(null);

            Assert.AreEqual("Error occurred", _vm.Message);
            Assert.AreEqual("Valid", _vm.StockName);
            Assert.AreEqual("SY", _vm.StockSymbol);
            Assert.AreEqual("1234567890123", _vm.AuthorCnp);
        }
    }
}
