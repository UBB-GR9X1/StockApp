namespace BankApi.Controllers
{
    using System;
    using BankApi.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    /// <summary>
    /// Controller for managing stock-related operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StockPageController : ControllerBase
    {
        private readonly IStockPageRepository _repository;
        private readonly ILogger<StockPageController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockPageController"/> class.
        /// </summary>
        /// <param name="repository">The stock repository.</param>
        /// <param name="logger">The logger.</param>
        public StockPageController(IStockPageRepository repository, ILogger<StockPageController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("AddOrUpdateUserStock")]
        public async Task<IActionResult> AddOrUpdateUserStockAsync(string userCNP, string stockName, int quantity)
        {
            try
            {
                await _repository.AddOrUpdateUserStockAsync(userCNP, stockName, quantity);
                return Ok("Stock updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user stock.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost("AddStockValue")]
        public async Task<IActionResult> AddStockValueAsync(string stockName, int price)
        {
            try
            {
                await _repository.AddStockValueAsync(stockName, price);
                return Ok("Stock value added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding stock value.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("GetFavorite")]
        public async Task<IActionResult> GetFavoriteAsync(string userCNP, string stockName)
        {
            try
            {
                var isFavorite = await _repository.GetFavoriteAsync(userCNP, stockName);
                return Ok(isFavorite);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving favorite status.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("GetOwnedStocks")]
        public async Task<IActionResult> GetOwnedStocksAsync(string userCNP, string stockName)
        {
            try
            {
                var quantity = await _repository.GetOwnedStocksAsync(userCNP, stockName);
                return Ok(quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving owned stocks.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("GetStock")]
        public async Task<IActionResult> GetStockAsync(string userCNP, string stockName)
        {
            try
            {
                var stock = await _repository.GetStockAsync(userCNP, stockName);
                return Ok(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stock.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("GetStockHistory")]
        public async Task<IActionResult> GetStockHistoryAsync(string stockName)
        {
            try
            {
                var history = await _repository.GetStockHistoryAsync(stockName);
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stock history.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost("ToggleFavorite")]
        public async Task<IActionResult> ToggleFavoriteAsync(string userCNP, string stockName, bool state)
        {
            try
            {
                await _repository.ToggleFavoriteAsync(userCNP, stockName, state);
                return Ok("Favorite status updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling favorite status.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost("UpdateUserGems")]
        public async Task<IActionResult> UpdateUserGemsAsync(string userCNP, int newGemBalance)
        {
            try
            {
                await _repository.UpdateUserGemsAsync(userCNP, newGemBalance);
                return Ok("User gems updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user gems.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}