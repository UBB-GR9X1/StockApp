using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockApp.Repositories;

namespace StockApp.Controllers
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

        [HttpPut("balance")]
        public async Task<IActionResult> UpdateUserGemBalance([FromBody] UpdateBalanceRequest request)
        {
            try
            {
                await _gemStoreRepository.UpdateUserGemBalanceAsync(request.Cnp, request.NewBalance);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("guest/{cnp}")]
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

    public class UpdateBalanceRequest
    {
        public string Cnp { get; set; } = string.Empty;
        public int NewBalance { get; set; }
    }
} 