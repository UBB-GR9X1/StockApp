namespace StockApp.Models
{
    using System;

    public interface ITransactionLogTransaction
    {
        string StockSymbol { get; }

        string StockName { get; }

        string Type { get; }

        int Amount { get; }

        int PricePerStock { get; }

        int TotalValue { get; }

        DateTime Date { get; }

        string Author { get; }
    }
}
