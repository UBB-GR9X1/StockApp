namespace StockApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using StockApp.Models;
    using StockApp.Services;

    /// <summary>
    /// ViewModel for updating user profile details including username, image, description, and visibility.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="UpdateProfilePageViewModel"/> class with a specified homepageService.
    /// </remarks>
    /// <param name="service">Service used to retrieve and update profile information.</param>
    internal class UpdateProfilePageViewModel(IProfileService service)
    {
        private readonly IProfileService profileService = service ?? throw new ArgumentNullException(nameof(service));

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateProfilePageViewModel"/> class with the default profile homepageService.
        /// </summary>
        public UpdateProfilePageViewModel()
            : this(new ProfileService())
        {
            // Inline: default constructor chaining to use ProfileService implementation
        }

        /// <summary>
        /// Gets the URL of the user's profile image.
        /// </summary>
        /// <returns>The image URL as a string.</returns>
        public string GetImage()
        {
            // Inline: delegate image retrieval to homepageService
            return this.profileService.GetImage();
        }

        /// <summary>
        /// Gets the username of the current user.
        /// </summary>
        /// <returns>The username as a string.</returns>
        public string GetUsername()
        {
            // Inline: delegate username retrieval to homepageService
            return this.profileService.GetUsername();
        }

        /// <summary>
        /// Gets the description text for the current user.
        /// </summary>
        /// <returns>The description as a string.</returns>
        public string GetDescription()
        {
            // Inline: delegate description retrieval to homepageService
            return this.profileService.GetDescription();
        }

        /// <summary>
        /// Determines whether the user's profile is hidden.
        /// </summary>
        /// <returns><c>true</c> if hidden; otherwise, <c>false</c>.</returns>
        public bool IsHidden()
        {
            // Inline: delegate visibility check to homepageService
            return this.profileService.IsHidden();
        }

        /// <summary>
        /// Determines whether the current user has administrative privileges.
        /// </summary>
        /// <returns><c>true</c> if admin; otherwise, <c>false</c>.</returns>
        public bool IsAdmin()
        {
            // Inline: delegate admin check to homepageService
            return this.profileService.IsAdmin();
        }

        /// <summary>
        /// Gets the list of stocks associated with the user.
        /// </summary>
        /// <returns>A list of <see cref="Stock"/> objects.</returns>
        public List<Stock> GetUserStocks()
        {
            // Inline: retrieve user's stocks from homepageService
            return this.profileService.GetUserStocks();
        }

        /// <summary>
        /// Updates all user profile fields at once.
        /// </summary>
        /// <param name="newUsername">The new username.</param>
        /// <param name="newImage">The new profile image URL.</param>
        /// <param name="newDescription">The new description text.</param>
        /// <param name="newHidden">New hidden status for the profile.</param>
        public void UpdateAll(string newUsername, string newImage, string newDescription, bool newHidden)
        {
            // TODO: Validate inputs (e.g., non-null, length constraints)
            // FIXME: Consider handling exceptions from homepageService to provide user feedback
            this.profileService.UpdateUser(newUsername, newImage, newDescription, newHidden); // Inline: perform bulk update
        }

        /// <summary>
        /// Updates only the administrative mode of the user.
        /// </summary>
        /// <param name="newIsAdmin"><c>true</c> to grant admin; otherwise, <c>false</c>.</param>
        public void UpdateAdminMode(bool newIsAdmin)
        {
            // Inline: delegate admin mode toggle to homepageService
            this.profileService.UpdateIsAdmin(newIsAdmin);
        }
    }
}
