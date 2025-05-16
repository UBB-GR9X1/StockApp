using BankApi.Repositories;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryRepository _historyRepository;

        public HistoryController(IHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository ?? throw new ArgumentNullException(nameof(historyRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CreditScoreHistory>>> GetAllHistory()
        {
            try
            {
                var history = await _historyRepository.GetAllHistoryAsync();
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CreditScoreHistory>> GetHistoryById(int id)
        {
            try
            {
                var history = await _historyRepository.GetHistoryByIdAsync(id);
                if (history == null)
                    return NotFound($"History entry with ID {id} not found.");

                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CreditScoreHistory>> AddHistory(CreditScoreHistory history)
        {
            try
            {
                var addedHistory = await _historyRepository.AddHistoryAsync(history);
                return CreatedAtAction(nameof(GetHistoryById), new { id = addedHistory.Id }, addedHistory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CreditScoreHistory>> UpdateHistory(int id, CreditScoreHistory history)
        {
            if (id != history.Id)
                return BadRequest("ID mismatch");

            try
            {
                var updatedHistory = await _historyRepository.UpdateHistoryAsync(history);
                return Ok(updatedHistory);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteHistory(int id)
        {
            try
            {
                await _historyRepository.DeleteHistoryAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user/{userCnp}")]
        public async Task<ActionResult<IEnumerable<CreditScoreHistory>>> GetHistoryForUser(string userCnp)
        {
            try
            {
                var history = await _historyRepository.GetHistoryForUserAsync(userCnp);
                return Ok(history);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user/{userCnp}/weekly")]
        public async Task<ActionResult<IEnumerable<CreditScoreHistory>>> GetHistoryWeekly(string userCnp)
        {
            try
            {
                var history = await _historyRepository.GetHistoryWeeklyAsync(userCnp);
                return Ok(history);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user/{userCnp}/monthly")]
        public async Task<ActionResult<IEnumerable<CreditScoreHistory>>> GetHistoryMonthly(string userCnp)
        {
            try
            {
                var history = await _historyRepository.GetHistoryMonthlyAsync(userCnp);
                return Ok(history);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user/{userCnp}/yearly")]
        public async Task<ActionResult<IEnumerable<CreditScoreHistory>>> GetHistoryYearly(string userCnp)
        {
            try
            {
                var history = await _historyRepository.GetHistoryYearlyAsync(userCnp);
                return Ok(history);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}