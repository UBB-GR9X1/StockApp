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

        [HttpPost("UserStock")]
        public async Task<IActionResult> AddOrUpdateUserStockAsync([FromBody] UserStockRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.userCNP) || string.IsNullOrWhiteSpace(request.stockName) || request.quantity < 0)
            {
                return BadRequest("Invalid user stock request.");
            }
            try
            {
                await _repository.AddOrUpdateUserStockAsync(request.userCNP, request.stockName, request.quantity);
                return Ok("Stock updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user stock.");
                return StatusCode(500, "Internal server error.");
            }
        }

        public class UserStockRequest
        {
            public string userCNP { get; set; }
            public string stockName { get; set; }
            public int quantity { get; set; }
        }

        [HttpPost("StockValue")]
        public async Task<IActionResult> AddStockValueAsync([FromBody] StockValueRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.StockName) || request.Price <= 0)
            {
                return BadRequest("Invalid stock value request.");
            }

            try
            {
                await _repository.AddStockValueAsync(request.StockName, request.Price);
                return Ok("Stock value added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding stock value.");
                return StatusCode(500, "Internal server error.");
            }
        }

        public class StockValueRequest
        {
            public string StockName { get; set; }
            public int Price { get; set; }
        }

        [HttpGet("Favorite")]
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

        [HttpGet("OwnedStocks")]
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

        [HttpGet("Stock")]
        public async Task<IActionResult> GetStockAsync(string stockName)
        {
            try
            {
                var stock = await _repository.GetStockAsync(stockName);
                return Ok(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stock.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("UserStock")]
        public async Task<IActionResult> GetUserStockAsync(string userCNP, string stockName)
        {
            try
            {
                var stock = await _repository.GetUserStockAsync(userCNP, stockName);
                return Ok(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user stock.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("StockHistory")]
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
        public async Task<IActionResult> ToggleFavoriteAsync([FromBody] ToggleFavoriteRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.userCNP) || string.IsNullOrWhiteSpace(request.stockName))
            {
                return BadRequest("Invalid toggle favorite request.");
            }
            try
            {
                await _repository.ToggleFavoriteAsync(request.userCNP, request.stockName, request.state);
                return Ok("Favorite status updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling favorite status.");
                return StatusCode(500, "Internal server error.");
            }
        }
        public class ToggleFavoriteRequest
        {
            public string userCNP { get; set; }
            public string stockName { get; set; }
            public bool state { get; set; }
        }
    }
}