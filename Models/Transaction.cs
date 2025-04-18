namespace StockApp.Models
{
    using System;

    public class Transaction(
        string name,
        string symbol,
        string authorCnp,
        string transactionType,
        int amount,
        int pricePerSstock,
        DateTime transactionTime,
        string transactionAuthorCnp) : BaseStock(name, symbol, authorCnp)
    {
        public string TransactionType { get; set; } = transactionType;

        public int Amount { get; set; } = amount;

        public int PricePerStock { get; set; } = pricePerSstock;

        public int TotalValue => Amount * PricePerStock;

        public DateTime TransactionDate { get; set; } = transactionTime;

        public string TransactionAuthorCnp { get; set; } = transactionAuthorCnp;
    }
}
