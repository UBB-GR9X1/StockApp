using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApi.Models
{
    /// <summary>
    /// Represents a user's stock holding, including its base information and quantity.
    /// </summary>
    /// <param name="name">The display name of the stock.</param>
    /// <param name="symbol">The trading symbol of the stock.</param>
    /// <param name="authorCnp">The CNP identifier of the author who created this entry.</param>
    /// <param name="quantity">The number of shares held by the user.</param>
    public class UserStock
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserCNP { get; set; }
        
        [Required]
        public string StockName { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [ForeignKey("UserCNP")]
        public User User { get; set; }
        
        [ForeignKey("StockName")]
        public Stock Stock { get; set; }
    }
}
