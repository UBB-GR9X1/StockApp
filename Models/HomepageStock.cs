namespace StockApp.Models
{
    /// <summary>
    /// Represents a stock item on the homepage, including its current price, change, and favorite status.
    /// </summary>
    public class HomepageStock
    {
        /// <summary>
        /// Gets or sets the unique identifier for the homepage stock.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the stock symbol.
        /// </summary>
        required public Stock StockDetails { get; set; }

        /// <summary>
        /// Gets or sets the price change, prefixed with '+' or '-' as appropriate.
        /// </summary>
        public decimal Change { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this stock is marked as a favorite.
        /// </summary>
        public bool IsFavorite { get; set; }
    }
}
