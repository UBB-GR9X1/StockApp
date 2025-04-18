namespace StockApp.Models
{
    public class User
    {
        public string CNP { get; set; } = string.Empty;

        public string Username { get; set; } = username ?? string.Empty;

        public string Description { get; set; } = description ?? string.Empty;

        public bool IsModerator { get; set; } = isModerator ?? false;

        public string Image { get; set; } = image ?? string.Empty;

        public bool IsHidden { get; set; } = isHidden ?? false;

        public User(
            string? cnp = null,
            string? username = null,
            string? description = null,
            bool? isModerator = null,
            string? image = null,
            bool? isHidden = null,
            int? gem_balance = null)
        {
            CNP = cnp ?? string.Empty;
            Username = username ?? string.Empty;
            Description = description ?? string.Empty;
            IsModerator = isModerator ?? false;
            Image = image ?? string.Empty;
            IsHidden = isHidden ?? false;
            GemBalance = gem_balance ?? 0;
        }
        public int GemBalance { get; set; } = gem_balance ?? 0;
    }
}
