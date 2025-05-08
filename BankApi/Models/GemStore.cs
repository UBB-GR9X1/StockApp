using System;

namespace BankApi.Models
{
    public class GemStore
    {
        public string Cnp { get; set; } = string.Empty;
        public int GemBalance { get; set; }
        public bool IsGuest { get; set; }
        public DateTime LastUpdated { get; set; }
    }
} 