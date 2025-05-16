// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StockApp
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.UI.Xaml;
    using StockApp.Pages;
    using StockApp.Repositories;
    using StockApp.Repositories.Api;
    using StockApp.Services;
    using StockApp.Services.Api;
    using StockApp.ViewModels;
    using StockApp.Views;
    using StockApp.Views.Components;
    using StockApp.Views.Pages;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow? MainAppWindow { get; private set; } = null!;

        public static IHost Host { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            // Note: We've replaced DatabaseHelper.InitializeDatabase() with EF Core migrations
            this.InitializeComponent();
            this.ConfigureHost();

            // explanation before the OnUnhandledException method
            // this.UnhandledException += this.OnUnhandledException;
        }

        private void ConfigureHost()
        {
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Build configuration
                    var configBuilder = new ConfigurationBuilder()
                        .AddUserSecrets<App>()
                        .AddEnvironmentVariables()
                        .AddInMemoryCollection(new Dictionary<string, string?> {
                            {
                            "ApiBaseUrl", "https://localhost:7001/"
                            },
                        });

                    var config = configBuilder.Build();
                    services.AddSingleton<IConfiguration>(config);

                    // Repositories
                    services.AddScoped<IAlertRepository, AlertProxyRepository>();
                    services.AddScoped<ITransactionRepository, TransactionProxyRepository>();
                    services.AddScoped<ITransactionLogService, TransactionLogService>();
                    services.AddScoped<IInvestmentsRepository, InvestmentsProxyRepository>();
                    services.AddScoped<IBillSplitReportRepository, BillSplitReportProxyRepository>();
                    services.AddScoped<ITransactionRepository, TransactionProxyRepository>();
                    services.AddScoped<IChatReportRepository, ChatReportProxyRepostiory>();
                    services.AddScoped<ILoanRepository, LoanProxyRepository>();
                    services.AddScoped<ILoanRequestRepository, LoanRequestProxyRepository>();
                    services.AddScoped<IActivityRepo, ActivityProxyRepository>();
                    services.AddScoped<IGemStoreRepository, GemStoreProxyRepo>();
                    services.AddScoped<IUserRepository, UserProxyRepository>();
                    services.AddScoped<IBaseStocksRepository, BaseStocksProxyRepository>();
                    services.AddScoped<IHistoryRepository, HistoryProxyRepository>();
                    services.AddScoped<IHomepageStocksRepository, HomepageStocksProxyRepository>();
                    services.AddScoped<ITipsRepository, TipsProxyRepository>();
                    services.AddScoped<IMessagesRepository, MessageProxyRepository>();
                    services.AddScoped<IChatReportRepository, ChatReportProxyRepostiory>();
                    services.AddScoped<INewsRepository, NewsProxyRepository>();
                    services.AddScoped<IStockRepository, StockProxyRepository>();

                    //FIXME: port \/\/\/\/
                    services.AddSingleton<IStockPageRepository, StockPageProxyRepository>();

                    // HttpClient for API communication
                    services.AddHttpClient<IChatReportRepository, ChatReportProxyRepostiory>(client =>
                    {
                        client.BaseAddress = new Uri(config["ApiBaseUrl"]);
                    });
                    services.AddHttpClient<IBillSplitReportRepository, BillSplitReportProxyRepository>((client) =>
                    {
                        client.BaseAddress = new Uri("https://localhost:7001/");
                    });
                    services.AddHttpClient<IAlertRepository, AlertProxyRepository>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost:7001/");
                    });

                    services.AddHttpClient<IActivityRepo, ActivityProxyRepository>(client =>
                    {
                        client.BaseAddress = new Uri(config["ApiBaseUrl"]);
                    });
                    services.AddHttpClient<IUserRepository, UserProxyRepository>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost:7001/");
                    });

                    services.AddHttpClient<IBaseStocksRepository, BaseStocksProxyRepository>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost:7001/");
                    });
                    services.AddHttpClient<IInvestmentsRepository, InvestmentsProxyRepository>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost:7001/");
                    });

                    services.AddHttpClient<IHomepageStocksRepository, HomepageStocksProxyRepository>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost:7001/");
                    });
                    services.AddHttpClient<ILoanRequestRepository, LoanRequestProxyRepository>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost:7001/");
                    });
                    services.AddHttpClient<ILoanRepository, LoanProxyRepository>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost:7001/");
                    });

                    services.AddHttpClient<IGemStoreRepository, GemStoreProxyRepo>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost:7001/");
                    });

                    services.AddHttpClient<IHistoryRepository, HistoryProxyRepository>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost:7001/");
                    });

                    services.AddHttpClient<ITransactionRepository, TransactionProxyRepository>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost:7001/");
                    });

                    services.AddHttpClient<ITipsRepository, TipsProxyRepository>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost:7001/");
                    });

                    services.AddHttpClient<IMessagesRepository, MessageProxyRepository>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost:7001/");
                    });

                    services.AddHttpClient<IChatReportRepository, ChatReportProxyRepostiory>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost:7001/");
                    });
                    services.AddHttpClient<INewsRepository, NewsProxyRepository>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost:7001/");
                    });
                    services.AddHttpClient<IStockRepository, StockProxyRepository>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost:7001/");
                    });
                    services.AddHttpClient<IStockPageRepository, StockPageProxyRepository>(client =>
                    {
                        client.BaseAddress = new Uri("https://localhost:7001/");
                    });

                    // Other Services
                    services.AddScoped<IBaseStocksService, BaseStocksService>();
                    services.AddScoped<IBillSplitReportService, BillSplitReportService>();
                    services.AddScoped<IChatReportService, ChatReportService>();
                    services.AddScoped<IHistoryService, HistoryService>();
                    services.AddScoped<IInvestmentsService, InvestmentsService>();
                    services.AddScoped<ILoanRequestService, LoanRequestService>();
                    services.AddScoped<ILoanService, LoanService>();
                    services.AddScoped<IMessagesService, MessagesService>();
                    services.AddScoped<ITipsService, TipsService>();
                    services.AddScoped<IUserService, UserService>();
                    services.AddScoped<IActivityService, ActivityService>();
                    services.AddScoped<IStoreService, StoreService>();
                    services.AddScoped<IHomepageService, HomepageService>();
                    services.AddScoped<IStockPageService, StockPageService>();
                    services.AddScoped<IHistoryService, HistoryService>();
                    services.AddScoped<INewsService, NewsService>();
                    services.AddScoped<IStockService, StockService>();
                    services.AddScoped<IAlertService, AlertService>();
                    services.AddSingleton<MainWindow>();

                    // UI Components
                    services.AddTransient<BillSplitReportViewModel>();
                    services.AddTransient<BillSplitReportViewModel>();
                    services.AddTransient<BillSplitReportComponent>();
                    services.AddTransient<BillSplitReportPage>();
                    services.AddTransient<ChatReportComponent>();
                    services.AddTransient<LoanRequestComponent>();
                    services.AddTransient<LoanComponent>();
                    services.AddTransient<BillSplitReportComponent>();
                    services.AddTransient<LoansView>();
                    services.AddTransient<LoanRequestView>();
                    services.AddTransient<UserInfoComponent>();
                    services.AddTransient<UsersView>();
                    services.AddTransient<UserProfileComponent>();
                    services.AddTransient<AlertsView>();

                    // ViewModels
                    services.AddTransient<StoreViewModel>();
                    services.AddTransient<ProfilePageViewModel>();
                    services.AddTransient<InvestmentsViewModel>();
                    services.AddTransient<HomepageViewModel>();
                    services.AddTransient<CreateStockViewModel>();
                    services.AddTransient<CreateProfilePageViewModel>();
                    services.AddTransient<TipHistoryViewModel>();
                    services.AddTransient<NewsDetailViewModel>();
                    services.AddTransient<NewsListViewModel>();
                    services.AddTransient<NewsListPage>();
                    services.AddTransient<ArticleCreationViewModel>();
                    services.AddTransient<AdminNewsViewModel>();
                    services.AddTransient<StockPageViewModel>();
                    services.AddTransient<TransactionLogViewModel>();
                    services.AddTransient<UserProfileComponentViewModel>();
                    services.AddTransient<AlertViewModel>();
                    services.AddTransient<UpdateProfilePageViewModel>();

                    // Pages
                    services.AddTransient<CreateStockPage>();
                    services.AddTransient<TransactionLogPage>();
                    services.AddTransient<ProfilePage>();
                    services.AddTransient<GemStoreWindow>();
                    services.AddTransient<CreateProfilePage>();
                    services.AddTransient<HomepageView>();
                    services.AddTransient<InvestmentsView>();
                    services.AddTransient<TipHistoryWindow>();
                    services.AddTransient<NewsArticleView>();
                    services.AddTransient<LoansViewModel>();
                    services.AddTransient<ChatReportView>();
                    services.AddTransient<ArticleCreationView>();
                    services.AddTransient<AdminNewsControlView>();
                    services.AddTransient<StockPage>();
                    services.AddTransient<UpdateProfilePage>();
                    //FIXME: remove \/\/\/\/
                    services.AddTransient<Func<LoanRequestComponent>>(sp => () => sp.GetRequiredService<LoanRequestComponent>());
                    services.AddTransient<Func<ChatReportComponent>>(sp => () => sp.GetRequiredService<ChatReportComponent>());
                    services.AddTransient<Func<UserInfoComponent>>(sp => () => sp.GetRequiredService<UserInfoComponent>());

                }).Build();
        }

        /// <summary>
        /// Gets or sets the current window of the application.
        /// </summary>
        public static Window CurrentWindow { get; set; } = null!;

        /// <summary>
        /// Gets Configuration object for the application.
        /// </summary>
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="launchActivatedEventArgs">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs launchActivatedEventArgs)
        {
            MainAppWindow = Host.Services.GetRequiredService<MainWindow>();
            MainAppWindow.Activate();
        }

        // i found some stupid ass error for the debugger, got it twice and couldn't
        // recreate it ever since thus this method exists if someone finds it there is something to see
        // i assume it's because how the db is or some shit, happens when you exit the app and it crashes the debugger
        // and opens up a new VS window for the project
        // NO IDEA HOW THAT HAPPENS
        // if you find it pls fix it (if it's in the news stuff cuz i fucked something up tell me and i'll fix it)
        private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            System.Diagnostics.Debug.WriteLine($"Unhandled exception: {unhandledExceptionEventArgs.Exception.Message}");
            System.Diagnostics.Debug.WriteLine(unhandledExceptionEventArgs.Exception.StackTrace);
        }
    }
}
