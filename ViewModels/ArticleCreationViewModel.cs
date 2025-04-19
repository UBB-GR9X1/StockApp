namespace StockApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Microsoft.UI.Dispatching;
    using Microsoft.UI.Xaml.Controls;
    using StockApp;
    using StockApp.Commands;
    using StockApp.Models;
    using StockApp.Repositories;
    using StockApp.Services;
    using StockApp.Views;

    public class ArticleCreationViewModel : ViewModelBase
    {
        private readonly INewsService newsService;
        private readonly IDispatcher dispatcherQueue;
        private readonly IAppState appState;
        private readonly IBaseStocksRepository stocksRepository;

        // properties
        private string title;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        private string summary;
        public string Summary
        {
            get => summary;
            set => SetProperty(ref summary, value);
        }

        private string content;
        public string Content
        {
            get => content;
            set => SetProperty(ref content, value);
        }

        private ObservableCollection<string> topics = new();
        public ObservableCollection<string> Topics
        {
            get => topics;
            set => SetProperty(ref topics, value);
        }

        private string selectedTopic;
        public string SelectedTopic
        {
            get => selectedTopic;
            set => SetProperty(ref selectedTopic, value);
        }

        private string relatedStocksText;
        public string RelatedStocksText
        {
            get => relatedStocksText;
            set => SetProperty(ref relatedStocksText, value);
        }

        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        private bool hasError;
        public bool HasError
        {
            get => hasError;
            set => SetProperty(ref hasError, value);
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                SetProperty(ref errorMessage, value);
                HasError = !string.IsNullOrEmpty(value);
            }
        }

        // commands
        public ICommand BackCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand PreviewCommand { get; }
        public ICommand SubmitCommand { get; }

        // constructor
        public ArticleCreationViewModel(
            INewsService newsService,
            IDispatcher dispatcher,
            IAppState appState,
            IBaseStocksRepository stocksRepo)
        {
            this.newsService = newsService;
            this.dispatcherQueue = dispatcher;
            this.appState = appState;
            this.stocksRepository = stocksRepo;

            // init commands
            BackCommand = new StockNewsRelayCommand(() => NavigationService.Instance.GoBack());
            ClearCommand = new StockNewsRelayCommand(() => ClearForm());
            PreviewCommand = new StockNewsRelayCommand(async () => await PreviewArticleAsync());
            SubmitCommand = new StockNewsRelayCommand(async () => await SubmitArticleAsync());

            // init topics
            Topics.Add("Stock News");
            Topics.Add("Company News");
            Topics.Add("Market Analysis");
            Topics.Add("Economic News");
            Topics.Add("Functionality News");

            // set default values
            selectedTopic = "Stock News";
        }

        public ArticleCreationViewModel()
          : this(new NewsService(),
                 new DispatcherAdapter(),
                 AppState.Instance,
                 new BaseStocksRepository())
        { }

        // methods
        public void Initialize()
        {
            // check if user is logged in
            if (appState.CurrentUser == null)
            {
                // error and navigate back
                ShowErrorDialog("You must be logged in to create an article.");
                NavigationService.Instance.GoBack();
            }

            ClearForm();
        }

        private void ClearForm()
        {
            Title = string.Empty;
            Summary = string.Empty;
            Content = string.Empty;
            SelectedTopic = "Stock News";
            RelatedStocksText = string.Empty;
            ErrorMessage = string.Empty;
        }

        private async Task PreviewArticleAsync()
        {
            if (!ValidateForm())
                return;

            try
            {
                IsLoading = true;

                // create preview article
                var previewId = Guid.NewGuid().ToString();

                var article = new NewsArticle
                {
                    ArticleId = previewId,
                    Title = Title,
                    Summary = Summary ?? "",
                    Content = Content,
                    Source = $"User: {appState.CurrentUser?.CNP ?? "Anonymous"}",
                    PublishedDate = DateTime.Now.ToString("MMMM dd, yyyy"),
                    IsRead = false,
                    IsWatchlistRelated = false,
                    Category = SelectedTopic,
                    RelatedStocks = ParseRelatedStocks()
                };

                // create temp user article
                var userArticle = new UserArticle
                {
                    ArticleId = previewId,
                    Title = Title,
                    Summary = Summary ?? "",
                    Content = Content,
                    Author = appState.CurrentUser ?? throw new InvalidOperationException("User not found"),
                    SubmissionDate = DateTime.Now,
                    Status = "Preview",
                    Topic = SelectedTopic,
                    RelatedStocks = ParseRelatedStocks()
                };

                // store preview in service
                newsService.StorePreviewArticle(article, userArticle);

                // nav to preview
                NavigationService.Instance.Navigate(typeof(NewsArticleView), $"preview:{previewId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error previewing article: {ex.Message}");
                ErrorMessage = "An error occurred while trying to preview the article.";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SubmitArticleAsync()
        {
            if (!ValidateForm())
                return;

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                // parse and validate related stocks before submitting
                List<string> relatedStocks = ParseRelatedStocks();

                // validate stocks and show dialog if needed
                bool continueSubmission = await ValidateStocksAsync(relatedStocks);
                if (!continueSubmission)
                {
                    IsLoading = false;
                    return;
                }

                var article = new UserArticle
                {
                    ArticleId = Guid.NewGuid().ToString(),
                    Title = Title,
                    Summary = Summary,
                    Content = Content,
                    Author = appState.CurrentUser ?? throw new InvalidOperationException("User not found"),
                    SubmissionDate = DateTime.Now,
                    Status = "Pending",
                    Topic = SelectedTopic,
                    RelatedStocks = relatedStocks
                };

                bool success = await newsService.SubmitUserArticleAsync(article);

                if (success)
                {
                    dispatcherQueue.TryEnqueue(async () =>
                    {
                        var dialog = new ContentDialog
                        {
                            Title = "Success",
                            Content = "Your article has been submitted for review.",
                            CloseButtonText = "OK",
                            XamlRoot = App.CurrentWindow.Content.XamlRoot
                        };

                        await dialog.ShowAsync();
                        ClearForm();
                        NavigationService.Instance.GoBack();
                    });
                }
                else
                {
                    ShowErrorDialog("Failed to submit article. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                ShowErrorDialog($"An error occurred: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool ValidateForm()
        {
            ErrorMessage = string.Empty;

            // validate title
            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Title is required.";
                return false;
            }

            // validate summary
            if (string.IsNullOrWhiteSpace(Summary))
            {
                ErrorMessage = "Summary is required.";
                return false;
            }

            // validate content
            if (string.IsNullOrWhiteSpace(Content))
            {
                ErrorMessage = "Content is required.";
                return false;
            }

            // validate topic
            if (string.IsNullOrWhiteSpace(SelectedTopic))
            {
                ErrorMessage = "Topic is required.";
                return false;
            }

            return true;
        }

        private async Task<bool> ValidateStocksAsync(List<string> enteredStocks)
        {
            if (enteredStocks == null)
                throw new ArgumentNullException(nameof(enteredStocks));

            if (enteredStocks.Count == 0)
                return true;

            var allStocks = stocksRepository.GetAllStocks()
                ?? throw new InvalidOperationException("Stocks repository returned null");

            // check if all entered stocks exist (by name or symbol)
            var invalidStocks = new List<string>();
            foreach (var stock in enteredStocks)
            {
                if (string.IsNullOrWhiteSpace(stock))
                    throw new ArgumentException("Stock name cannot be null or whitespace", nameof(enteredStocks));

                bool stockExists = allStocks.Any(s =>
                    s.Name.Equals(stock, StringComparison.OrdinalIgnoreCase) ||
                    s.Symbol.Equals(stock, StringComparison.OrdinalIgnoreCase));

                if (!stockExists)
                {
                    invalidStocks.Add(stock);
                }
            }

            if (invalidStocks.Count > 0)
            {
                // a string of available stocks for the dialog
                var availableStocksList = allStocks
                    .OrderBy(s => s.Name)
                    .Take(20) // first 20 to avoid large dialog
                    .Select(s => $"• {s.Name} ({s.Symbol})")
                    .ToList();

                string invalidStocksMessage = string.Join(", ", invalidStocks);
                string availableStocksMessage = string.Join("\n", availableStocksList);

                if (allStocks.Count > 20)
                {
                    availableStocksMessage += $"\n(and {allStocks.Count - 20} more...)";
                }

                string message = $"The following stocks are not found in our database: {invalidStocksMessage}\n\n" +
                    $"Would you like to continue anyway? We'll create these stocks automatically.\n\n" +
                    $"Available stocks include:\n{availableStocksMessage}";

                var dialog = new ContentDialog
                {
                    Title = "Stock Validation",
                    Content = message,
                    PrimaryButtonText = "Continue Anyway",
                    CloseButtonText = "Cancel",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = App.CurrentWindow.Content.XamlRoot
                };

                var result = await dialog.ShowAsync();
                return result == ContentDialogResult.Primary;
            }

            return true;
        }

        private List<string> ParseRelatedStocks()
        {
            if (string.IsNullOrWhiteSpace(RelatedStocksText))
                return new List<string>();

            return RelatedStocksText
                .Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }

        private NewsArticle CreateArticle()
        {
            return new NewsArticle
            {
                ArticleId = Guid.NewGuid().ToString(),
                Title = Title,
                Summary = Summary,
                Content = Content,
                Source = $"User: {appState.CurrentUser?.CNP ?? "Anonymous"}",
                PublishedDate = DateTime.Now.ToString("MMMM dd, yyyy"),
                IsRead = false,
                IsWatchlistRelated = false,
                Category = SelectedTopic,
                RelatedStocks = ParseRelatedStocks()
            };
        }

        private async void ShowErrorDialog(string message)
        {
            try
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = message,
                    CloseButtonText = "OK",
                    XamlRoot = App.CurrentWindow.Content.XamlRoot
                };

                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing error dialog: {ex.Message}");
            }
        }
    }
}

