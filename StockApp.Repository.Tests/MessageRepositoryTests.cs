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
    public class MessageRepositoryTests
    {
        private ApiDbContext _context;
        private IMessagesRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApiDbContext(options);
            _repository = new MessagesRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetMessagesForUserAsync_ShouldReturnUserMessages()
        {
            // Arrange
            var user = new User { CNP = "123", Username = "testuser" };
            await _context.Users.AddAsync(user);

            var tips = new List<Tip>
            {
                new Tip { Id = 1, Type = "Punishment", TipText = "Test message 1" },
                new Tip { Id = 2, Type = "Roast", TipText = "Test message 2" }
            };
            await _context.Tips.AddRangeAsync(tips);

            var givenTips = new List<GivenTip>
            {
                new GivenTip { User = user, Tip = tips[0], Date = DateTime.UtcNow },
                new GivenTip { User = user, Tip = tips[1], Date = DateTime.UtcNow }
            };
            await _context.GivenTips.AddRangeAsync(givenTips);

            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetMessagesForUserAsync("123");

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(m => m.MessageContent == "Test message 1"));
            Assert.IsTrue(result.Any(m => m.MessageContent == "Test message 2"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetMessagesForUserAsync_WithEmptyCnp_ShouldThrowException()
        {
            // Act
            await _repository.GetMessagesForUserAsync("");
        }

        [TestMethod]
        public async Task GiveUserRandomMessageAsync_ShouldAddMessage()
        {
            // Arrange
            var user = new User { CNP = "123", Username = "testuser" };
            await _context.Users.AddAsync(user);

            var tip = new Tip { Id = 1, Type = "Punishment", TipText = "Test message" };
            await _context.Tips.AddAsync(tip);

            await _context.SaveChangesAsync();

            // Act
            await _repository.GiveUserRandomMessageAsync("123");

            // Assert
            var givenTip = await _context.GivenTips
                .Include(gt => gt.User)
                .Include(gt => gt.Tip)
                .FirstOrDefaultAsync();

            Assert.IsNotNull(givenTip);
            Assert.AreEqual("123", givenTip.User.CNP);
            Assert.AreEqual("Test message", givenTip.Tip.TipText);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GiveUserRandomMessageAsync_WithEmptyCnp_ShouldThrowException()
        {
            // Act
            await _repository.GiveUserRandomMessageAsync("");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task GiveUserRandomMessageAsync_WithNoMessages_ShouldThrowException()
        {
            // Arrange
            var user = new User { CNP = "123", Username = "testuser" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            await _repository.GiveUserRandomMessageAsync("123");
        }

        [TestMethod]
        public async Task GiveUserRandomRoastMessageAsync_ShouldAddRoastMessage()
        {
            // Arrange
            var user = new User { CNP = "123", Username = "testuser" };
            await _context.Users.AddAsync(user);

            var tip = new Tip { Id = 1, Type = "Roast", TipText = "Test roast" };
            await _context.Tips.AddAsync(tip);

            await _context.SaveChangesAsync();

            // Act
            await _repository.GiveUserRandomRoastMessageAsync("123");

            // Assert
            var givenTip = await _context.GivenTips
                .Include(gt => gt.User)
                .Include(gt => gt.Tip)
                .FirstOrDefaultAsync();

            Assert.IsNotNull(givenTip);
            Assert.AreEqual("123", givenTip.User.CNP);
            Assert.AreEqual("Test roast", givenTip.Tip.TipText);
            Assert.AreEqual("Roast", givenTip.Tip.Type);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GiveUserRandomRoastMessageAsync_WithEmptyCnp_ShouldThrowException()
        {
            // Act
            await _repository.GiveUserRandomRoastMessageAsync("");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task GiveUserRandomRoastMessageAsync_WithNoRoastMessages_ShouldThrowException()
        {
            // Arrange
            var user = new User { CNP = "123", Username = "testuser" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            await _repository.GiveUserRandomRoastMessageAsync("123");
        }
    }
} 