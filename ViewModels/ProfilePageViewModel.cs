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
    /// Initializes a new instance of the <see cref="ProfilePageViewModel"/> class with the specified profile service.
    /// </remarks>
    /// <param name="profileService">Service used to retrieve profile data.</param>
    public class ProfilePageViewModel : INotifyPropertyChanged
    {
        private readonly IProfileService profileService;
        private BitmapImage imageSource;
        private string username = string.Empty;
        private string description = string.Empty;
        private List<Stock> userStocks = new();

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
        /// Gets or sets the username.
        /// </summary>
        public string Username
        {
            get => this.username;
            private set
            {
                this.username = value;
                this.OnPropertyChanged(nameof(this.Username));
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description
        {
            get => this.description;
            private set
            {
                this.description = value;
                this.OnPropertyChanged(nameof(this.Description));
            }
        }

        /// <summary>
        /// Gets or sets the user stocks.
        /// </summary>
        public List<Stock> UserStocks
        {
            get => this.userStocks;
            private set
            {
                this.userStocks = value;
                this.OnPropertyChanged(nameof(this.UserStocks));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilePageViewModel"/> class with the default profile service and loads the profile image.
        /// </summary>
        public ProfilePageViewModel()
        {
            try
            {
                this.profileService = new ProfileService();
                this.LoadProfileData();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing ProfilePageViewModel: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilePageViewModel"/> class with the specified profile service.
        /// </summary>
        /// <param name="profileService">Service used to retrieve profile data.</param>
        public ProfilePageViewModel(IProfileService profileService)
        {
            this.profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
            this.LoadProfileData();
        }

        private void LoadProfileData()
        {
            try
            {
                this.Username = this.profileService.GetUsername();
                this.Description = this.profileService.GetDescription();
                this.UserStocks = this.profileService.GetUserStocks();
                this.LoadProfileImage();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading profile data: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Loads the profile image from the profile service.
        /// </summary>
        private void LoadProfileImage()
        {
            try
            {
                string imageUrl = this.profileService.GetImage();
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    this.ImageSource = new BitmapImage(new Uri(imageUrl));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                // Don't throw here, as image loading failure shouldn't break the whole profile
            }
        }

        /// <summary>
        /// Gets the CNP of the currently logged-in user.
        /// </summary>
        /// <returns>The user's CNP as a string.</returns>
        public string GetLoggedInUserCnp() => this.profileService.GetLoggedInUserCnp();

        /// <summary>
        /// Gets the username of the currently logged-in user.
        /// </summary>
        /// <returns>The username.</returns>
        public string GetUsername() => this.Username;

        /// <summary>
        /// Gets the description of the user.
        /// </summary>
        /// <returns>The user description.</returns>
        public string GetDescription() => this.Description;

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
        public List<Stock> GetUserStocks() => this.UserStocks;

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
