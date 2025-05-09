using BankApi.Models;
using BankApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileRepository _profileRepository;

        public ProfileController(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));
        }

        [HttpGet("{cnp}")]
        public async Task<ActionResult<User>> GetProfile(string cnp)
        {
            try
            {
                var profile = await _profileRepository.GetProfileByCnpAsync(cnp);
                if (profile == null)
                {
                    return NotFound($"Profile with CNP {cnp} not found.");
                }
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateProfile([FromBody] User profile)
        {
            try
            {
                var createdProfile = await _profileRepository.CreateProfileAsync(profile);
                return CreatedAtAction(nameof(GetProfile), new { cnp = createdProfile.CNP }, createdProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{cnp}")]
        public async Task<ActionResult<User>> UpdateProfile(string cnp, [FromBody] User profile)
        {
            try
            {
                if (cnp != profile.CNP)
                {
                    return BadRequest("CNP mismatch");
                }

                var updatedProfile = await _profileRepository.UpdateProfileAsync(profile);
                return Ok(updatedProfile);
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

        [HttpPut("{cnp}/admin")]
        public async Task<ActionResult> UpdateAdminStatus(string cnp, [FromBody] bool isAdmin)
        {
            try
            {
                var success = await _profileRepository.UpdateAdminStatusAsync(cnp, isAdmin);
                if (!success)
                {
                    return NotFound($"Profile with CNP {cnp} not found.");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{cnp}/stocks")]
        public async Task<ActionResult<List<Stock>>> GetUserStocks(string cnp)
        {
            try
            {
                var stocks = await _profileRepository.GetUserStocksAsync(cnp);
                return Ok(stocks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("username/random")]
        public ActionResult<string> GetRandomUsername()
        {
            try
            {
                var username = _profileRepository.GenerateRandomUsername();
                return Ok(username);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}