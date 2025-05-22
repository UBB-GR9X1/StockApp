using BankApi.Repositories;
using BankApi.Services;
using Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace StockApp.Service.Tests;
[TestClass]
public class StockServiceTests
{
    private Mock<IStockRepository> stockRepoMock;
    private Mock<IHomepageStockRepository> homepageRepoMock;
    private StockService stockService;

    [TestInitialize]
    public void Setup()
    {
        stockRepoMock = new Mock<IStockRepository>();
        homepageRepoMock = new Mock<IHomepageStockRepository>();

        stockService = new StockService(stockRepoMock.Object, homepageRepoMock.Object);
    }

    [TestMethod]
    public async Task CreateStockAsync_ValidStock_ReturnsCreatedStock()
    {
        var stock = new Stock { Id = 1, Name = "TestStock" };
        stockRepoMock.Setup(r => r.CreateAsync(stock)).ReturnsAsync(stock);

        var result = await stockService.CreateStockAsync(stock);

        Assert.AreEqual(stock, result);
        stockRepoMock.Verify(r => r.CreateAsync(stock), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task CreateStockAsync_NullStock_ThrowsArgumentNullException()
    {
        await stockService.CreateStockAsync(null);
    }

    [TestMethod]
    public async Task DeleteStockAsync_ExistingId_ReturnsTrue()
    {
        stockRepoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var result = await stockService.DeleteStockAsync(1);

        Assert.IsTrue(result);
        stockRepoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [TestMethod]
    public async Task GetAllStocksAsync_ReturnsListOfStocks()
    {
        var stocks = new List<Stock> { new Stock { Id = 1 }, new Stock { Id = 2 } };
        stockRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(stocks);

        var result = await stockService.GetAllStocksAsync();

        CollectionAssert.AreEqual(stocks, result.ToList());
    }

    [TestMethod]
    public async Task GetStockByIdAsync_ExistingId_ReturnsStock()
    {
        var stock = new Stock { Id = 1 };
        stockRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(stock);

        var result = await stockService.GetStockByIdAsync(1);

        Assert.AreEqual(stock, result);
    }

    [TestMethod]
    public async Task UpdateStockAsync_ValidInput_ReturnsUpdatedStock()
    {
        var updatedStock = new Stock { Id = 1, Name = "Updated" };
        stockRepoMock.Setup(r => r.UpdateAsync(1, updatedStock)).ReturnsAsync(updatedStock);

        var result = await stockService.UpdateStockAsync(1, updatedStock);

        Assert.AreEqual(updatedStock, result);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task UpdateStockAsync_NullUpdatedStock_ThrowsArgumentNullException()
    {
        await stockService.UpdateStockAsync(1, null);
    }

    [TestMethod]
    public async Task UserStocksAsync_ValidCnp_ReturnsUserStocks()
    {
        var cnp = "1234567890";
        var userStocks = new List<Stock> { new Stock { Id = 1 }, new Stock { Id = 2 } };
        stockRepoMock.Setup(r => r.UserStocksAsync(cnp)).ReturnsAsync(userStocks);

        var result = await stockService.UserStocksAsync(cnp);

        CollectionAssert.AreEqual(userStocks, result);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task UserStocksAsync_EmptyCnp_ThrowsArgumentException()
    {
        await stockService.UserStocksAsync("");
    }

    [TestMethod]
    public async Task GetFilteredAndSortedStocksAsync_FiltersAndSortsCorrectly()
    {
        var userCnp = "user1";
        var stocks = new List<HomepageStock>
        {
            new HomepageStock { Id = 1, IsFavorite = true, Change = 2, StockDetails = new Stock { Name = "AAA", Symbol = "AAA", Price = 10 } },
            new HomepageStock { Id = 2, IsFavorite = false, Change = -1, StockDetails = new Stock { Name = "BBB", Symbol = "BBB", Price = 20 } },
            new HomepageStock { Id = 3, IsFavorite = true, Change = 5, StockDetails = new Stock { Name = "CCC", Symbol = "CCC", Price = 15 } },
        };

        homepageRepoMock.Setup(r => r.GetAllAsync(userCnp)).ReturnsAsync(stocks);

        // Test filter by query "A" and favorites only = true, sorted by name
        var result = await stockService.GetFilteredAndSortedStocksAsync("A", "Sort by Name", true, userCnp);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("AAA", result[0].StockDetails.Name);

        // Test no filter, sort by Price
        result = await stockService.GetFilteredAndSortedStocksAsync("", "Sort by Price", false, userCnp);
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("AAA", result[0].StockDetails.Name);
        Assert.AreEqual("CCC", result[1].StockDetails.Name);
        Assert.AreEqual("BBB", result[2].StockDetails.Name);
    }

    [TestMethod]
    public async Task AddToFavoritesAsync_ValidStock_SetsIsFavoriteTrueAndUpdates()
    {
        var stock = new HomepageStock { Id = 1, IsFavorite = false };
        homepageRepoMock.Setup(r => r.UpdateAsync(stock.Id, stock)).ReturnsAsync(true);

        await stockService.AddToFavoritesAsync(stock);

        Assert.IsTrue(stock.IsFavorite);
        homepageRepoMock.Verify(r => r.UpdateAsync(stock.Id, stock), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(NullReferenceException))]
    public async Task AddToFavoritesAsync_NullStock_ThrowsArgumentNullException()
    {
        await stockService.AddToFavoritesAsync(null);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task AddToFavoritesAsync_UpdateFails_ThrowsInvalidOperationException()
    {
        var stock = new HomepageStock { Id = 1, IsFavorite = false };
        homepageRepoMock.Setup(r => r.UpdateAsync(stock.Id, stock)).ReturnsAsync(false);

        await stockService.AddToFavoritesAsync(stock);
    }

    [TestMethod]
    public async Task RemoveFromFavoritesAsync_ValidStock_SetsIsFavoriteFalseAndUpdates()
    {
        var stock = new HomepageStock { Id = 1, IsFavorite = true };
        homepageRepoMock.Setup(r => r.UpdateAsync(stock.Id, stock)).ReturnsAsync(true);

        await stockService.RemoveFromFavoritesAsync(stock);

        Assert.IsFalse(stock.IsFavorite);
        homepageRepoMock.Verify(r => r.UpdateAsync(stock.Id, stock), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(NullReferenceException))]
    public async Task RemoveFromFavoritesAsync_NullStock_ThrowsArgumentNullException()
    {
        await stockService.RemoveFromFavoritesAsync(null);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task RemoveFromFavoritesAsync_UpdateFails_ThrowsInvalidOperationException()
    {
        var stock = new HomepageStock { Id = 1, IsFavorite = true };
        homepageRepoMock.Setup(r => r.UpdateAsync(stock.Id, stock)).ReturnsAsync(false);

        await stockService.RemoveFromFavoritesAsync(stock);
    }
}
