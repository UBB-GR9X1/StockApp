using System.ComponentModel.DataAnnotations;

namespace BankApi.Models
{
    using System;

    public class BillSplitReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(13)]
        public string ReportedUserCnp { get; set; }

        [Required]
        [StringLength(13)]
        public string ReportingUserCnp { get; set; }

        [Required]
        public DateTime DateOfTransaction { get; set; }

        [Required]
        public decimal BillShare { get; set; }
    }
}