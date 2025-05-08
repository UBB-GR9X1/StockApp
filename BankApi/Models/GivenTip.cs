namespace BankApi.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
 
    /// <summary>
    /// Represents a given tip that a user has received.
    /// </summary>
    public class GivenTip
    {
        /// <summary>
        /// Gets or sets the unique identifier for this given tip.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the CNP (personal identification number) of the user who received the tip.
        /// </summary>
        [Required]
        [MaxLength(13)] // Assuming CNP is a 13-digit number
        public string UserCnp { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date when the tip was given.
        /// </summary>
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        /// <summary>
        /// Gets or sets the unique identifier of the tip that was given to the user.
        /// </summary>
        [Required]
        public int TipId { get; set; }
       

        /// <summary>
        /// Navigation property to the related Tip.
        /// </summary>
        public virtual Tip Tip { get; set; }
    }
}
