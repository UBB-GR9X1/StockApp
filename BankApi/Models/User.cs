namespace BankApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string CNP { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string HashedPassword { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsModerator { get; set; }
        public string Image { get; set; } = string.Empty;
        public bool IsHidden { get; set; }
        public int GemBalance { get; set; }
        public int NumberOfOffenses { get; set; }
        public int RiskScore { get; set; }
        public decimal ROI { get; set; }
        public int CreditScore { get; set; }
        public DateOnly Birthday { get; set; }
        public string ZodiacSign { get; set; } = string.Empty;
        public string ZodiacAttribute { get; set; } = string.Empty;
        public int NumberOfBillSharesPaid { get; set; }
        public int Income { get; set; }
        public decimal Balance { get; set; }
    }
}
