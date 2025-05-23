using Common.Models;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StockAppWeb.Pages.Analysis
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IActivityService _activityService;
        private readonly IHistoryService _historyService;

        public IndexModel(
            IUserService userService,
            IActivityService activityService,
            IHistoryService historyService)
        {
            _userService = userService;
            _activityService = activityService;
            _historyService = historyService;
        }

        public User CurrentUser { get; set; } = null!;
        public List<ActivityLog> Activities { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                CurrentUser = await _userService.GetCurrentUserAsync();
                if (CurrentUser == null)
                {
                    ErrorMessage = "Unable to identify user. Please log in again.";
                    return Page();
                }

                Activities = await _activityService.GetActivityForUser(CurrentUser.CNP);
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading user data: {ex.Message}";
                return Page();
            }
        }
    }
} 