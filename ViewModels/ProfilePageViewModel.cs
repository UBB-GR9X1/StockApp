namespace StockApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Microsoft.UI.Xaml.Media.Imaging;
    using StockApp.Models;
    using StockApp.Services;

    public class ProfilePageViewModel : INotifyPropertyChanged
    {
        private readonly IProfileService profileService;

        private BitmapImage imageSource;
        public BitmapImage ImageSource
        {
            get => this.imageSource;
            set
            {
                this.imageSource = value;
                this.OnPropertyChanged(nameof(this.ImageSource));
            }
        }

        public ProfilePageViewModel(IProfileService profileService)
        {
            this.profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
        }

        // default ctor for production
        public ProfilePageViewModel()
            : this(new ProfileService())
        {
            this.LoadProfileImage();
        }

        internal void LoadProfileImage()
        {
            string imageUrl = this.profileService.GetImage();
            if (!string.IsNullOrEmpty(imageUrl))
            {
                try
                {
                    this.ImageSource = new BitmapImage(new Uri(imageUrl));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                }
            }
        }

        public string GetLoggedInUserCnp()
        {
            return this.profileService.GetLoggedInUserCnp();
        }

        public string GetUsername() => this.profileService.GetUsername();
        public string GetDescription() => this.profileService.GetDescription();
        public bool IsHidden() => this.profileService.IsHidden();
        public bool IsAdmin() => this.profileService.IsAdmin();
        public List<Stock> GetUserStocks() => this.profileService.GetUserStocks();
        public void UpdateAdminMode(bool newIsAdmin)
        {
            this.profileService.UpdateIsAdmin(newIsAdmin);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
