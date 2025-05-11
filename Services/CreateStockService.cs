namespace StockApp.Services
{
    using System;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using StockApp.Exceptions;
    using StockApp.Models;
    using StockApp.Repositories;

    /// <summary>
    /// Service for creating stocks
    /// </summary>
    internal class CreateStockService : ICreateStockService
    {
        private readonly IBaseStocksService _stocksService;
        private readonly IUserRepository _userRepository;
        private readonly Random random = new();

        public CreateStockService(IBaseStocksService stocksService,
                                  IUserRepository userRepository)
        {
            _stocksService = stocksService ?? throw new ArgumentNullException(nameof(stocksService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>Parameter-less ctor used by DI fallback.</summary>
        public CreateStockService()
        {
            try
            {
                _stocksService = App.Host.Services.GetService<IBaseStocksService>()
                               ?? throw new InvalidOperationException("Could not resolve IBaseStocksService.");
                _userRepository = App.Host.Services.GetService<IUserRepository>()
                               ?? throw new InvalidOperationException("Could not resolve IUserRepository.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing CreateStockService: {ex.Message}");
                throw;
            }
        }

        /*────────────────────────── Helpers ─────────────────────────*/

        public bool CheckIfUserIsGuest()
        {
            var cnp = GetUserCnp();
            return string.IsNullOrEmpty(cnp) || cnp == "0000000000000";
        }

        private string GetUserCnp() => IUserRepository.CurrentUserCNP;

        /*──────────────────────  Public API  ──────────────────────*/

        public async Task<string> AddStockAsync(string stockName,
                                           string stockSymbol,
                                           string authorCNP)
        {
            if (string.IsNullOrWhiteSpace(stockName) ||
                string.IsNullOrWhiteSpace(stockSymbol) ||
                string.IsNullOrWhiteSpace(authorCNP))
            {
                throw new ArgumentException("All stock fields (name, symbol, author CNP) are required.");
            }

            if (!Regex.IsMatch(stockSymbol, @"^[A-Z]{1,5}$"))
            {
                throw new ArgumentException("Stock symbol must consist of 1 to 5 uppercase letters.");
            }

            if (!Regex.IsMatch(authorCNP, @"^\d{13}$"))
            {
                throw new ArgumentException("Author CNP must be exactly 13 digits.");
            }

            try
            {
                int initialPrice = random.Next(50, 501);
                var stock = new BaseStock(stockName, stockSymbol, authorCNP);

                /* FIX: AddStockAsync now returns Task, not bool */
                await _stocksService.AddStockAsync(stock, initialPrice);

                return "Stock added successfully with initial value!";
            }
            catch (DuplicateStockException)
            {
                throw;
            }
            catch (InvalidOperationException opEx)
            {
                throw new StockPersistenceException("Failed to add stock due to a persistence operation error.", opEx);
            }
        }

        public async Task<(bool success, string message)> CreateStockAsync(string stockName,
                                                                          string stockSymbol,
                                                                          string authorCnp)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(stockName))
                {
                    return (false, "Stock name cannot be empty.");
                }

                if (string.IsNullOrWhiteSpace(stockSymbol))
                {
                    return (false, "Stock symbol cannot be empty.");
                }

                if (string.IsNullOrWhiteSpace(authorCnp))
                {
                    authorCnp = GetUserCnp();
                }

                if (stockName.Length > 100)
                {
                    return (false, "Stock name cannot exceed 100 characters.");
                }

                if (stockSymbol.Length > 10)
                {
                    return (false, "Stock symbol cannot exceed 10 characters.");
                }

                var stock = new BaseStock(stockName, stockSymbol, authorCnp);

                /* FIX: AddStockAsync now returns Task, assume success if no exception */
                await _stocksService.AddStockAsync(stock);

                return (true, "Stock created successfully!");
            }
            catch (DuplicateStockException)
            {
                return (false, $"A stock with the name '{stockName}' already exists.");
            }
            catch (Exception ex)
            {
                return (false, $"Error creating stock: {ex.Message}");
            }
        }
    }
}
