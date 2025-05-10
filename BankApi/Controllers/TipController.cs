using BankApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using BankApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipController : ControllerBase
    {
        private readonly ITipsRepository _tipsRepository;

        public TipController(ITipsRepository tipsRepository)
        {
            _tipsRepository = tipsRepository;
        }

        /// <summary>
        /// Retrieves all tips given to a specific user.
        /// </summary>
        /// <param name="userCnp">The unique user identifier.</param>
        /// <returns>A list of tips given to the user.</returns>
        [HttpGet("{userCnp}")]
        public async Task<IActionResult> GetTipsForUser(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
                return BadRequest("User CNP is required.");

            try
            {
                var tips = await _tipsRepository.GetTipsForUserAsync(userCnp);
                if (tips == null || tips.Count == 0)
                    return NotFound("No tips found for the given user.");

                return Ok(tips);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Assigns a low-credit tip to the user.
        /// </summary>
        /// <param name="userCnp">The unique user identifier.</param>
        /// <returns>The assigned tip.</returns>
        [HttpPost("low/{userCnp}")]
        public async Task<IActionResult> GiveLowBracketTip(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
                return BadRequest("User CNP is required.");

            try
            {
                var givenTip = await _tipsRepository.GiveLowBracketTipAsync(userCnp);
                return CreatedAtAction(nameof(GetTipsForUser), new { userCnp = userCnp }, givenTip);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Assigns a medium-credit tip to the user.
        /// </summary>
        /// <param name="userCnp">The unique user identifier.</param>
        /// <returns>The assigned tip.</returns>
        [HttpPost("medium/{userCnp}")]
        public async Task<IActionResult> GiveMediumBracketTip(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
                return BadRequest("User CNP is required.");

            try
            {
                var givenTip = await _tipsRepository.GiveMediumBracketTipAsync(userCnp);
                return CreatedAtAction(nameof(GetTipsForUser), new { userCnp = userCnp }, givenTip);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Assigns a high-credit tip to the user.
        /// </summary>
        /// <param name="userCnp">The unique user identifier.</param>
        /// <returns>The assigned tip.</returns>
        [HttpPost("high/{userCnp}")]
        public async Task<IActionResult> GiveHighBracketTip(string userCnp)
        {
            if (string.IsNullOrWhiteSpace(userCnp))
                return BadRequest("User CNP is required.");

            try
            {
                var givenTip = await _tipsRepository.GiveHighBracketTipAsync(userCnp);
                return CreatedAtAction(nameof(GetTipsForUser), new { userCnp = userCnp }, givenTip);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
