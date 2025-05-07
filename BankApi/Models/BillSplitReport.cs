using System;
using System.ComponentModel.DataAnnotations;

namespace BankApi.Models
{
    /// <summary>
    /// Represents a bill split report in the system.
    /// </summary>
    public class BillSplitReport
    {
        /// <summary>
        /// Gets or sets the unique identifier for the bill split report.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the CNP of the user who was reported.
        /// </summary>
        [Required]
        [MaxLength(13)]
        public string ReportedUserCnp { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the CNP of the user who reported.
        /// </summary>
        [Required]
        [MaxLength(13)]
        public string ReportingUserCnp { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date of the transaction.
        /// </summary>
        [Required]
        public DateTime DateOfTransaction { get; set; }

        /// <summary>
        /// Gets or sets the bill share amount.
        /// </summary>
        [Required]
        public float BillShare { get; set; }
    }
} 