using System.Net.Http;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StockApp
{
    using Common.Services;
    using Common.Services.Impl;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.UI.Xaml;
    using StockApp.Pages;
    using StockApp.Services;
    using StockApp.ViewModels;
    using StockApp.Views;
    using StockApp.Views.Components;
    using StockApp.Views.Pages;
    using System;

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
                        .AddEnvironmentVariables();

                    var config = configBuilder.Build();
                    string apiBaseUrl = App.Configuration.GetValue<string>("ApiBase")
                        ?? throw new InvalidOperationException("API base URL is not configured");

                    services.AddSingleton<IConfiguration>(config);

                    services.AddSingleton<IAuthenticationService, AuthenticationService>();
                    // Other Services
                    // services.AddScoped<ITransactionLogService, TransactionLogService>();
                    // services.AddScoped<IBillSplitReportService, BillSplitReportService>();
                    // services.AddScoped<IChatReportService, ChatReportService>();
                    // services.AddScoped<ILoanRequestService, LoanRequestService>();
                    // services.AddScoped<ILoanService, LoanService>();
                    // services.AddScoped<IMessagesService, MessagesService>();
                    // services.AddScoped<ITipsService, TipsService>();
                    // services.AddScoped<IHistoryService, HistoryService>();
                    services.AddScoped<IStockPageService, StockPageProxyService>();
                    services.AddScoped<IInvestmentsService, InvestmentsProxyService>();
                    services.AddScoped<IUserService, UserProxyService>();
                    services.AddScoped<IActivityService, ActivityProxyService>();
                    services.AddScoped<IStoreService, StoreProxyService>();
                    services.AddScoped<INewsService, NewsProxyService>();
                    services.AddScoped<IStockService, StockProxyService>();
                    services.AddScoped<IAlertService, AlertProxyService>();
                    services.AddScoped<IProfanityChecker, ProfanityChecker>();

                    services.AddHttpClient<IStockService, StockProxyService>\(client =>\n                    {\n                        client.BaseAddress = new Uri\(apiBaseUrl\);\n                    }\).AddHttpMessageHandler<AuthenticationDelegatingHandler>\(\);
                    {
                        client.BaseAddress = new Uri(apiBaseUrl);
                    });
                    services.AddHttpClient<IInvestmentsService, InvestmentsProxyService>(client =>
                    {
                        client.BaseAddress = new Uri(apiBaseUrl);
                    });
                    services.AddHttpClient<IUserService, UserProxyService>(client =>
                    {
                        client.BaseAddress = new Uri(apiBaseUrl);
                    });
                    services.AddHttpClient<IActivityService, ActivityProxyService>(client =>
                    {
                        client.BaseAddress = new Uri(apiBaseUrl);
                    });
                    services.AddHttpClient<IStoreService, StoreProxyService>(client =>
                    {
                        client.BaseAddress = new Uri(apiBaseUrl);
                    });
                    services.AddHttpClient<INewsService, NewsProxyService>(client =>
                    {
                        client.BaseAddress = new Uri(apiBaseUrl);
                    });
                    services.AddHttpClient<IStockPageService, StockPageProxyService>(client =>
                    {
                        client.BaseAddress = new Uri(apiBaseUrl);
                    });
                    services.AddHttpClient<IAlertService, AlertProxyService>(client =>
                    {
                        client.BaseAddress = new Uri(apiBaseUrl);
                    });
                    services.AddHttpClient<IAuthenticationService, AuthenticationService>(client =>
                    {
                        client.BaseAddress = new Uri(apiBaseUrl);
                    });

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
                    services.AddTransient<LoginComponent>();

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
                    services.AddTransient<AuthenticationViewModel>();

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
                    services.AddTransient<LoginPage>();

                    // FIXME: remove \/\/\/\/
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
