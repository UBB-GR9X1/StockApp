using System;
using System.ComponentModel.DataAnnotations;

namespace BankApi.Models
{
    public class LoanRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(13)]
        public string UserCnp { get; set; }

        [Required]
        public float Amount { get; set; }

        [Required]
        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime RepaymentDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string Status { get; set; }

        public LoanRequest(int requestId, string userCnp, float amount, DateTime applicationDate, DateTime repaymentDate, string status)
        {
            this.Id = requestId;
            this.UserCnp = userCnp;
            this.Amount = amount;
            this.ApplicationDate = applicationDate;
            this.RepaymentDate = repaymentDate;
            this.Status = status;
        }
    }
}
