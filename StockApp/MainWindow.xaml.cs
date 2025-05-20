namespace StockApp
{
    using Common.Services;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using StockApp.Pages;
    using StockApp.Views;
    using StockApp.Views.Pages;
    using System;

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IAuthenticationService authenticationService;

        public Frame MainAppFrame => this.MainFrame;

        public bool LoginButtonVisibility { get; set; }

        public bool ProfileButtonVisibility { get; set; }

        public MainWindow(IServiceProvider serviceProvider)
        {
            this.InitializeComponent();
            this.serviceProvider = serviceProvider;

            this.MainFrame.Content = this.serviceProvider.GetRequiredService<HomepageView>();
            this.authenticationService = this.serviceProvider.GetRequiredService<IAuthenticationService>();
            this.authenticationService.UserLoggedIn += this.AuthenticationService_UserLoggedIn;
            this.authenticationService.UserLoggedOut += this.AuthenticationService_UserLoggedOut;
            if (this.authenticationService.IsUserLoggedIn())
            {
                this.LoginButtonVisibility = false;
                this.ProfileButtonVisibility = true;
            }
            else
            {
                this.LoginButtonVisibility = true;
                this.ProfileButtonVisibility = false;
            }
        }

        private void AuthenticationService_UserLoggedIn(object? sender, UserLoggedInEventArgs e)
        {
            this.LoginButtonVisibility = false;
            this.ProfileButtonVisibility = true;
        }

        private void AuthenticationService_UserLoggedOut(object? sender, UserLoggedOutEventArgs e)
        {
            this.LoginButtonVisibility = true;
            this.ProfileButtonVisibility = false;
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
                    "Investments" => this.serviceProvider.GetRequiredService<InvestmentsView>(),
                    "HomePage" => this.serviceProvider.GetRequiredService<HomepageView>(),
                    "NewsListPage" => this.serviceProvider.GetRequiredService<NewsListPage>(),
                    "CreateStockPage" => this.serviceProvider.GetRequiredService<CreateStockPage>(),
                    "TransactionLogPage" => this.serviceProvider.GetRequiredService<TransactionLogPage>(),
                    "ProfilePage" => this.serviceProvider.GetRequiredService<ProfilePage>(),
                    "LoginPage" => this.serviceProvider.GetRequiredService<LoginPage>(),
                    "GemStoreWindow" => this.serviceProvider.GetRequiredService<GemStoreWindow>(),
                    _ => throw new InvalidOperationException($"Unknown navigation item: {invokedItemTag}")
                };
            }
        }
    }
}