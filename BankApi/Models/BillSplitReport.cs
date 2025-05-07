using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApi.Models
{
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
        public float BillShare { get; set; }
    }
} 