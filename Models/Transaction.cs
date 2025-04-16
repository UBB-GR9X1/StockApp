namespace StockApp.Models
{
    using System;

    public class Transaction(
        string name,
        string symbol,
        string author_cnp,
        string transactionType,
        int amount,
        int pricePerStock,
        int totalValue,
        DateTime transactionDate,
        string transactionAuthorCNP) : BaseStock(name, symbol, author_cnp)
    {
        public string TransactionType { get; set; } = transactionType;

        public int Amount { get; set; } = amount;

        public int PricePerStock { get; set; } = pricePerStock;

        public int TotalValue { get; set; } = totalValue;

        public DateTime TransactionDate { get; set; } = transactionDate;

        public string TransactionAuthorCNP { get; set; } = transactionAuthorCNP;
    }
}
