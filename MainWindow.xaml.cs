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
using StockApp.Database;
using StockApp.Model;
using StockApp.Repositories;
using CreateStock;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StockApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            DatabaseHelper.InitializeDatabase();

            rootFrame.Navigate(typeof(CreateStockPage), null);

            // rootFrame.Navigate(typeof(StocksHomepage.MainPage), null);
            // rootFrame.Navigate(typeof(Test.TestPage), null);

            // GEM STORE:
            //rootFrame.Navigate(typeof(GemStore.GemStoreWindow), null);

        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page: " + e.SourcePageType.FullName);
        }
    }
}
