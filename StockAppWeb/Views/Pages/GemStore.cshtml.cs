using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockAppWeb.Models;
using Common.Models;

namespace StockAppWeb.Views.Pages
{
    public class GemStoreModel : PageModel
    {
        [BindProperty]
        public GemStoreViewModel ViewModel { get; set; } = new();

        public void OnGet()
        {
            // Load data using the existing business logic (simulate ViewModel initialization)
            ViewModel.Initialize();
        }

        public IActionResult OnPostBuy(string dealTitle)
        {
            ViewModel.ErrorMessage = null;
            ViewModel.SuccessMessage = null;

            var deal = ViewModel.AvailableDeals.FirstOrDefault(d => d.Title == dealTitle);
            if (deal == null)
            {
                ViewModel.ErrorMessage = "Please select a deal before buying.";
                ViewModel.Initialize();
                return Page();
            }

            if (ViewModel.IsGuest)
            {
                ViewModel.ErrorMessage = "Guests are not allowed to buy gems.";
                ViewModel.Initialize();
                return Page();
            }

            if (string.IsNullOrEmpty(ViewModel.SelectedBankAccount))
            {
                ViewModel.ErrorMessage = "No bank account selected.";
                ViewModel.Initialize();
                return Page();
            }

            ViewModel.SuccessMessage = ViewModel.BuyGems(deal, ViewModel.SelectedBankAccount);
            ViewModel.Initialize();
            return Page();
        }

        public IActionResult OnPostSell()
        {
            ViewModel.ErrorMessage = null;
            ViewModel.SuccessMessage = null;

            if (ViewModel.IsGuest)
            {
                ViewModel.ErrorMessage = "Guests are not allowed to sell gems.";
                ViewModel.Initialize();
                return Page();
            }

            if (ViewModel.GemsToSell <= 0)
            {
                ViewModel.ErrorMessage = "Enter a valid number of Gems.";
                ViewModel.Initialize();
                return Page();
            }

            if (ViewModel.GemsToSell > ViewModel.UserGems)
            {
                ViewModel.ErrorMessage = "Not enough Gems to sell.";
                ViewModel.Initialize();
                return Page();
            }

            if (string.IsNullOrEmpty(ViewModel.SelectedBankAccount))
            {
                ViewModel.ErrorMessage = "No bank account selected.";
                ViewModel.Initialize();
                return Page();
            }

            ViewModel.SuccessMessage = ViewModel.SellGems(ViewModel.GemsToSell, ViewModel.SelectedBankAccount);
            ViewModel.Initialize();
            return Page();
        }
    }
}
