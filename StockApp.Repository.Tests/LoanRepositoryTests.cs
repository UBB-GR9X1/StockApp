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
    public class LoanRepositoryTests
    {
        private ApiDbContext _context;
        private ILoanRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApiDbContext(options);
            _repository = new LoanRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetLoansAsync_ShouldReturnAllLoans()
        {
            // Arrange
            var loans = new List<Loan>
            {
                new Loan { Id = 1, UserCnp = "123", Amount = 1000, Status = "Active" },
                new Loan { Id = 2, UserCnp = "456", Amount = 2000, Status = "Pending" }
            };
            await _context.Loans.AddRangeAsync(loans);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetLoansAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(l => l.UserCnp == "123"));
            Assert.IsTrue(result.Any(l => l.UserCnp == "456"));
        }

        [TestMethod]
        public async Task GetUserLoansAsync_ShouldReturnUserLoans()
        {
            // Arrange
            var loans = new List<Loan>
            {
                new Loan { Id = 1, UserCnp = "123", Amount = 1000, Status = "Active" },
                new Loan { Id = 2, UserCnp = "123", Amount = 2000, Status = "Pending" },
                new Loan { Id = 3, UserCnp = "456", Amount = 3000, Status = "Active" }
            };
            await _context.Loans.AddRangeAsync(loans);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserLoansAsync("123");

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(l => l.UserCnp == "123"));
        }

        [TestMethod]
        public async Task AddLoanAsync_WithValidData_ShouldAddLoan()
        {
            // Arrange
            var loan = new Loan
            {
                UserCnp = "123",
                Amount = 1000,
                Status = "Active"
            };

            // Act
            await _repository.AddLoanAsync(loan);

            // Assert
            Assert.AreEqual(1, await _context.Loans.CountAsync());
            var addedLoan = await _context.Loans.FirstAsync();
            Assert.AreEqual("123", addedLoan.UserCnp);
            Assert.AreEqual(1000, addedLoan.Amount);
            Assert.AreEqual("Active", addedLoan.Status);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task AddLoanAsync_WithNullLoan_ShouldThrowException()
        {
            // Act
            await _repository.AddLoanAsync(null);
        }

        [TestMethod]
        public async Task UpdateLoanAsync_WithValidData_ShouldUpdateLoan()
        {
            // Arrange
            var loan = new Loan
            {
                Id = 1,
                UserCnp = "123",
                Amount = 1000,
                Status = "Active"
            };
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            // Act
            loan.Status = "Completed";
            await _repository.UpdateLoanAsync(loan);

            // Assert
            var updatedLoan = await _context.Loans.FindAsync(1);
            Assert.AreEqual("Completed", updatedLoan.Status);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task UpdateLoanAsync_WithNullLoan_ShouldThrowException()
        {
            // Act
            await _repository.UpdateLoanAsync(null);
        }

        [TestMethod]
        public async Task DeleteLoanAsync_ShouldDeleteLoan()
        {
            // Arrange
            var loan = new Loan
            {
                Id = 1,
                UserCnp = "123",
                Amount = 1000,
                Status = "Active"
            };
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteLoanAsync(1);

            // Assert
            Assert.AreEqual(0, await _context.Loans.CountAsync());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task DeleteLoanAsync_WithInvalidId_ShouldThrowException()
        {
            // Act
            await _repository.DeleteLoanAsync(0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task DeleteLoanAsync_WithNonExistentId_ShouldThrowException()
        {
            // Act
            await _repository.DeleteLoanAsync(999);
        }

        [TestMethod]
        public async Task GetLoanByIdAsync_ShouldReturnCorrectLoan()
        {
            // Arrange
            var loan = new Loan
            {
                Id = 1,
                UserCnp = "123",
                Amount = 1000,
                Status = "Active"
            };
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetLoanByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("123", result.UserCnp);
            Assert.AreEqual(1000, result.Amount);
            Assert.AreEqual("Active", result.Status);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetLoanByIdAsync_WithInvalidId_ShouldThrowException()
        {
            // Act
            await _repository.GetLoanByIdAsync(0);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task GetLoanByIdAsync_WithNonExistentId_ShouldThrowException()
        {
            // Act
            await _repository.GetLoanByIdAsync(999);
        }

        [TestMethod]
        public async Task UpdateCreditScoreHistoryForUserAsync_ShouldUpdateHistory()
        {
            // Arrange
            var userCnp = "123";
            var newScore = 750;

            // Act
            await _repository.UpdateCreditScoreHistoryForUserAsync(userCnp, newScore);

            // Assert
            var history = await _context.CreditScoreHistories.FirstAsync();
            Assert.AreEqual(userCnp, history.UserCnp);
            Assert.AreEqual(newScore, history.Score);
            Assert.AreEqual(DateTime.UtcNow.Date, history.Date.Date);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task UpdateCreditScoreHistoryForUserAsync_WithEmptyCnp_ShouldThrowException()
        {
            // Act
            await _repository.UpdateCreditScoreHistoryForUserAsync("", 750);
        }
    }
} 