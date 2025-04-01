using StockApp.Model;
using StockApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.CreateStock.Service
{
    internal class CreateStockService
    {
        private readonly BaseStocksRepository _stocksRepository;

        public CreateStockService()
        {
            _stocksRepository = new BaseStocksRepository();
        }

        public string AddStock(string stockName, string stockSymbol, string authorCNP)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(stockName) || string.IsNullOrWhiteSpace(stockSymbol) || string.IsNullOrWhiteSpace(authorCNP))
                {
                    return "All fields are required!";
                }

                var stock = new BaseStock(stockName, stockSymbol, authorCNP);
                _stocksRepository.AddStock(stock);
                return "Stock added successfully!";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
