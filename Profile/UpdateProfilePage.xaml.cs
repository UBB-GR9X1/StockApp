using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StocksApp.Services;

namespace StocksApp
{
    public sealed partial class UpdateProfilePage : Page
    {
        private ProfieServices profServ = ProfieServices.Instance;

        public UpdateProfilePage()
        {
            this.InitializeComponent();
        }

        private void GoToProfilePage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProfilePage));
        }

        private void GetAdminPassword(object sender, RoutedEventArgs e)
        {
            string userTryPass = PasswordTry.Text;
            if (profServ.getPass() == userTryPass)
            {
                profServ.updateIsAdmin(true);
                MyPopupAdmin.IsOpen = true;
            }
            else
            {
                profServ.updateIsAdmin(false);
                MyPopupAdminNOT.IsOpen = true;
            }
        }

        private void ClosePopupAdmin(object sender, RoutedEventArgs e)
        {
            MyPopupAdmin.IsOpen = false;
        }

        private void ClosePopupAdminNOT(object sender, RoutedEventArgs e)
        {
            MyPopupAdminNOT.IsOpen = false;
        }

        private void UpdateUserProfile(object sender, RoutedEventArgs e)
        {
            bool newHidden = MyCheckBox.IsChecked == true;
            string newUsername = UsernameInput.Text;
            string newImage = ImageInput.Text;
            string newDescription = DescriptionInput.Text;

            if (string.IsNullOrEmpty(newUsername) || string.IsNullOrEmpty(newImage) || string.IsNullOrEmpty(newDescription))
            {
                MyPopupUpdate.IsOpen = true;
            }
            else
            {
                if(newUsername.Length >= 8 && newUsername.Length <= 24 && newDescription.Length >= 0 && newDescription.Length <= 100)
                {
                    profServ.updateUser(newUsername, newImage, newDescription, newHidden);
                }
                else
                {
                    MyPopupUpdate.IsOpen = true;
                }

            }
        }

        private void ClosePopUpUpdate(object sender, RoutedEventArgs e)
        {
            MyPopupUpdate.IsOpen = false;
        }
    }
}
