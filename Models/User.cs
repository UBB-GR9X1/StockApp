namespace StockApp.Models
{
    /// <summary>
    /// Represents an application user, with optional profile details and permissions.
    /// </summary>
    /// <param name="cnp">The unique CNP identifier of the user; optional.</param>
    /// <param name="username">The display username; optional.</param>
    /// <param name="description">A short bio or description; optional.</param>
    /// <param name="isModerator">Whether the user has moderator privileges; optional.</param>
    /// <param name="image">A URL or path to the user's avatar image; optional.</param>
    /// <param name="isHidden">Whether the user's profile is hidden; optional.</param>
    /// <param name="gem_balance">The user's starting gem balance; optional.</param>
    public class User(
        string? cnp = null,
        string? username = null,
        string? description = null,
        bool? isModerator = null,
        string? image = null,
        bool? isHidden = null,
        int? gem_balance = null)
    {
        /// <summary>
        /// Gets or sets the user's CNP identifier.
        /// </summary>
        public string CNP { get; set; } = cnp ?? string.Empty;

        /// <summary>
        /// Gets or sets the user's display name.
        /// </summary>
        public string Username { get; set; } = username ?? string.Empty;

        /// <summary>
        /// Gets or sets the user's profile description or bio.
        /// </summary>
        public string Description { get; set; } = description ?? string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this user is a moderator.
        /// </summary>
        public bool IsModerator { get; set; } = isModerator ?? false;

        /// <summary>
        /// Gets or sets the path or URL to the user's avatar image.
        /// </summary>
        public string Image { get; set; } = image ?? string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the user's profile is hidden.
        /// </summary>
        public bool IsHidden { get; set; } = isHidden ?? false;

        /// <summary>
        /// Gets or sets the user's gem balance.
        /// </summary>
        public int GemBalance { get; set; } = gem_balance ?? 0;
    }
}
