
namespace BankApi.Models
{
    using System.ComponentModel.DataAnnotations;
    /// <summary>
    /// Represents a stock holding, including its name, symbol, author, price per share, and quantity.
    /// </summary>
    public class Stock : BaseStock
    {
        /// <summary>
        /// Gets or sets the unique identifier for the stock.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the purchase price of each share.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int Price { get; set; }

        /// <summary>
        /// Gets or sets the number of shares held.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        /// <summary>
        /// Default constructor required by Entity Framework.
        /// </summary>
        public Stock() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Stock"/> class.
        /// </summary>
        /// <param name="name">The display name of the stock.</param>
        /// <param name="symbol">The trading symbol of the stock.</param>
        /// <param name="authorCNP">The CNP identifier of the author who created this entry.</param>
        /// <param name="price">The purchase price of each share.</param>
        /// <param name="quantity">The number of shares held.</param>
        public Stock(string name, string symbol, string authorCNP, int price, int quantity)
        {
            base.Name = name;
            base.Symbol = symbol;
            base.AuthorCNP = authorCNP;
            this.Price = price;
            this.Quantity = quantity;
        }

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
