namespace StockApp.Models
{
    using System;

    public class TransactionLogTransaction : ITransactionLogTransaction
    {
        public string StockSymbol { get; }

        public string StockName { get; }

        public string Type { get; }

        public int Amount { get; }

        public int PricePerStock { get; }

        public int TotalValue => Amount * PricePerStock;

        public DateTime Date { get; }

        public string Author { get; }

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
                throw new ArgumentException("StockSymbol required");
            if (string.IsNullOrWhiteSpace(stockName))
                throw new ArgumentException("StockName required");
            if (type is not ("BUY" or "SELL"))
                throw new ArgumentException("Type must be BUY or SELL");
            if (amount <= 0)
                throw new ArgumentException("Amount > 0");
            if (pricePerStock <= 0)
                throw new ArgumentException("PricePerStock > 0");
            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("Author required");

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
