using BankApi.Data;
using BankApi.Repositories;
using BankApi.Repositories.Impl;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApp.Repository.Tests
{
    [TestClass]
    public class InvestmentsRepositoryTests
    {
        private ApiDbContext _context;
        private IInvestmentsRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApiDbContext(options);
            _repository = new InvestmentsRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetInvestmentsHistory_ShouldReturnAllInvestments()
        {
            // Arrange
            var investments = new List<Investment>
            {
                new Investment { Id = 1, InvestorCnp = "123", Amount = 1000, AmountReturned = -1 },
                new Investment { Id = 2, InvestorCnp = "456", Amount = 2000, AmountReturned = 2200 }
            };
            await _context.Investments.AddRangeAsync(investments);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetInvestmentsHistory();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(i => i.InvestorCnp == "123"));
            Assert.IsTrue(result.Any(i => i.InvestorCnp == "456"));
        }

        [TestMethod]
        public async Task AddInvestment_WithValidData_ShouldAddInvestment()
        {
            // Arrange
            var investment = new Investment
            {
                InvestorCnp = "123",
                Amount = 1000,
                AmountReturned = -1
            };

            // Act
            await _repository.AddInvestment(investment);

            // Assert
            Assert.AreEqual(1, await _context.Investments.CountAsync());
            var addedInvestment = await _context.Investments.FirstAsync();
            Assert.AreEqual("123", addedInvestment.InvestorCnp);
            Assert.AreEqual(1000, addedInvestment.Amount);
            Assert.AreEqual(-1, addedInvestment.AmountReturned);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task AddInvestment_WithNullInvestment_ShouldThrowException()
        {
            // Act
            await _repository.AddInvestment(null);
        }

        [TestMethod]
        public async Task UpdateInvestment_WithValidData_ShouldUpdateInvestment()
        {
            // Arrange
            var investment = new Investment
            {
                Id = 1,
                InvestorCnp = "123",
                Amount = 1000,
                AmountReturned = -1
            };
            await _context.Investments.AddAsync(investment);
            await _context.SaveChangesAsync();

            // Act
            await _repository.UpdateInvestment(1, "123", 1100);

            // Assert
            var updatedInvestment = await _context.Investments.FindAsync(1);
            Assert.AreEqual(1100, updatedInvestment.AmountReturned);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task UpdateInvestment_WithNonExistentId_ShouldThrowException()
        {
            // Act
            await _repository.UpdateInvestment(999, "123", 1100);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task UpdateInvestment_WithWrongInvestorCnp_ShouldThrowException()
        {
            // Arrange
            var investment = new Investment
            {
                Id = 1,
                InvestorCnp = "123",
                Amount = 1000,
                AmountReturned = -1
            };
            await _context.Investments.AddAsync(investment);
            await _context.SaveChangesAsync();

            // Act
            await _repository.UpdateInvestment(1, "456", 1100);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task UpdateInvestment_WithAlreadyProcessedInvestment_ShouldThrowException()
        {
            // Arrange
            var investment = new Investment
            {
                Id = 1,
                InvestorCnp = "123",
                Amount = 1000,
                AmountReturned = 1100
            };
            await _context.Investments.AddAsync(investment);
            await _context.SaveChangesAsync();

            // Act
            await _repository.UpdateInvestment(1, "123", 1200);
        }
    }
} 