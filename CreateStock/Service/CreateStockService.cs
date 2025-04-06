using StockApp.Model;
using StockApp.Repositories;
using StocksHomepage.Repositories;
using System;
using System.Text.RegularExpressions;

namespace StockApp.CreateStock.Service
{
    internal class CreateStockService
    {
        private readonly BaseStocksRepository _stocksRepository;
        private readonly Random _random = new Random();

        public CreateStockService(BaseStocksRepository stocksRepository = null)
        {
            _stocksRepository = stocksRepository ?? new BaseStocksRepository();
        }

        public bool checkIfUserIsGuest()
        {
            HomepageStocksRepository homepageStocksRepository = new HomepageStocksRepository();
            return homepageStocksRepository.IsGuestUser(homepageStocksRepository.getCNP());
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

                int initialPrice = _random.Next(50, 501);

                _stocksRepository.AddStock(stock, initialPrice);

                return "Stock added successfully with initial value!";
            }
            catch (Exception ex)
            {
                return $"Failed to add stock: {ex.Message}";
            }
        }
    }
}