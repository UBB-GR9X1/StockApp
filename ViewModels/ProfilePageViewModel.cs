namespace StockApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Microsoft.UI.Xaml.Media.Imaging;
    using StockApp.Services;

    public class ProfilePageViewModel : INotifyPropertyChanged
    {
        private ProfileServices profServ;

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
            profServ = new ProfileServices(authorCnp);
            LoadProfileImage();
        }

        private void LoadProfileImage()
        {
            string imageUrl = profServ.GetImage();
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
            return profServ.GetLoggedInUserCnp();
        }

        public string GetUsername() => profServ.GetUsername();
        public string GetDescription() => profServ.GetDescription();
        public bool IsHidden() => profServ.IsHidden();
        public bool IsAdmin() => profServ.IsAdmin();
        public List<string> GetUserStocks() => profServ.GetUserStocks();
        public string GetPassword() => profServ.GetPass();

        public string ExtractMyStockName(string fullStock) => profServ.ExtractStockName(fullStock);


        public void UpdateAdminMode(bool newIsAdmin)
        {
            profServ.UpdateIsAdmin(newIsAdmin);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
