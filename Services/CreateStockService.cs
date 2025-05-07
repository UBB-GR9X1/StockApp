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

    /// <summary>
    /// Service for creating stocks
    /// </summary>
    internal class CreateStockService : ICreateStockService
    {
        private readonly IBaseStocksRepository _stocksRepository;
        private readonly Random random = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateStockService"/> class.
        /// </summary>
        public CreateStockService(IBaseStocksRepository stocksRepository)
        {
            _stocksRepository = stocksRepository ?? throw new ArgumentNullException(nameof(stocksRepository));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateStockService"/> class with a database context.
        /// </summary>
        public CreateStockService(AppDbContext dbContext)
        {
            if (App.Host != null)
            {
                _stocksRepository = App.Host.Services.GetService<IBaseStocksRepository>();
                if (_stocksRepository == null)
                {
                    throw new InvalidOperationException("Could not resolve IBaseStocksRepository from the service provider");
                }
            }
            else
            {
                throw new InvalidOperationException("App.Host is null, cannot resolve dependencies");
            }
        }

        /// <summary>
        /// Checks if the user is a guest.
        /// </summary>
        /// <returns></returns>
        public bool CheckIfUserIsGuest()
        {
            HomepageStocksRepository homepageStocksRepository = new();
            return homepageStocksRepository.IsGuestUser(homepageStocksRepository.GetUserCnp());
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
                var stock = new BaseStock(stockName, stockSymbol, authorCNP);
                int initialPrice = this.random.Next(50, 501);
                var result = await _stocksRepository.AddStockAsync(stock, initialPrice);
                return "Stock added successfully with initial value!";
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
                    return (false, "Author CNP cannot be empty.");
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
                await _stocksRepository.AddStockAsync(stock);

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