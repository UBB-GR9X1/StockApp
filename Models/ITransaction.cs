namespace StockApp.Models
{
    using System;

    public interface ITransaction : IBaseStock
    {
        string TransactionType { get; set; }

        int Amount { get; set; }

        int PricePerStock { get; set; }

        int TotalValue { get; }

        DateTime TransactionDate { get; set; }

        string TransactionAuthorCnp { get; set; }
    }
}
