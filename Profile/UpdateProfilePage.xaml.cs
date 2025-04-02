using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StockApp.Profile;
using System;
using System.Threading.Tasks;

namespace StocksApp
{
    public sealed partial class UpdateProfilePage : Page
    {
        private UpdateProfilePageViewModel viewModelUpdate = new UpdateProfilePageViewModel(); 

        public UpdateProfilePage()
        {
            this.InitializeComponent();
        }

        private void GoToProfilePage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProfilePage));
        }

        private async void GetAdminPassword(object sender, RoutedEventArgs e)
        {
            string userTryPass = PasswordTry.Text;
            bool isAdmin = viewModelUpdate.getPassword() == userTryPass;
            viewModelUpdate.updateAdminMode(isAdmin);

            string message = isAdmin ? "You are now ADMIN!" : "Incorrect Password!";
            string title = isAdmin ? "Success" : "Error";
            ContentDialog dialog = CreateDialog(title, message);
            await dialog.ShowAsync();
        }

        private async void UpdateUserProfile(object sender, RoutedEventArgs e)
        {
            bool newHidden = MyCheckBox.IsChecked == true;
            string newUsername = UsernameInput.Text;
            string newImage = ImageInput.Text;
            string newDescription = DescriptionInput.Text;

            if (string.IsNullOrEmpty(newUsername) && string.IsNullOrEmpty(newImage) && string.IsNullOrEmpty(newDescription))
            {
                await ShowErrorDialog("Please fill up all of the information fields");
                return;
            }

            if (newUsername.Length < 8 || newUsername.Length > 24)
            {
                await ShowErrorDialog("Username must be 8-24 characters long.");
                return;
            }

            if(newDescription.Length > 100)
            {
                await ShowErrorDialog("The description should be max 100 characters long.");
                return;
            }

            viewModelUpdate.updateAll(newUsername, newImage, newDescription, newHidden);
            await ShowSuccessDialog("Profile updated successfully!");
        }

        private async Task ShowErrorDialog(string message)
        {
            ContentDialog dialog = CreateDialog("Error", message);
            await dialog.ShowAsync();
        }

        private async Task ShowSuccessDialog(string message)
        {
            ContentDialog dialog = CreateDialog("Success", message);
            await dialog.ShowAsync();
        }

        private ContentDialog CreateDialog(string title, string message)
        {
            return new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };
        }
    }
}
