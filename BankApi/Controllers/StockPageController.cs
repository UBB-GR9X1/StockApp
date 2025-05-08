using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankApi.Models;
using BankApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StockPageController : ControllerBase
    {
        private readonly IStockPageRepository _repository;

        public StockPageController(IStockPageRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("user")]
        public async Task<ActionResult<User>> GetUser()
        {
            try
            {
                var user = await _repository.GetUserAsync();
                if (user == null)
                {
                    return NotFound("User not found");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("gems")]
        public async Task<IActionResult> UpdateGems([FromBody] int gems)
        {
            try
            {
                await _repository.UpdateUserGemsAsync(gems);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("stocks/{stockName}/quantity")]
        public async Task<IActionResult> UpdateStockQuantity(string stockName, [FromBody] int quantity)
        {
            try
            {
                await _repository.AddOrUpdateUserStockAsync(stockName, quantity);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("stocks/{stockName}/value")]
        public async Task<IActionResult> AddStockValue(string stockName, [FromBody] int price)
        {
            try
            {
                await _repository.AddStockValueAsync(stockName, price);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("stocks/{stockName}")]
        public async Task<ActionResult<Stock>> GetStock(string stockName)
        {
            try
            {
                var stock = await _repository.GetStockAsync(stockName);
                return Ok(stock);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("stocks/{stockName}/history")]
        public async Task<ActionResult<List<int>>> GetStockHistory(string stockName)
        {
            try
            {
                var history = await _repository.GetStockHistoryAsync(stockName);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("stocks/{stockName}/owned")]
        public async Task<ActionResult<int>> GetOwnedStocks(string stockName)
        {
            try
            {
                var quantity = await _repository.GetOwnedStocksAsync(stockName);
                return Ok(quantity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("stocks/{stockName}/favorite")]
        public async Task<ActionResult<bool>> GetFavorite(string stockName)
        {
            try
            {
                var isFavorite = await _repository.GetFavoriteAsync(stockName);
                return Ok(isFavorite);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("stocks/{stockName}/favorite")]
        public async Task<IActionResult> ToggleFavorite(string stockName, [FromBody] bool state)
        {
            try
            {
                await _repository.ToggleFavoriteAsync(stockName, state);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
} 