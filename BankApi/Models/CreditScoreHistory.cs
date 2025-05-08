using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApi.Models
{
    public class CreditScoreHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(13)]
        public string UserCnp { get; set; } = string.Empty;

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        [Range(0, 1000)]
        public int Score { get; set; }

        public CreditScoreHistory()
        {
        }

        public CreditScoreHistory(int id, string userCnp, DateOnly date, int creditScore)
        {
            Id = id;
            UserCnp = userCnp;
            Date = date;
            Score = creditScore;
        }
    }
}
