namespace StockApp.Services
{
    using System;
    using System.Text.RegularExpressions;
    using StockApp.Models;
    using StockApp.Repositories;

    internal class CreateStockService : ICreateStockService
    {
        private readonly BaseStocksRepository stocksRepository;
        private readonly Random random = new();

        public CreateStockService(BaseStocksRepository? stocksRepository = null)
        {
            this.stocksRepository = stocksRepository ?? new BaseStocksRepository();
        }

        public bool CheckIfUserIsGuest()
        {
            HomepageStocksRepository homepageStocksRepository = new();
            return homepageStocksRepository.IsGuestUser(homepageStocksRepository.GetUserCnp());
        }

        public string AddStock(string stockName, string stockSymbol, string authorCNP)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(stockName) ||
                    string.IsNullOrWhiteSpace(stockSymbol) ||
                    string.IsNullOrWhiteSpace(authorCNP))
                {
                    return "All fields are required!";
                }

                if (!Regex.IsMatch(stockSymbol, @"^[A-Z]{1,5}$"))
                {
                    return "Stock symbol must be 1-5 uppercase letters!";
                }

                if (!Regex.IsMatch(authorCNP, @"^\d{13}$"))
                {
                    return "Invalid CNP! It must be exactly 13 digits.";
                }

                var stock = new BaseStock(stockName, stockSymbol, authorCNP);

                int initialPrice = this.random.Next(50, 501);

                this.stocksRepository.AddStock(stock, initialPrice);

                return "Stock added successfully with initial value!";
            }
            catch (Exception ex)
            {
                return $"Failed to add stock: {ex.Message}";
            }
        }
    }
}