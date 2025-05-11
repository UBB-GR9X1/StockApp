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
        public async Task<ActionResult<User>> UpdateProfile(string cnp, [FromBody] ApiUpdateUser profile)
        {
            try
            {
                if (cnp != profile.CNP)
                {
                    return BadRequest("CNP mismatch");
                }

                var existingProfile = await _profileRepository.GetProfileByCnpAsync(cnp);

                if (existingProfile == null)
                {
                    return NotFound($"Profile with CNP {cnp} not found.");
                }

                existingProfile.Username = profile.Name;
                existingProfile.Image = profile.ProfilePicture;
                existingProfile.Description = profile.Description;
                existingProfile.IsHidden = profile.IsHidden;
                existingProfile.GemBalance = profile.GemBalance;

                var updatedProfile = await _profileRepository.UpdateProfileAsync(existingProfile);
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
    public class ApiUpdateUser
    {
        public string CNP { get; set; }
        public string Name { get; set; }
        public string ProfilePicture { get; set; }
        public string Description { get; set; }
        public bool IsHidden { get; set; }
        public bool IsAdmin { get; set; }
        public int GemBalance { get; set; }
        public DateTime LastUpdated { get; set; }
    }

}
