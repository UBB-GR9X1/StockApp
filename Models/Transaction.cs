namespace StockApp.Models
{
    using System;

    public class Transaction : BaseStock
    {
        public string TransactionType { get; set; } = transactionType;

        public int Amount { get; set; } = amount;

        public int PricePerStock { get; set; } = pricePerSstock;

        public int TotalValue => Amount * PricePerStock;

        public DateTime TransactionDate { get; set; } = transactionTime;

        public string TransactionAuthorCnp { get; set; }

        public Transaction(
            string name,
            string symbol,
            string authorCnp,
            string transactionType,
            int amount,
            int pricePerStock,
            DateTime transactionDate,
            string transactionAuthorCnp)
            : base(name, symbol, authorCnp)
        {
            TransactionType = transactionType;
            Amount = amount;
            PricePerStock = pricePerStock;
            TransactionDate = transactionDate;
            TransactionAuthorCnp = transactionAuthorCnp;
        }
    }
}
