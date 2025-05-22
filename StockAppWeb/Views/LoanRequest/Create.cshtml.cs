using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Common.Models;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace StockAppWeb.Views.LoanRequest
{
    using Common.Models;

    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ILoanService _loanService;

        public CreateModel(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Range(100, 100000)]
            public decimal Amount { get; set; }

            [Required]
            [DataType(DataType.Date)]
            public DateTime RepaymentDate { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Simulate getting UserCNP from session/identity
                string userCnp = User.Identity?.Name ?? "1234567890123"; // Replace with real user ID logic

                var request = new LoanRequest
                {
                    UserCnp = userCnp,
                    Amount = Input.Amount,
                    RepaymentDate = Input.RepaymentDate,
                    Status = "Pending"
                };

                await _loanService.AddLoanAsync(request);
                SuccessMessage = "Loan request submitted successfully.";
                ModelState.Clear();
                Input = new();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to submit request: {ex.Message}";
            }

            return Page();
        }
    }
}
