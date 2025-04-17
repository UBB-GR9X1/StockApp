namespace StockApp.Models
{
    using System;

    public class TransactionLogTransaction
    {
        public TransactionLogTransaction(
            string stockSymbol,
            string stockName,
            string type,
            int amount,
            int pricePerStock,
            DateTime date,
            string author)
        {
            if (string.IsNullOrWhiteSpace(stockSymbol))
            {
                throw new Exception("Stock symbol cannot be empty!");
            }

            if (string.IsNullOrWhiteSpace(stockName))
            {
                throw new Exception("Stock name cannot be empty!");
            }

            if (string.IsNullOrWhiteSpace(type))
            {
                throw new Exception("Transaction type cannot be empty!");
            }

            if (!type.Equals("BUY") && !type.Equals("SELL"))
            {
                throw new Exception("Transaction type must be \"BUY\" or \"SELL\"!");
            }

            if (amount <= 0)
            {
                throw new Exception("Amount must be greater than zero.");
            }

            if (pricePerStock <= 0)
            {
                throw new Exception("Price per stock must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(author))
            {
                throw new Exception("Author cannot be empty.");
            }

            this.StockSymbol = stockSymbol;
            this.StockName = stockName;
            this.Type = type;
            this.Amount = amount;
            this.PricePerStock = pricePerStock;
            this.Date = date;
            this.Author = author;
        }

        public string StockSymbol { get; private set; }

        public string StockName { get; private set; }

        public string Type { get; private set; }

        public int Amount { get; private set; }

        public int PricePerStock { get; private set; }

        public int TotalValue => this.Amount * this.PricePerStock;

        public DateTime Date { get; private set; }

        public string Author { get; private set; }
    }
}
