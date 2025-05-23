using Common.Models;
using Common.Services;
using Microsoft.AspNetCore.Mvc;
using StockAppWeb.Models;

namespace StockAppWeb.Controllers
{
    public class HomepageController : Controller
    {
        private readonly IStockService _stockService;
        private readonly IAuthenticationService _authenticationService;

        public HomepageController(IStockService stockService, IAuthenticationService authenticationService)
        {
            _stockService = stockService;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchQuery = "", string? selectedSortOption = "")
        {
            var viewModel = new HomepageViewModel(_stockService, _authenticationService)
            {
                SearchQuery = searchQuery ?? string.Empty,
                SelectedSortOption = selectedSortOption ?? string.Empty
            };

            await viewModel.LoadStocksAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return RedirectToAction("Index");

            var allStocks = await _stockService.GetFilteredAndSortedStocksAsync("", "", false);
            var favoriteStocks = await _stockService.GetFilteredAndSortedStocksAsync("", "", true);

            var stock = allStocks.FirstOrDefault(s => s.StockDetails.Symbol == symbol)
                     ?? favoriteStocks.FirstOrDefault(s => s.StockDetails.Symbol == symbol);

            if (stock == null)
                return RedirectToAction("Index");

            if (stock.IsFavorite)
                await _stockService.RemoveFromFavoritesAsync(stock);
            else
                await _stockService.AddToFavoritesAsync(stock);

            return RedirectToAction("Index");
        }
    }
}
