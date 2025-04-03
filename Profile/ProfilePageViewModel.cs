﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;

namespace StockApp.Profile
{
    internal class ProfilePageViewModel : INotifyPropertyChanged
    {
        private ProfieServices profServ = ProfieServices.Instance;

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

        public ProfilePageViewModel()
        {
            LoadProfileImage();
        }

        private void LoadProfileImage()
        {
            string imageUrl = profServ.getImage();
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

        public string getUsername() => profServ.getUsername();
        public string getDescription() => profServ.getDescription();
        public bool isHidden() => profServ.isHidden();
        public bool isAdmin() => profServ.isAdmin();
        public List<string> getUserStocks() => profServ.getUserStocks();
        public string getPassword() => profServ.getPass();


        public void updateAdminMode(bool newIsAdmin)
        {
            profServ.updateIsAdmin(newIsAdmin);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
