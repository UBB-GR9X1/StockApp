namespace BankApi.Controllers
{
    using BankApi.Models;
    using BankApi.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/[controller]")]
    public class HomepageStocksController : ControllerBase
    {
        private readonly IHomepageStocksRepository _repository;

        public HomepageStocksController(IHomepageStocksRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<HomepageStock>>> GetAllStocks()
        {
            var stocks = await _repository.GetAllStocksAsync();
            return Ok(stocks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HomepageStock>> GetStockById(int id)
        {
            var stock = await _repository.GetStockByIdAsync(id);
            if (stock == null) return NotFound();
            return Ok(stock);
        }

        [HttpPost]
        public async Task<ActionResult> AddStock(HomepageStock stock)
        {
            await _repository.AddStockAsync(stock);
            return CreatedAtAction(nameof(GetStockById), new { id = stock.Id }, stock);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateStock(int id, HomepageStock updatedStock)
        {
            if (id != updatedStock.Id) return BadRequest();

            await _repository.UpdateStockAsync(updatedStock);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStock(int id)
        {
            await _repository.DeleteStockAsync(id);
            return NoContent();
        }
    }
}
