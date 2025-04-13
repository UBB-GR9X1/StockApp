using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Model
{
    public class TransactionLogTransaction
    {
        public string StockSymbol { get; private set; }
        public string StockName { get; private set; }
        public string Type { get; private set; }
        public int Amount { get; private set; }
        public int PricePerStock { get; private set; }
        public int TotalValue => Amount * PricePerStock;
        public DateTime Date { get; private set; }
        public string Author { get; private set; }

        public TransactionLogTransaction(string stockSymbol, string stockName, string type, int amount, int pricePerStock, DateTime date, string author)
        {
            if (string.IsNullOrWhiteSpace(stockSymbol)) 
                throw new Exception("Stock symbol cannot be empty!");
            if (string.IsNullOrWhiteSpace(stockName)) 
                throw new Exception("Stock name cannot be empty!");
            if (string.IsNullOrWhiteSpace(type)) 
                throw new Exception("Transaction type cannot be empty!");
            if (!type.Equals("BUY") && !type.Equals("SELL"))
                throw new Exception("Transaction type must be \"BUY\" or \"SELL\"!");
            if (amount <= 0) 
                throw new Exception("Amount must be greater than zero.");
            if (pricePerStock <= 0) 
                throw new Exception("Price per stock must be greater than zero.");
            if (string.IsNullOrWhiteSpace(author)) 
                throw new Exception("Author cannot be empty.");

            StockSymbol = stockSymbol;
            StockName = stockName;
            Type = type;
            Amount = amount;
            PricePerStock = pricePerStock;
            Date = date;
            Author = author;
        }
    }
}
