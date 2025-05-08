namespace BankApi.Models
{
    /// <summary>
    /// Represents a stock item on the homepage, including its current price, change, and favorite status.
    /// </summary>
    public class HomepageStock
    {
        /// <summary>
        /// Gets or sets the stock symbol.
        /// </summary>
        required public Stock StockDetails { get; set; }

        /// <summary>
        /// Gets or sets the price change, prefixed with '+' or '-' as appropriate.
        /// </summary>
        public string Change { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this stock is marked as a favorite.
        /// </summary>
        public bool IsFavorite { get; set; }

    }
}
