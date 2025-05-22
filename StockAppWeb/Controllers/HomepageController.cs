using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Common.Models;
using StockAppWeb.Models;
using StockAppWeb.Services; // Adjust if your service is elsewhere
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockAppWeb.Controllers
{
    public class HomepageController : Controller
    {
        private readonly StockDbContext _context;
        private readonly StockProxyService _stockService;

        public HomepageController(StockDbContext context, StockProxyService stockService)
        {
            _context = context;
            _stockService = stockService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchQuery, string? selectedSortOption)
        {
            // Step 1: Fetch all stocks from proxy service
            var allStocks = await _stockService.GetAllStocksAsync(); // returns List<Stock>

            // Step 2: Filter by search query
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                allStocks = allStocks.Where(s =>
                    s.Symbol.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                    s.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Step 3: Sort by option
            allStocks = selectedSortOption switch
            {
                "Name" => allStocks.OrderBy(s => s.Name).ToList(),
                "Price" => allStocks.OrderByDescending(s => s.Price).ToList(),
                "Change" => allStocks.OrderByDescending(s => s.Change).ToList(),
                _ => allStocks
            };

            // Step 4: Load favorites from DB
            var favoriteSymbols = await _context.Favorites
                .Select(f => f.StockSymbol)
                .ToListAsync();

            // Step 5: Convert to HomepageStock
            var homepageStocks = allStocks.Select(s => new HomepageStock
            {
                StockDetails = s,
                Change = s.Change,
                IsFavorite = favoriteSymbols.Contains(s.Symbol)
            }).ToList();

            // Step 6: Build view model
            var viewModel = new HomepageViewModel
            {
                SearchQuery = searchQuery,
                SelectedSortOption = selectedSortOption,
                FilteredStocks = homepageStocks,
                FilteredFavoriteStocks = homepageStocks.Where(s => s.IsFavorite).ToList() 
            };

            return View("Homepage", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(string symbol)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.StockSymbol == symbol);

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
            }
            else
            {
                _context.Favorites.Add(new Favorite { StockSymbol = symbol });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
