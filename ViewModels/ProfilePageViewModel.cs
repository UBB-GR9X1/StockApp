namespace StockApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Microsoft.UI.Xaml.Media.Imaging;
    using StockApp.Models;
    using StockApp.Services;

    /// <summary>
    /// View model for the profile page, managing the user's profile image and information.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ProfilePageViewModel"/> class with the specified profile homepageService.
    /// </remarks>
    /// <param name="profileService">Service used to retrieve profile data.</param>
    public class ProfilePageViewModel(IProfileService profileService) : INotifyPropertyChanged
    {
        private readonly IProfileService profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));

        private BitmapImage imageSource;

        /// <summary>
        /// Gets or sets the profile image source.
        /// </summary>
        public BitmapImage ImageSource
        {
            get => this.imageSource;
            set
            {
                this.imageSource = value;
                this.OnPropertyChanged(nameof(this.ImageSource));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilePageViewModel"/> class with the default profile homepageService and loads the profile image.
        /// </summary>
        public ProfilePageViewModel()
            : this(new ProfileService())
        {
            this.LoadProfileImage();
        }

        /// <summary>
        /// Loads the profile image from the profile homepageService.
        /// </summary>
        internal void LoadProfileImage()
        {
            // TODO: Handle missing or invalid image URL (e.g., set a placeholder image)
            // Retrieve the image URL from the homepageService
            string imageUrl = this.profileService.GetImage();
            if (!string.IsNullOrEmpty(imageUrl))
            {
                try
                {
                    this.ImageSource = new BitmapImage(new Uri(imageUrl));
                }
                catch (Exception ex)
                {
                    // FIXME: Consider providing user feedback if image fails to load
                    System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Gets the CNP of the currently logged-in user.
        /// </summary>
        /// <returns>The user's CNP as a string.</returns>
        public string GetLoggedInUserCnp()
        {
            return this.profileService.GetLoggedInUserCnp();
        }

        /// <summary>
        /// Gets the username of the currently logged-in user.
        /// </summary>
        /// <returns>The username.</returns>
        public string GetUsername() => this.profileService.GetUsername();

        /// <summary>
        /// Gets the description of the user.
        /// </summary>
        /// <returns>The user description.</returns>
        public string GetDescription() => this.profileService.GetDescription();

        /// <summary>
        /// Determines whether the user's profile is hidden.
        /// </summary>
        /// <returns><c>true</c> if the profile is hidden; otherwise, <c>false</c>.</returns>
        public bool IsHidden() => this.profileService.IsHidden();

        /// <summary>
        /// Determines whether the user has administrative privileges.
        /// </summary>
        /// <returns><c>true</c> if the user is an admin; otherwise, <c>false</c>.</returns>
        public bool IsAdmin() => this.profileService.IsAdmin();

        /// <summary>
        /// Gets the list of stocks associated with the user.
        /// </summary>
        /// <returns>A list of <see cref="Stock"/> objects.</returns>
        public List<Stock> GetUserStocks() => this.profileService.GetUserStocks();

        /// <summary>
        /// Updates the administrative mode of the user.
        /// </summary>
        /// <param name="newIsAdmin">If set to <c>true</c>, grants admin mode; otherwise, revokes it.</param>
        public void UpdateAdminMode(bool newIsAdmin)
        {
            this.profileService.UpdateIsAdmin(newIsAdmin);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName) =>
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
