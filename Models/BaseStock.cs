namespace StockApp.Models
{
    /// <summary>
    /// Represents the base information for a stock, including its name, symbol, and the author’s CNP.
    /// </summary>
    /// <param name="name">The display name of the stock.</param>
    /// <param name="symbol">The trading symbol of the stock.</param>
    /// <param name="authorCnp">The CNP identifier of the author who created this entry.</param>
    public class BaseStock(string name, string symbol, string authorCnp)
    {
        /// <summary>
        /// Gets the display name of the stock.
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Gets the trading symbol of the stock.
        /// </summary>
        public string Symbol { get; } = symbol;

        /// <summary>
        /// Gets the CNP identifier of the author who created this entry.
        /// </summary>
        public string AuthorCnp { get; } = authorCnp;
    }
}
