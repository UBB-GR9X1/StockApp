using Microsoft.AspNetCore.Mvc;
using StockApp.Database;
using StockApp.Models;
using StockApp.Repositories;
using System;
using System.Threading.Tasks;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GemStoreController : ControllerBase
    {
        private readonly IGemStoreRepository _gemStoreRepository;

        public GemStoreController(IGemStoreRepository gemStoreRepository)
        {
            _gemStoreRepository = gemStoreRepository ?? throw new ArgumentNullException(nameof(gemStoreRepository));
        }

        [HttpGet("cnp")]
        public async Task<ActionResult<string>> GetCnp()
        {
            try
            {
                var cnp = await _gemStoreRepository.GetCnpAsync();
                return Ok(cnp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("balance/{cnp}")]
        public async Task<ActionResult<int>> GetUserGemBalance(string cnp)
        {
            try
            {
                var balance = await _gemStoreRepository.GetUserGemBalanceAsync(cnp);
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("balance/{cnp}")]
        public async Task<ActionResult> UpdateUserGemBalance(string cnp, [FromBody] int newBalance)
        {
            try
            {
                await _gemStoreRepository.UpdateUserGemBalanceAsync(cnp, newBalance);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("isguest/{cnp}")]
        public async Task<ActionResult<bool>> IsGuest(string cnp)
        {
            try
            {
                var isGuest = await _gemStoreRepository.IsGuestAsync(cnp);
                return Ok(isGuest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
} 