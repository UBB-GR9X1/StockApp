namespace StockApp.Models
{
    public class User(string cnp, string username, string description, bool is_moderator, string image, bool is_hidden)
    {
        public string CNP { get; set; } = cnp;

        public string Username { get; set; } = username;

        public string Description { get; set; } = description;

        public bool IsModerator { get; set; } = is_moderator;

        public string Image { get; set; } = image;

        public bool IsHidden { get; set; } = is_hidden;
    }
}
