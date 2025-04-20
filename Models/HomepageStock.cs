namespace StockApp.Models
{
    using System.Windows.Input;
    using Microsoft.UI;
    using Microsoft.UI.Xaml.Media;

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

        /// <summary>
        /// Gets the brush color to represent the price change: green for positive, red for negative or zero.
        /// </summary>
        public SolidColorBrush ChangeColor

            // Use green when the change starts with '+', otherwise red
            => this.Change.StartsWith('+')
                ? new SolidColorBrush(Colors.Green)
                : new SolidColorBrush(Colors.Red);

        /// <summary>
        /// Gets the star symbol to display: a filled star if favorite, or an outline otherwise.
        /// </summary>
        public string FavoriteStar

            // Return filled star when favorite, outline star when not
            => this.IsFavorite ? "★" : "☆";
    }
}
