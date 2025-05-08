using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApi.Models
{
    /// <summary>
    /// Represents a stock holding, including its name, symbol, author, price per share, and quantity.
    /// </summary>
    /// <param name="name">The display name of the stock.</param>
    /// <param name="symbol">The trading symbol of the stock.</param>
    /// <param name="authorCNP">The CNP identifier of the author who created this entry.</param>
    /// <param name="price">The purchase price of each share.</param>
    /// <param name="quantity">The number of shares held.</param>
    public class Stock
    {
        [Key]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Symbol { get; set; } = string.Empty;

        [Required]
        public int Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [MaxLength(13)]
        public string AuthorCnp { get; set; } = string.Empty;

        public ICollection<UserStock> UserStocks { get; set; } = new List<UserStock>();

        /// <summary>
        /// Returns a string that represents the current stock,
        /// including its name, symbol, quantity, and price.
        /// </summary>
        public override string ToString()
        {
            return $"{this.Name} ({this.Symbol}) - x{this.Quantity} at {this.Price}";
        }
    }
}
