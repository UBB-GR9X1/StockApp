using StockApp.Model;
using StockApp.Repositories;
using System;
using System.Text.RegularExpressions;

namespace StockApp.CreateStock.Service
{
    internal class CreateStockService
    {
        private readonly BaseStocksRepository _stocksRepository;
        public CreateStockService(BaseStocksRepository stocksRepository = null)
        {
            _stocksRepository = stocksRepository ?? new BaseStocksRepository();
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
                _stocksRepository.AddStock(stock);
                return "Stock added successfully!";
            }
            catch (Exception ex)
            {
                return $"Failed to add stock: {ex.Message}";
            }
        }
    }
}
