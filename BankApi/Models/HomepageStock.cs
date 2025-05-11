namespace BankApi.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class HomepageStock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Symbol { get; set; } = string.Empty;

        [Required]
        public Stock StockDetails { get; set; } = new Stock();

        [NotMapped]
        public bool IsFavorite { get; set; }

        public decimal Change { get; set; }
    }
}
