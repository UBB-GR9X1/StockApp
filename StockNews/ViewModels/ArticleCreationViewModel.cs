using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using StockNewsPage.Models;
using StockNewsPage.Services;
using StockNewsPage.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using StockApp;

namespace StockNewsPage.ViewModels
{
    public class ArticleCreationViewModel : ViewModelBase
    {
        private readonly NewsService _newsService;
        private readonly DispatcherQueue _dispatcherQueue;
        private readonly AppState _appState;

        // properties
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _summary;
        public string Summary
        {
            get => _summary;
            set => SetProperty(ref _summary, value);
        }

        private string _content;
        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        private ObservableCollection<string> _topics = new();
        public ObservableCollection<string> Topics
        {
            get => _topics;
            set => SetProperty(ref _topics, value);
        }

        private string _selectedTopic;
        public string SelectedTopic
        {
            get => _selectedTopic;
            set => SetProperty(ref _selectedTopic, value);
        }

        private string _relatedStocksText;
        public string RelatedStocksText
        {
            get => _relatedStocksText;
            set => SetProperty(ref _relatedStocksText, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private bool _hasError;
        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                HasError = !string.IsNullOrEmpty(value);
            }
        }

        // commands
        public ICommand BackCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand PreviewCommand { get; }
        public ICommand SubmitCommand { get; }

        // constructor
        public ArticleCreationViewModel()
        {
            _newsService = new NewsService();
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            _appState = AppState.Instance;

            // init commands
            BackCommand = new RelayCommand(() => NavigationService.Instance.GoBack());
            ClearCommand = new RelayCommand(() => ClearForm());
            PreviewCommand = new RelayCommand(async () => await PreviewArticleAsync());
            SubmitCommand = new RelayCommand(async () => await SubmitArticleAsync());

            // init topics
            Topics.Add("Stock News");
            Topics.Add("Company News");
            Topics.Add("Market Analysis");
            Topics.Add("Economic News");
            Topics.Add("Functionality News");

            // set default values
            _selectedTopic = "Stock News";
        }

        // methods
        public void Initialize()
        {
            // check if user is logged in
            if (_appState.CurrentUser == null)
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
                    Summary = Summary,
                    Content = Content,
                    Source = $"User: {_appState.CurrentUser?.Username ?? "Anonymous"}",
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
                    Summary = Summary,
                    Content = Content,
                    Author = _appState.CurrentUser?.Username ?? "Anonymous",
                    SubmissionDate = DateTime.Now,
                    Status = "Preview",
                    Topic = SelectedTopic,
                    RelatedStocks = ParseRelatedStocks()
                };
                // store preview in service
                _newsService.StorePreviewArticle(article, userArticle);

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

            try
            {
                IsLoading = true;

                // create user article
                var userArticle = new UserArticle
                {
                    ArticleId = Guid.NewGuid().ToString(),
                    Title = Title,
                    Summary = Summary,
                    Content = Content,
                    Author = _appState.CurrentUser?.Username ?? "Anonymous",
                    SubmissionDate = DateTime.Now,
                    Status = "Pending",
                    Topic = SelectedTopic,
                    RelatedStocks = ParseRelatedStocks()
                };

                // submit article
                var success = await _newsService.SubmitUserArticleAsync(userArticle);

                if (success)
                {
                    // success message
                    var dialog = new ContentDialog
                    {
                        Title = "Success",
                        Content = "Your article has been submitted for review.",
                        CloseButtonText = "OK",
                        XamlRoot = App.CurrentWindow.Content.XamlRoot
                    };

                    await dialog.ShowAsync();

                    // clear and navigate back
                    ClearForm();
                    NavigationService.Instance.GoBack();
                }
                else
                {
                    ErrorMessage = "Failed to submit article. Please try again.";
                }
            }
            catch (UnauthorizedAccessException)
            {
                ErrorMessage = "You must be logged in to submit an article.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error submitting article: {ex.Message}");
                ErrorMessage = "An error occurred while trying to submit the article.";
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
                Source = $"User: {_appState.CurrentUser?.Username ?? "Anonymous"}",
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

