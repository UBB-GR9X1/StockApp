namespace StockApp.Services
{
    using System;
    using System.Text.RegularExpressions;
    using Microsoft.Data.SqlClient;
    using StockApp.Exceptions;
    using StockApp.Models;
    using StockApp.Repositories;

    internal class CreateStockService : ICreateStockService
    {
        private readonly BaseStocksRepository stocksRepository;
        private readonly Random random = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateStockService"/> class.
        /// </summary>
        /// <param name="stocksRepository"></param>
        public CreateStockService(BaseStocksRepository? stocksRepository = null)
        {
            this.stocksRepository = stocksRepository ?? new BaseStocksRepository();
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
                this.stocksRepository.AddStock(stock, initialPrice);
                return "Stock added successfully with initial value!";
            }
            catch (DuplicateStockException duplicateStockEx)
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
    }
}