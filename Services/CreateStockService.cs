namespace StockApp.Services
{
    using System;
    using System.Text.RegularExpressions;
    using Microsoft.Data.SqlClient;
    using StockApp.Exceptions;
    using StockApp.Models;
    using StockApp.Repositories;
    using System.Threading.Tasks;
    using StockApp.Database;

    /// <summary>
    /// Service for creating stocks
    /// </summary>
    internal class CreateStockService : ICreateStockService
    {
        private readonly IBaseStocksRepository _stocksRepository;
        private readonly Random random = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateStockService"/> class.
        /// </summary>
        public CreateStockService(AppDbContext dbContext)
        {
            _stocksRepository = new BaseStocksRepository(dbContext);
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
        public string AddStock(string stockName, string stockSymbol, string authorCNP)
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
                var result = _stocksRepository.AddStockAsync(stock, initialPrice).GetAwaiter().GetResult();
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

        public async Task<bool> CreateStock(string stockName, string stockSymbol, string authorCnp, out string message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(stockName))
                {
                    message = "Stock name cannot be empty.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(stockSymbol))
                {
                    message = "Stock symbol cannot be empty.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(authorCnp))
                {
                    message = "Author CNP cannot be empty.";
                    return false;
                }

                if (stockName.Length > 100)
                {
                    message = "Stock name cannot exceed 100 characters.";
                    return false;
                }

                if (stockSymbol.Length > 10)
                {
                    message = "Stock symbol cannot exceed 10 characters.";
                    return false;
                }

                var stock = new BaseStock(stockName, stockSymbol, authorCnp);
                await _stocksRepository.AddStockAsync(stock);
                
                message = "Stock created successfully!";
                return true;
            }
            catch (DuplicateStockException)
            {
                message = $"A stock with the name '{stockName}' already exists.";
                return false;
            }
            catch (Exception ex)
            {
                message = $"Error creating stock: {ex.Message}";
                return false;
            }
        }
    }
}