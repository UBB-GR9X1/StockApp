namespace StockApp.Models
{
    using System;

    /// <summary>
    /// Represents a stock transaction, including its type, amount, price, and timing details.
    /// </summary>
    /// <param name="name">The display name of the stock.</param>
    /// <param name="symbol">The trading symbol of the stock.</param>
    /// <param name="authorCnp">The CNP identifier of the author who created this entry.</param>
    /// <param name="transactionType">The type of transaction (e.g., "Buy", "Sell").</param>
    /// <param name="amount">The number of shares transacted.</param>
    /// <param name="pricePerSstock">The price per share at the time of transaction.</param>
    /// <param name="transactionTime">The date and time when the transaction occurred.</param>
    /// <param name="transactionAuthorCnp">The CNP identifier of the user who executed the transaction.</param>
    public class Transaction(
        string name,
        string symbol,
        string authorCnp,
        string transactionType,
        int amount,
        int pricePerSstock,
        DateTime transactionTime,
        string transactionAuthorCnp)
        : BaseStock(name, symbol, authorCnp)
    {
        /// <summary>
        /// Gets or sets the type of this transaction (e.g., buy or sell).
        /// </summary>
        public string TransactionType { get; set; } = transactionType;

        /// <summary>
        /// Gets or sets the number of shares involved in the transaction.
        /// </summary>
        public int Amount { get; set; } = amount;

        /// <summary>
        /// Gets or sets the price paid per share in the transaction.
        /// </summary>
        public int PricePerStock { get; set; } = pricePerSstock;

        /// <summary>
        /// Gets the total value of the transaction (Amount multiplied by PricePerStock).
        /// </summary>
        public int TotalValue
            // Calculate the total value of this transaction
            => Amount * PricePerStock;

        /// <summary>
        /// Gets or sets the date and time when this transaction took place.
        /// </summary>
        public DateTime TransactionDate { get; set; } = transactionTime;

        /// <summary>
        /// Gets or sets the CNP identifier of the user who executed this transaction.
        /// </summary>
        public string TransactionAuthorCnp { get; set; } = transactionAuthorCnp;
    }
}
