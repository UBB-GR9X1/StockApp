using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;
using Model;
using StockApp.Database;
using StockApp.Repository;

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

            CheckAndHandleAlerts();

            //rootFrame.Navigate(typeof(CreateStockPage), null);
            // rootFrame.Navigate(typeof(ProfilePage), null);
            // rootFrame.Navigate(typeof(MainPage), null);

            //string stockName = "Tesla";
            //rootFrame.Navigate(typeof(StockPage.StockPage), stockName);  

            // rootFrame.Navigate(typeof(CreateStockPage.MainPage), null);

            rootFrame.Navigate(typeof(StocksHomepage.MainPage), null);

            // string stockName = "Tesla";

            // NavigationService.Instance.Initialize(rootFrame);
            // NavigationService.Instance.Navigate(typeof(StockPage.StockPage), stockName);
            //rootFrame.Navigate(typeof(StockPage.StockPage), stockName);  

            // rootFrame.Navigate(typeof(CreateStockPage.MainPage), null);

            //rootFrame.Navigate(typeof(StocksHomepage.MainPage), null);

            // rootFrame.Navigate(typeof(Test.TestPage), null);


            // <news>
            // NavigationService.Instance.Initialize(rootFrame);
            // NavigationService.Instance.Navigate(typeof(NewsListView));

            // GEM STORE:
            //rootFrame.Navigate(typeof(GemStore.GemStoreWindow), null);

            // TRANSACTION LOG:
            //rootFrame.Navigate(typeof(TransactionLog.TransactionLogView), null);

            // Alerts
            //rootFrame.Navigate(typeof(Alerts.AlertWindow), null);
        }
        private void CheckAndHandleAlerts()
        {
            var alertRepository = new AlertRepository();
            var triggeredAlerts = alertRepository.GetTriggeredAlerts();

            if (triggeredAlerts.Count > 0)
            {
                DisplayTriggeredAlerts(triggeredAlerts);
            }
            else
            {
                rootFrame.Navigate(typeof(StocksHomepage.MainPage), null);
            }
        }
        private void DisplayTriggeredAlerts(List<TriggeredAlert> triggeredAlerts)
        {
            rootFrame.Navigate(typeof(Alerts.AlertWindow), triggeredAlerts);
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page: " + e.SourcePageType.FullName);
        }
    }
}