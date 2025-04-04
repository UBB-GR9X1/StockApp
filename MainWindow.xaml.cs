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
using StockNewsPage.Services;
using StockNewsPage.Views;

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

            //rootFrame.Navigate(typeof(CreateStockPage), null);
            // rootFrame.Navigate(typeof(ProfilePage), null);
            // rootFrame.Navigate(typeof(MainPage), null);

            // string stockName = "Tesla";
            // rootFrame.Navigate(typeof(StockPage.StockPage), stockName);  

            // rootFrame.Navigate(typeof(CreateStockPage.MainPage), null);

            rootFrame.Navigate(typeof(StocksHomepage.MainPage), null);

            // string stockName = "stock1";

            // rootFrame.Navigate(typeof(StockPage.StockPage), stockName);  

            // rootFrame.Navigate(typeof(CreateStockPage.MainPage), null);

            // rootFrame.Navigate(typeof(StocksHomepage.MainPage), null);

            // rootFrame.Navigate(typeof(Test.TestPage), null);


            // <news>
            // NavigationService.Instance.Initialize(rootFrame);
            // NavigationService.Instance.Navigate(typeof(NewsListView));

            // GEM STORE:
            //rootFrame.Navigate(typeof(GemStore.GemStoreWindow), null);

            // TRANSACTION LOG:
            //rootFrame.Navigate(typeof(TransactionLog.TransactionLogView), null);
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page: " + e.SourcePageType.FullName);
        }
    }
}