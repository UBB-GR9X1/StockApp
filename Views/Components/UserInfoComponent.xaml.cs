namespace StockApp.Views.Components
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Src.Data;
    using Src.View.Pages;
    using StockApp.Models;

    public sealed partial class UserInfoComponent : Page
    {
        public User User;

        public UserInfoComponent()
        {
            this.InitializeComponent();
        }

        public void SetUserData(User userData)
        {
            User = userData;
            NameTextBlock.Text = $"{User.FirstName}  {User.LastName}";
            CNPTextBlock.Text = $"{User.Cnp}";
            ScoreTextBlock.Text = $"Score: {User.CreditScore}";
        }

        private async void OnAnalysisClick(object sender, RoutedEventArgs e)
        {
            if (User != null)
            {
                AnalysisWindow analysisWindow = new AnalysisWindow(User);
                analysisWindow.Activate();
            }
        }

        private async void OnTipHistoryClick(object seder, RoutedEventArgs e)
        {
            if (User != null)
            {
                DatabaseConnection dbConnection = new DatabaseConnection();
                TipHistoryWindow tipHistoryWindow = new TipHistoryWindow(User, new MessagesRepository(dbConnection), new TipsRepository(dbConnection));
                tipHistoryWindow.Activate();
            }
        }
    }
}
