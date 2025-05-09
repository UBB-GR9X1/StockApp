namespace StockApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Microsoft.UI.Xaml.Media.Imaging;
    using StockApp.Models;
    using StockApp.Services;

    /// <summary>
    /// View model for the profile page, managing the user's profile image and information.
    /// </summary>
    public class ProfilePageViewModel : INotifyPropertyChanged
    {
        private readonly IProfileService profileService;
        private BitmapImage? imageSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilePageViewModel"/> class with the default profile service and loads the profile image.
        /// </summary>
        /// <param name="profileService">Service used to retrieve profile data.</param>
        public ProfilePageViewModel(IProfileService profileService)
        {
            this.profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
            this.PropertyChanged = null!;
            this.LoadProfileImageAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets or sets the profile image source.
        /// </summary>
        public BitmapImage? ImageSource
        {
            get => this.imageSource;
            set
            {
                this.imageSource = value;
                this.OnPropertyChanged(nameof(this.ImageSource));
            }
        }

        /// <summary>
        /// Loads the profile image from the profile service asynchronously.
        /// </summary>
        private async Task LoadProfileImageAsync()
        {
            try
            {
                var user = await this.profileService.CurrentUserProfile;
                if (!string.IsNullOrEmpty(user.Image))
                {
                    this.ImageSource = new BitmapImage(new Uri(user.Image));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the CNP of the currently logged-in user.
        /// </summary>
        /// <returns>The user's CNP as a string.</returns>
        public async Task<string> GetLoggedInUserCnpAsync()
        {
            var user = await this.profileService.CurrentUserProfile;
            return user.CNP ?? string.Empty;
        }

        /// <summary>
        /// Gets the username of the currently logged-in user.
        /// </summary>
        /// <returns>The username.</returns>
        public async Task<string> GetUsernameAsync()
        {
            var user = await this.profileService.CurrentUserProfile;
            return user.Username ?? string.Empty;
        }

        /// <summary>
        /// Gets the description of the user.
        /// </summary>
        /// <returns>The user description.</returns>
        public async Task<string> GetDescriptionAsync()
        {
            var user = await this.profileService.CurrentUserProfile;
            return user.Description ?? string.Empty;
        }

        /// <summary>
        /// Determines whether the user's profile is hidden.
        /// </summary>
        /// <returns><c>true</c> if the profile is hidden; otherwise, <c>false</c>.</returns>
        public async Task<bool> IsHiddenAsync()
        {
            var user = await this.profileService.CurrentUserProfile;
            return user.IsHidden;
        }

        /// <summary>
        /// Determines whether the user has administrative privileges.
        /// </summary>
        /// <returns><c>true</c> if the user is an admin; otherwise, <c>false</c>.</returns>
        public async Task<bool> IsAdminAsync()
        {
            var user = await this.profileService.CurrentUserProfile;
            return user.IsModerator;
        }

        /// <summary>
        /// Gets the list of stocks associated with the user.
        /// </summary>
        /// <returns>A list of <see cref="Stock"/> objects.</returns>
        public async Task<List<Stock>> GetUserStocksAsync()
        {
            return await this.profileService.GetUserStocksAsync();
        }

        /// <summary>
        /// Updates the administrative mode of the user.
        /// </summary>
        /// <param name="newIsAdmin">If set to <c>true</c>, grants admin mode; otherwise, revokes it.</param>
        public async Task UpdateAdminModeAsync(bool newIsAdmin)
        {
            await this.profileService.UpdateIsAdminAsync(newIsAdmin);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName) =>
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
