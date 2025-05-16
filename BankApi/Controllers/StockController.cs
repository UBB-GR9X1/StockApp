using BankApi.Repositories;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockRepository _stockRepository;
        public StockController(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }
        // Create a new stock
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Stock stock)
        {
            if (stock == null)
            {
                return BadRequest("Stock cannot be null.");
            }
            var createdStock = await _stockRepository.CreateAsync(stock);
            return CreatedAtAction(nameof(GetById), new { id = createdStock.Id }, createdStock);
        }
        // Retrieve a stock by its ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var stock = await _stockRepository.GetByIdAsync(id);
            if (stock == null)
            {
                return NotFound();
            }
            return Ok(stock);
        }
        // Retrieve all stocks
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var stocks = await _stockRepository.GetAllAsync();
            return Ok(stocks);
        }
        // Update an existing stock
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Stock updatedStock)
        {
            if (updatedStock == null)
            {
                return BadRequest("Updated stock cannot be null.");
            }
            var stock = await _stockRepository.UpdateAsync(id, updatedStock);
            if (stock == null)
            {
                return NotFound();
            }
            return Ok(stock);
        }
        // Delete a stock by its ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _stockRepository.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        // Retrieve stocks for a specific user
        [HttpGet("user/{cnp}")]
        public async Task<IActionResult> GetUserStocks(string cnp)
        {
            if (string.IsNullOrEmpty(cnp))
            {
                return BadRequest("CNP cannot be null or empty.");
            }
            var stocks = await _stockRepository.UserStocksAsync(cnp);
            return Ok(stocks);
        }
    }
}
