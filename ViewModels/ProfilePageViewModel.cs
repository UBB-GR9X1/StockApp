namespace StockApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Microsoft.UI.Xaml.Media.Imaging;
    using StockApp.Services;

    public class ProfilePageViewModel : INotifyPropertyChanged
    {
        private readonly ProfieService profileService;

        private BitmapImage _imageSource;
        public BitmapImage ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        public ProfilePageViewModel(string authorCnp)
        {
            profileService = new ProfieService(authorCnp);
            LoadProfileImage();
        }

        private void LoadProfileImage()
        {
            string imageUrl = profileService.GetImage();
            if (!string.IsNullOrEmpty(imageUrl))
            {
                try
                {
                    ImageSource = new BitmapImage(new Uri(imageUrl));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                }
            }
        }


        public string GetLoggedInUserCnp()
        {
            return profileService.GetLoggedInUserCnp();
        }

        public string GetUsername() => profileService.GetUsername();
        public string GetDescription() => profileService.GetDescription();
        public bool IsHidden() => profileService.IsHidden();
        public bool IsAdmin() => profileService.IsAdmin();
        public List<Stock> GetUserStocks() => profileService.GetUserStocks();
        public void UpdateAdminMode(bool newIsAdmin)
        {
            profileService.UpdateIsAdmin(newIsAdmin);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
