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
        public string CompanyName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal Change { get; set; }

        public decimal PercentChange { get; set; }
    }
}
