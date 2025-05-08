using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApi.Models
{
    public class Profile
    {
        [Key]
        [MaxLength(13)]
        public string Cnp { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ProfilePicture { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public bool IsHidden { get; set; }

        [Required]
        public bool IsAdmin { get; set; }

        [Required]
        public int GemBalance { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
} 