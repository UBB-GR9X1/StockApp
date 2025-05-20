using BankApi.Repositories;
using Common.Models;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService, IUserRepository userRepository) : ControllerBase
    {
        private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository)); // For GetCurrentUserCnp

        private async Task<string> GetCurrentUserCnp()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }
            var user = await _userRepository.GetByIdAsync(int.Parse(userId));
            return user == null ? throw new Exception("User not found") : user.CNP;
        }

        [HttpGet("current")]
        [Authorize]
        public async Task<ActionResult<User>> GetCurrentUser()
        {
            try
            {
                var userCnp = await GetCurrentUserCnp();
                var user = await _userService.GetUserByCnpAsync(userCnp);
                return Ok(user);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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

        [HttpGet("{cnp}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUserByCnp(string cnp)
        {
            try
            {
                // Allow admins to get any user, otherwise only the current user can get their own info.
                var currentUserCnp = await GetCurrentUserCnp();
                if (!User.IsInRole("Admin") && currentUserCnp != cnp)
                {
                    return Forbid();
                }

                var user = await _userService.GetUserByCnpAsync(cnp);
                return Ok(user);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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

        [HttpGet]
        [Authorize(Roles = "Admin")] // Only admins can get a list of all users
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost] // Typically, user creation is handled by an Identity system or a dedicated registration endpoint.
                   // This endpoint is provided if direct creation via this controller is intended.
        [AllowAnonymous] // This endpoint should be accessible without authentication
        public async Task<ActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                await _userService.CreateUser(user);
                // Return a 201 Created response, pointing to the GetUserByCnp endpoint.
                return CreatedAtAction(nameof(GetUserByCnp), new { cnp = user.CNP }, user);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // This might include exceptions if the user already exists, depending on repository implementation.
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("current")]
        [Authorize]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UserUpdateDto dto)
        {
            try
            {
                var userCnp = await GetCurrentUserCnp();
                await _userService.UpdateUserAsync(dto.UserName, dto.Image, dto.Description, dto.IsHidden, userCnp);
                return NoContent(); // Or Ok(updatedUser) if the service returns the updated user.
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
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

        [HttpPut("{cnp}")]
        [Authorize(Roles = "Admin")] // Only admins can update other users' profiles directly by CNP
        public async Task<IActionResult> UpdateUserByCnp(string cnp, [FromBody] UserUpdateDto dto)
        {
            try
            {
                await _userService.UpdateUserAsync(dto.UserName, dto.Image, dto.Description, dto.IsHidden, cnp);
                return NoContent();
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

        [HttpPut("{cnp}/admin-status")]
        [Authorize(Roles = "Admin")] // Only admins can change admin status
        public async Task<IActionResult> UpdateUserAdminStatus(string cnp, [FromBody] UpdateAdminStatusDto dto)
        {
            try
            {
                await _userService.UpdateIsAdminAsync(dto.IsAdmin, cnp);
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
    }

    public class UserUpdateDto
    {
        public string UserName { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public bool IsHidden { get; set; }
    }

    public class UpdateAdminStatusDto
    {
        public bool IsAdmin { get; set; }
    }
}
