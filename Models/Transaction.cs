namespace StockApp.Models
{
    using System;

    public class Transaction : BaseStock
    {
        public string TransactionType { get; set; }

        public int Amount { get; set; }

        public int PricePerStock { get; set; }

        public int TotalValue => Amount * PricePerStock;

        public DateTime TransactionDate { get; set; }

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
