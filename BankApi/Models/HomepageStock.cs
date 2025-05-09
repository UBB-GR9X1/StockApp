namespace BankApi.Models
{
    using System.ComponentModel.DataAnnotations;

    public class HomepageStock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Symbol { get; set; } = string.Empty;

        [Required]
        public Stock StockDetails { get; set; } = new Stock();

        public decimal Change { get; set; }
    }
}
