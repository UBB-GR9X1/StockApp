using BankApi.Models;
using BankApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseStocksController : ControllerBase
    {
        private readonly IBaseStockRepository _repository;
        private readonly ILogger<BaseStocksController> _logger;

        public BaseStocksController(IBaseStockRepository repository, ILogger<BaseStocksController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/BaseStocks
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BaseStock>>> GetAllStocks()
        {
            try
            {
                var stocks = await _repository.GetAllStocksAsync();
                return Ok(stocks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all stocks");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        // GET: api/BaseStocks/{name}
        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseStock>> GetStockByName(string name)
        {
            try
            {
                var stock = await _repository.GetStockByNameAsync(name);
                return Ok(stock);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Stock not found: {StockName}", name);
                return NotFound($"Stock with name '{name}' not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stock by name: {StockName}", name);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
            }
        }

        // POST: api/BaseStocks
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseStock>> CreateStock([FromBody] BaseStock stock)
        {
            try
            {
                if (stock == null)
                {
                    return BadRequest("Stock data is null");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdStock = await _repository.AddStockAsync(stock);

                return CreatedAtAction(nameof(GetStockByName),
                    new { name = createdStock.Name },
                    createdStock);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                _logger.LogWarning(ex, "Duplicate stock: {StockName}", stock.Name);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating stock: {StockName}", stock?.Name);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating stock");
            }
        }

        // PUT: api/BaseStocks/{name}
        [HttpPut("{name}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateStock(string name, [FromBody] BaseStock stock)
        {
            try
            {
                if (stock == null)
                {
                    return BadRequest("Stock data is null");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (name != stock.Name)
                {
                    return BadRequest("Name in URL does not match the stock name in the body");
                }

                await _repository.UpdateStockAsync(stock);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Stock not found during update: {StockName}", name);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock: {StockName}", name);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating stock");
            }
        }

        // DELETE: api/BaseStocks/{name}
        [HttpDelete("{name}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteStock(string name)
        {
            try
            {
                var success = await _repository.DeleteStockAsync(name);

                if (!success)
                {
                    return NotFound($"Stock with name '{name}' not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting stock: {StockName}", name);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting stock");
            }
        }
    }
}