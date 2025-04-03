using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StockApp.StockPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StockPage : Page
    {
        private StockPageViewModel _viewModel;

        public StockPage()
        {
            this.InitializeComponent();
        }

        public void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        public void AuthorButton_Click(object sender, RoutedEventArgs e)
        {
            // this.Frame.Navigate(typeof(AuthorPage.AuthorPage));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Retrieve the stock name passed during navigation
            if (e.Parameter is string stockName)
            {
                _viewModel = new StockPageViewModel(stockName);
                this.DataContext = _viewModel;
            }
        }
    }
}
