namespace StockApp.Models
{
    public class User
    {
        public string CNP { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsModerator { get; set; }

        public string Image { get; set; } = string.Empty;

        public bool IsHidden { get; set; }

        public User(
            string? cnp = null,
            string? username = null,
            string? description = null,
            bool? isModerator = null,
            string? image = null,
            bool? isHidden = null)
        {
            CNP = cnp ?? string.Empty;
            Username = username ?? string.Empty;
            Description = description ?? string.Empty;
            IsModerator = isModerator ?? false;
            Image = image ?? string.Empty;
            IsHidden = isHidden ?? false;
        }
    }
}
