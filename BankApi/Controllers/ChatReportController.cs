using BankApi.Repositories;
using Common.Models;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatReportController(IChatReportService chatReportService, IUserRepository userRepository) : ControllerBase
    {
        private readonly IChatReportService _chatReportService = chatReportService ?? throw new ArgumentNullException(nameof(chatReportService));
        private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

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

        [HttpGet]
        [Authorize(Roles = "Admin")] // Only admins can view all chat reports
        public async Task<ActionResult<List<ChatReport>>> GetAllChatReports()
        {
            try
            {
                var reports = await _chatReportService.GetAllChatReportsAsync();
                return Ok(reports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChatReport>> GetChatReportById(int id)
        {
            try
            {
                var report = await _chatReportService.GetChatReportByIdAsync(id);
                if (report == null)
                {
                    return NotFound($"Chat report with ID {id} not found");
                }

                // Ensure user has access to this report (they submitted it or are an admin)
                var currentUserCnp = await GetCurrentUserCnp();
                return report.SubmitterCnp != currentUserCnp && !User.IsInRole("Admin") ? (ActionResult<ChatReport>)Forbid() : (ActionResult<ChatReport>)Ok(report);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ChatReport>> AddChatReport([FromBody] ChatReport report)
        {
            try
            {
                // Set the submitter CNP to the current user
                var currentUserCnp = await GetCurrentUserCnp();
                report.SubmitterCnp = currentUserCnp;

                // Validate that user doesn't report themselves
                if (report.ReportedUserCnp == currentUserCnp)
                {
                    return BadRequest("You cannot report yourself");
                }

                await _chatReportService.AddChatReportAsync(report);
                return CreatedAtAction(nameof(GetChatReportById), new { id = report.Id }, report);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChatReport(int id)
        {
            try
            {
                // Verify the report exists
                var report = await _chatReportService.GetChatReportByIdAsync(id);
                if (report == null)
                {
                    return NotFound($"Chat report with ID {id} not found");
                }

                // Ensure user has permission (they submitted it or are an admin)
                var currentUserCnp = await GetCurrentUserCnp();
                if (report.SubmitterCnp != currentUserCnp && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                await _chatReportService.DeleteChatReportAsync(id);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user-tips/{userCnp}")]
        public async Task<ActionResult<int>> GetNumberOfGivenTipsForUser(string userCnp)
        {
            try
            {
                // Only allow users to view their own tips, or admins to view anyone's
                var currentUserCnp = await GetCurrentUserCnp();
                if (userCnp != currentUserCnp && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var count = await _chatReportService.GetNumberOfGivenTipsForUserAsync(userCnp);
                return Ok(count);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("activity-log")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateActivityLog([FromBody] ActivityLogUpdateDto update)
        {
            try
            {
                var currentUserCnp = await GetCurrentUserCnp();
                await _chatReportService.UpdateActivityLogAsync(update.Amount, currentUserCnp);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("score-history")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateScoreHistoryForUser([FromBody] ScoreHistoryUpdateDto update)
        {
            try
            {
                var currentUserCnp = await GetCurrentUserCnp();
                await _chatReportService.UpdateScoreHistoryForUserAsync(update.NewScore, currentUserCnp);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class ActivityLogUpdateDto
    {
        public int Amount { get; set; }
    }

    public class ScoreHistoryUpdateDto
    {
        public int NewScore { get; set; }
    }
}