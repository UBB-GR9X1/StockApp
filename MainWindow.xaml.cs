namespace StockApp
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using StockApp.Database;
    using StockApp.Pages;
    using StockApp.Repositories;
    using StockApp.Services;
    using StockApp.Views;
    using StockApp.Views.Components;
    using StockApp.Views.Pages;

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly IServiceProvider serviceProvider;
        private static readonly IUserRepository userRepository = new UserRepository();

        public Frame MainAppFrame => this.MainFrame;
        public bool CreateProfileButtonVisibility => userRepository.CurrentUserCNP == null;

        public static bool ProfileButtonVisibility => userRepository.CurrentUserCNP != null;

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
                switch (invokedItemTag)
                {
                    case "ChatReports":
                        var chatReportsPage = this.serviceProvider.GetRequiredService<ChatReportView>();
                        this.MainFrame.Content = chatReportsPage;
                        break;
                    case "LoanRequest":
                        var loanRequestPage = this.serviceProvider.GetRequiredService<LoanRequestView>();
                        this.MainFrame.Content = loanRequestPage;
                        break;
                    case "Loans":
                        var loansPage = this.serviceProvider.GetRequiredService<LoansView>();
                        this.MainFrame.Content = loansPage;
                        break;
                    case "UsersList":
                        var usersView = this.serviceProvider.GetRequiredService<UsersView>();
                        this.MainFrame.Content = usersView;
                        break;
                    case "BillSplitReports":
                        var billSplitPage = this.serviceProvider.GetRequiredService<BillSplitReportPage>();
                        this.MainFrame.Content = billSplitPage;
                        break;
                    case "Zodiac":
                        this.ZodiacFeature(sender, null);
                        break;
                    case "Investments":
                        var investmentsView = this.serviceProvider.GetRequiredService<InvestmentsView>();
                        this.MainFrame.Content = investmentsView;
                        break;
                    case "HomePage":
                        var homePage = this.serviceProvider.GetRequiredService<HomepageView>();
                        this.MainFrame.Content = homePage;
                        break;
                    case "NewsListPage":
                        var newsListPage = this.serviceProvider.GetRequiredService<NewsListPage>();
                        this.MainFrame.Content = newsListPage;
                        break;
                    case "CreateStockPage":
                        var createStockPage = this.serviceProvider.GetRequiredService<CreateStockPage>();
                        this.MainFrame.Content = createStockPage;
                        break;
                    case "TransactionLogPage":
                        var transactionLogPage = this.serviceProvider.GetRequiredService<TransactionLogPage>();
                        this.MainFrame.Content = transactionLogPage;
                        break;
                    case "ProfilePage":
                        var profilePage = this.serviceProvider.GetRequiredService<ProfilePage>();
                        this.MainFrame.Content = profilePage;
                        break;
                    case "CreateProfile":
                        var createProfilePage = this.serviceProvider.GetRequiredService<CreateProfilePage>();
                        this.MainFrame.Content = createProfilePage;
                        break;
                    case "GemStoreWindow":
                        var gemStoreWindow = this.serviceProvider.GetRequiredService<GemStoreWindow>();
                        this.MainFrame.Content = gemStoreWindow;
                        break;
                }
            }
        }

        private void ZodiacFeature(object sender, RoutedEventArgs e)
        {
            UserRepository userRepository = new UserRepository();
            ZodiacService zodiacService = new ZodiacService(userRepository);

            zodiacService.CreditScoreModificationBasedOnJokeAndCoinFlipAsync();
            zodiacService.CreditScoreModificationBasedOnAttributeAndGravity();

            this.MainFrame.Navigate(typeof(ZodiacFeatureView));
        }
    }
}