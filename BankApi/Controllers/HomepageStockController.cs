using BankApi.Models;
using BankApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomepageStockController : ControllerBase
    {
        private readonly IHomepageStockRepository _repository;
        private readonly ILogger<HomepageStockController> _logger;

        public HomepageStockController(IHomepageStockRepository repository, ILogger<HomepageStockController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<List<HomepageStock>>> GetAllHomepageStocks(string userCNP)
        {
            try
            {
                var stocks = await _repository.GetAllAsync(userCNP);
                return Ok(stocks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all homepage stocks");
                return StatusCode(500, "An error occurred while retrieving homepage stocks");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HomepageStock>> GetHomepageStockById(int id, string userCNP)
        {
            try
            {
                var stock = await _repository.GetByIdAsync(id, userCNP);
                if (stock == null)
                {
                    return NotFound($"Homepage stock with ID {id} not found");
                }
                return Ok(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving homepage stock with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the homepage stock");
            }
        }

        [HttpGet("symbol/{symbol}")]
        public async Task<ActionResult<HomepageStock>> GetHomepageStockBySymbol(string symbol)
        {
            try
            {
                var stock = await _repository.GetBySymbolAsync(symbol);
                if (stock == null)
                {
                    return NotFound($"Homepage stock with symbol {symbol} not found");
                }
                return Ok(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving homepage stock with symbol {Symbol}", symbol);
                return StatusCode(500, "An error occurred while retrieving the homepage stock");
            }
        }

        [HttpPost]
        public async Task<ActionResult<HomepageStock>> CreateHomepageStock([FromBody] HomepageStock stock)
        {
            try
            {
                var createdStock = await _repository.CreateAsync(stock);
                return CreatedAtAction(nameof(GetHomepageStockById), new { id = createdStock.Id }, createdStock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating homepage stock");
                return StatusCode(500, "An error occurred while creating the homepage stock");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHomepageStock(int id, [FromBody] HomepageStock updatedStock)
        {
            try
            {
                var result = await _repository.UpdateAsync(id, updatedStock);
                if (!result)
                {
                    return NotFound($"Homepage stock with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating homepage stock with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the homepage stock");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHomepageStock(int id)
        {
            try
            {
                var result = await _repository.DeleteAsync(id);
                if (!result)
                {
                    return NotFound($"Homepage stock with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting homepage stock with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the homepage stock");
            }
        }
    }
}
