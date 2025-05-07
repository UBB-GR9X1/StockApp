namespace StockApp.Services
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.DependencyInjection;
    using StockApp.Database;
    using StockApp.Exceptions;
    using StockApp.Models;
    using StockApp.Repositories;
    using System.Diagnostics;

    /// <summary>
    /// Service for creating stocks
    /// </summary>
    internal class CreateStockService : ICreateStockService
    {
        private readonly IBaseStocksApiService _apiService;
        private readonly IUserRepository _userRepository;
        private readonly Random random = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateStockService"/> class.
        /// </summary>
        public CreateStockService(IBaseStocksApiService apiService, IUserRepository userRepository)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateStockService"/> class.
        /// </summary>
        public CreateStockService()
        {
            try
            {
                _apiService = App.Host.Services.GetService<IBaseStocksApiService>();
                _userRepository = App.Host.Services.GetService<IUserRepository>();
                
                if (_apiService == null)
                {
                    throw new InvalidOperationException("Could not resolve IBaseStocksApiService from the service provider");
                }
                
                if (_userRepository == null)
                {
                    throw new InvalidOperationException("Could not resolve IUserRepository from the service provider");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing CreateStockService: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Checks if the user is a guest.
        /// </summary>
        /// <returns></returns>
        public bool CheckIfUserIsGuest()
        {
            // If CNP is empty or has the default guest value, the user is a guest
            var cnp = GetUserCnp();
            return string.IsNullOrEmpty(cnp) || cnp == "0000000000000";
        }

        /// <summary>
        /// Gets the current user's CNP
        /// </summary>
        private string GetUserCnp()
        {
            return _userRepository.CurrentUserCNP;
        }

        /// <summary>
        /// Adds a new stock to the repository.
        /// </summary>
        /// <param name="stockName"></param>
        /// <param name="stockSymbol"></param>
        /// <param name="authorCNP"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="StockPersistenceException"></exception>
        public async Task<string> AddStock(string stockName, string stockSymbol, string authorCNP)
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
                int initialPrice = this.random.Next(50, 501);
                var stock = new BaseStock(stockName, stockSymbol, authorCNP);
                bool result = await _apiService.AddStockAsync(stock, initialPrice);
                
                if (result)
                {
                    return "Stock added successfully with initial value!";
                }
                else
                {
                    return "Failed to add stock.";
                }
            }
            catch (DuplicateStockException)
            {
                throw;
            }
            catch (InvalidOperationException operationFailure)
            {
                throw new StockPersistenceException("Failed to add stock due to a persistence operation error.", operationFailure);
            }
            catch (SqlException sqlIssue)
            {
                throw new StockPersistenceException("Database error occurred while adding the stock.", sqlIssue);
            }
        }

        public async Task<(bool success, string message)> CreateStockAsync(string stockName, string stockSymbol, string authorCnp)
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
                    // If no CNP provided, use current user's CNP
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
                bool success = await _apiService.AddStockAsync(stock);

                return success 
                    ? (true, "Stock created successfully!") 
                    : (false, "Failed to create stock.");
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