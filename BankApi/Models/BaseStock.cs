using System.ComponentModel.DataAnnotations;

namespace BankApi.Models
{
    /// <summary>
    /// Represents the base information for a stock in the API.
    /// </summary>
    public class BaseStock
    {
        /// <summary>
        /// Gets or sets the unique identifier for the stock.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the display name of the stock.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the trading symbol of the stock.
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the CNP identifier of the author who created this entry.
        /// </summary>
        [Required]
        [MaxLength(13)]
        public string AuthorCNP { get; set; } = string.Empty;
    }
}