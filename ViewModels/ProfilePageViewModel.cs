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
    /// <remarks>
    /// Initializes a new instance of the <see cref="ProfilePageViewModel"/> class with the specified profile service.
    /// </remarks>
    /// <param name="profileService">Service used to retrieve profile data.</param>
    public class ProfilePageViewModel : INotifyPropertyChanged
    {
        private readonly IProfileService profileService;
        private readonly IUserService userService;
        private BitmapImage imageSource;
        private string username = string.Empty;
        private string description = string.Empty;
        private List<Stock> userStocks = [];
        private Stock? selectedStock;
        private bool isAdmin = false;
        private bool isHidden = false;

        public bool IsGuest => this.userService.IsGuest();

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

        public Stock? SelectedStock
        {
            get => this.selectedStock;
            set
            {
                this.selectedStock = value;
                this.OnPropertyChanged(nameof(this.SelectedStock));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the profile is hidden.
        /// </summary>
        public bool IsAdmin
        {
            get => this.isAdmin;
            set
            {
                this.isAdmin = value;
                this.OnPropertyChanged(nameof(this.isAdmin));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the profile is hidden.
        /// </summary>
        public bool IsHidden
        {
            get => this.isHidden;
            set
            {
                this.isHidden = value;
                this.OnPropertyChanged(nameof(this.IsHidden));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilePageViewModel"/> class with the default profile service and loads the profile image.
        /// </summary>
        public ProfilePageViewModel(IProfileService profileService, IUserService userService)
        {
            this.profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            try
            {
                this.profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
                if (!this.userService.IsGuest())
                {
                    _ = this.LoadProfileData();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing ProfilePageViewModel: {ex.Message}");
                throw;
            }
        }

        private async Task LoadProfileData()
        {
            try
            {
                User currentUser = await this.userService.GetCurrentUserAsync();

                this.Username = currentUser.Username;
                this.Description = currentUser.Description;
                this.IsAdmin = currentUser.IsModerator;
                this.IsHidden = currentUser.IsHidden;
                this.UserStocks = await this.profileService.GetUserStocksAsync();

                if (!string.IsNullOrEmpty(currentUser.Image) && Uri.IsWellFormedUriString(currentUser.Image, UriKind.Absolute))
                {
                    this.ImageSource = new BitmapImage(new Uri(currentUser.Image));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading profile data: {ex.Message}");
                throw;
            }
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
