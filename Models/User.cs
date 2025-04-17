namespace StockApp.Models
{
    public class User(string? cnp, string? username, string? description, bool? isModerator, string? image, bool? isHidden)
    {
        public string CNP { get; set; } = cnp ?? string.Empty;

        public string Username { get; set; } = username ?? string.Empty;

        public string Description { get; set; } = description ?? string.Empty;

        public bool IsModerator { get; set; } = isModerator ?? false;

        public string Image { get; set; } = image ?? string.Empty;

        public bool IsHidden { get; set; } = isHidden ?? false;
    }
}
