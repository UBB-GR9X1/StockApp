namespace StockApp.Views.Components
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Src.Data;
    using StockApp.Models;
    using StockApp.Repositories;
    using StockApp.Views.Pages;

    public sealed partial class UserInfoComponent : Page
    {
        public User User;

        public UserInfoComponent()
        {
            this.InitializeComponent();
        }

        public void SetUserData(User userData)
        {
            this.User = userData;
            this.NameTextBlock.Text = $"{this.User.FirstName}  {this.User.LastName}";
            this.CNPTextBlock.Text = $"{this.User.CNP}";
            this.ScoreTextBlock.Text = $"Score: {this.User.CreditScore}";
        }

        private async void OnAnalysisClick(object sender, RoutedEventArgs e)
        {
            if (this.User != null)
            {
                AnalysisWindow analysisWindow = new AnalysisWindow(this.User);
                analysisWindow.Activate();
            }
        }

        private async void OnTipHistoryClick(object seder, RoutedEventArgs e)
        {
            if (this.User != null)
            {
                DatabaseConnection dbConnection = new DatabaseConnection();
                TipHistoryWindow tipHistoryWindow = new TipHistoryWindow(this.User, new MessagesRepository(dbConnection), new TipsRepository(dbConnection));
                tipHistoryWindow.Activate();
            }
        }
    }
}
