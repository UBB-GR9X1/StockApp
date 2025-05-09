namespace StockApp
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using StockApp.Database;
    using StockApp.Pages;
    using StockApp.Repositories;
    using StockApp.Views;
    using StockApp.Views.Components;
    using StockApp.Views.Pages;

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly IServiceProvider serviceProvider;

        public Frame MainAppFrame => this.MainFrame;
        public bool CreateProfileButtonVisibility => IUserRepository.CurrentUserCNP == null;

        public static bool ProfileButtonVisibility => IUserRepository.CurrentUserCNP != null;

        public MainWindow(IServiceProvider serviceProvider)
        {
            this.InitializeComponent();
            DatabaseHelper.InitializeDatabase();
            this.serviceProvider = serviceProvider;

            this.MainFrame.Content = this.serviceProvider.GetRequiredService<HomepageView>();
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
                string invokedItemTag = args.SelectedItemContainer.Tag.ToString() ?? throw new InvalidOperationException("Tag cannot be null");

                this.MainFrame.Content = invokedItemTag switch
                {
                    "ChatReports" => this.serviceProvider.GetRequiredService<ChatReportView>(),
                    "LoanRequest" => this.serviceProvider.GetRequiredService<LoanRequestView>(),
                    "Loans" => this.serviceProvider.GetRequiredService<LoansView>(),
                    "UsersList" => this.serviceProvider.GetRequiredService<UsersView>(),
                    "BillSplitReports" => this.serviceProvider.GetRequiredService<BillSplitReportPage>(),
                    "Zodiac" => this.serviceProvider.GetRequiredService<ZodiacFeatureView>(),
                    "Investments" => this.serviceProvider.GetRequiredService<InvestmentsView>(),
                    "HomePage" => this.serviceProvider.GetRequiredService<HomepageView>(),
                    "NewsListPage" => this.serviceProvider.GetRequiredService<NewsListPage>(),
                    "CreateStockPage" => this.serviceProvider.GetRequiredService<CreateStockPage>(),
                    "TransactionLogPage" => this.serviceProvider.GetRequiredService<TransactionLogPage>(),
                    "ProfilePage" => this.serviceProvider.GetRequiredService<ProfilePage>(),
                    "CreateProfile" => this.serviceProvider.GetRequiredService<CreateProfilePage>(),
                    "GemStoreWindow" => this.serviceProvider.GetRequiredService<GemStoreWindow>(),
                    _ => throw new InvalidOperationException($"Unknown navigation item: {invokedItemTag}")
                };
            }
        }
    }
}