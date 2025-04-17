namespace StockApp.Models
{
    public class User(string? cnp = null, string? username = null, string? description = null, bool? isModerator = null, string? image = null, bool? isHidden = null, int? gem_balance = null)
    {
        public string CNP { get; set; } = cnp ?? string.Empty;

        public string Username { get; set; } = username ?? string.Empty;

        public string Description { get; set; } = description ?? string.Empty;

        public bool IsModerator { get; set; } = isModerator ?? false;

        public string Image { get; set; } = image ?? string.Empty;

        public bool IsHidden { get; set; } = isHidden ?? false;
    }
}
