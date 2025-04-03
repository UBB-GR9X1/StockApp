using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StockNewsPage.ViewModels;

namespace StockNewsPage.Views
{
    public sealed partial class AdminNewsControlView : Page
    {
        public AdminNewsViewModel ViewModel { get; } = new AdminNewsViewModel();

        public AdminNewsControlView()
        {
            this.InitializeComponent();
            this.Loaded += AdminNewsControlView_Loaded;

            ArticlesList.DataContext = ViewModel;
        }

        private void AdminNewsControlView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Initialize();
        }
    }
}