namespace StockNewsPage.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public string[] Roles { get; set; }
    }
}

