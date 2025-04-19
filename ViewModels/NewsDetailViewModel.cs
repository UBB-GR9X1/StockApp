using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using StockApp.Commands;
using StockApp.Models;
using StockApp.Services;

namespace StockApp.ViewModels
{
    /// <summary>
    /// ViewModel for displaying details of a news article, handling loading,
    /// preview mode, and administrative actions such as approval, rejection, and deletion.
    /// </summary>
    public class NewsDetailViewModel : ViewModelBase
    {
        private readonly INewsService newsService;
        private readonly IDispatcher dispatcherQueue;
        private string currentArticleId;
        private bool isPreviewMode;
        private string previewId;

        private NewsArticle article;
        private bool isLoading;
        private bool hasRelatedStocks;
        private bool isAdminPreview;
        private string articleStatus;
        private bool canApprove;
        private bool canReject;

        /// <summary>
        /// Gets or sets the currently displayed <see cref="NewsArticle"/>.
        /// </summary>
        public NewsArticle Article
        {
            get => article;
            set
            {
                // Inline comment: log debug info whenever the article property is set
                System.Diagnostics.Debug.WriteLine($"Setting Article: Title={value?.Title}, Content Length={value?.Content?.Length ?? 0}");
                SetProperty(ref article, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the article is currently loading.
        /// </summary>
        public bool IsLoading
        {
            get => isLoading;
            set
            {
                // Inline comment: log loading state changes for diagnostics
                System.Diagnostics.Debug.WriteLine($"Setting IsLoading: {value}");
                SetProperty(ref isLoading, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the article has related stocks to display.
        /// </summary>
        public bool HasRelatedStocks
        {
            get => hasRelatedStocks;
            set => SetProperty(ref hasRelatedStocks, value);
        }

        /// <summary>
        /// Gets or sets a value indicating if the view is in admin preview mode.
        /// </summary>
        public bool IsAdminPreview
        {
            get => isAdminPreview;
            set => SetProperty(ref isAdminPreview, value);
        }

        /// <summary>
        /// Gets or sets the current status of the article (e.g., "Pending", "Approved").
        /// </summary>
        public string ArticleStatus
        {
            get => articleStatus;
            set => SetProperty(ref articleStatus, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Approve action is available.
        /// </summary>
        public bool CanApprove
        {
            get => canApprove;
            set => SetProperty(ref canApprove, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Reject action is available.
        /// </summary>
        public bool CanReject
        {
            get => canReject;
            set => SetProperty(ref canReject, value);
        }

        /// <summary>
        /// Command to navigate back to the previous view.
        /// </summary>
        public ICommand BackCommand { get; }

        /// <summary>
        /// Command to approve the current article.
        /// </summary>
        public ICommand ApproveCommand { get; }

        /// <summary>
        /// Command to reject the current article.
        /// </summary>
        public ICommand RejectCommand { get; }

        /// <summary>
        /// Command to delete the current article.
        /// </summary>
        public ICommand DeleteCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewsDetailViewModel"/> class with the specified dependencies.
        /// </summary>
        /// <param name="newsService">Service for retrieving and modifying news articles.</param>
        /// <param name="dispatcher">Dispatcher for UI thread operations.</param>
        public NewsDetailViewModel(INewsService newsService, IDispatcher dispatcher)
        {
            this.newsService = newsService ?? throw new ArgumentNullException(nameof(newsService));
            this.dispatcherQueue = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            BackCommand = new StockNewsRelayCommand(() => NavigationService.Instance.GoBack());
            ApproveCommand = new StockNewsRelayCommand(async () => await ApproveArticleAsync());
            RejectCommand = new StockNewsRelayCommand(async () => await RejectArticleAsync());
            DeleteCommand = new StockNewsRelayCommand(async () => await DeleteArticleAsync());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewsDetailViewModel"/> class with default implementations.
        /// </summary>
        public NewsDetailViewModel()
          : this(new NewsService(), new DispatcherAdapter()) { }

        /// <summary>
        /// Loads the article with the given identifier, handling both preview and regular modes.
        /// </summary>
        /// <param name="articleId">The ID or preview token of the article to load.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="articleId"/> is null or empty.</exception>
        public async void LoadArticle(string articleId)
        {
            if (string.IsNullOrWhiteSpace(articleId))
            {
                System.Diagnostics.Debug.WriteLine("LoadArticle: ArticleId is null or empty");
                throw new ArgumentNullException(nameof(articleId));
            }

            IsLoading = true;

            try
            {
                System.Diagnostics.Debug.WriteLine($"LoadArticle: Loading article with ID: {articleId}");

                // Determine if this is a preview mode request
                if (articleId.StartsWith("preview:"))
                {
                    isPreviewMode = true;
                    previewId = articleId.Substring(8); // Inline: strip "preview:" prefix
                    currentArticleId = previewId;
                    IsAdminPreview = true;

                    var userArticle = newsService.GetUserArticleForPreview(previewId);
                    if (userArticle != null)
                    {
                        ArticleStatus = userArticle.Status;
                        CanApprove = userArticle.Status != "Approved";
                        CanReject = userArticle.Status != "Rejected";
                    }

                    var article = await newsService.GetNewsArticleByIdAsync(articleId);

                    dispatcherQueue.TryEnqueue(() =>
                    {
                        if (article != null)
                        {
                            Article = article;
                            HasRelatedStocks = article.RelatedStocks?.Any() == true; // Inline: check for related stocks
                        }
                        else
                        {
                            // FIXME: Provide fallback Article for missing preview
                            Article = new NewsArticle
                            {
                                Title = "Article Not Found",
                                Summary = "The requested preview article could not be found.",
                                Content = "This preview is unavailable."
                            };
                            HasRelatedStocks = false;
                        }

                        IsLoading = false;
                    });
                }
                else
                {
                    isPreviewMode = false;
                    currentArticleId = articleId;
                    IsAdminPreview = false;

                    var regularArticle = await newsService.GetNewsArticleByIdAsync(articleId);

                    if (regularArticle != null)
                    {
                        Article = regularArticle;
                        HasRelatedStocks = regularArticle.RelatedStocks?.Any() == true;

                        // Inline: mark article as read once loaded
                        try
                        {
                            await newsService.MarkArticleAsReadAsync(articleId);
                        }
                        catch (Exception ex)
                        {
                            // Inline: log failure to mark as read
                            System.Diagnostics.Debug.WriteLine($"Error marking article as read: {ex.Message}");
                        }
                    }
                    else
                    {
                        // Provide fallback for missing article
                        Article = new NewsArticle
                        {
                            Title = "Article Not Found",
                            Summary = "The requested article could not be found.",
                            Content = "This article may have been removed."
                        };
                        HasRelatedStocks = false;
                    }

                    IsLoading = false;
                }
            }
            catch (Exception ex)
            {
                // Inline: log unexpected errors during load
                System.Diagnostics.Debug.WriteLine($"Error loading article: {ex.Message}");
                dispatcherQueue.TryEnqueue(() =>
                {
                    Article = new NewsArticle
                    {
                        Title = "Error Loading Article",
                        Summary = "There was an error loading the article.",
                        Content = $"Error details: {ex.Message}\nPlease try again later."
                    };
                    HasRelatedStocks = false;
                    IsLoading = false;
                });
            }
        }

        /// <summary>
        /// Approves the preview article and navigates back on success.
        /// </summary>
        private async Task ApproveArticleAsync()
        {
            if (!isPreviewMode || string.IsNullOrEmpty(previewId))
                return;

            IsLoading = true;

            try
            {
                var success = await newsService.ApproveUserArticleAsync(previewId);
                if (success)
                {
                    ArticleStatus = "Approved";
                    CanApprove = false;
                    CanReject = true;

                    var dialog = new ContentDialog
                    {
                        Title = "Success",
                        Content = "Article has been approved.",
                        CloseButtonText = "OK",
                        XamlRoot = App.CurrentWindow.Content.XamlRoot
                    };

                    await dialog.ShowAsync();
                    NavigationService.Instance.GoBack();
                }
            }
            catch (Exception ex)
            {
                // Inline: show error on approval failure
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Failed to approve article. Please try again.",
                    CloseButtonText = "OK",
                    XamlRoot = App.CurrentWindow.Content.XamlRoot
                };
                await dialog.ShowAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Rejects the preview article and navigates back on success.
        /// </summary>
        private async Task RejectArticleAsync()
        {
            if (!isPreviewMode || string.IsNullOrEmpty(previewId))
                return;

            IsLoading = true;

            try
            {
                var success = await newsService.RejectUserArticleAsync(previewId);
                if (success)
                {
                    ArticleStatus = "Rejected";
                    CanApprove = true;
                    CanReject = false;

                    var dialog = new ContentDialog
                    {
                        Title = "Success",
                        Content = "Article has been rejected.",
                        CloseButtonText = "OK",
                        XamlRoot = App.CurrentWindow.Content.XamlRoot
                    };
                    await dialog.ShowAsync();
                    NavigationService.Instance.GoBack();
                }
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Failed to reject article. Please try again.",
                    CloseButtonText = "OK",
                    XamlRoot = App.CurrentWindow.Content.XamlRoot
                };
                await dialog.ShowAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Deletes the preview article after confirmation.
        /// </summary>
        private async Task DeleteArticleAsync()
        {
            if (!isPreviewMode || string.IsNullOrEmpty(previewId))
                return;

            try
            {
                var confirmDialog = new ContentDialog
                {
                    Title = "Confirm Deletion",
                    Content = "Are you sure you want to delete this article?",
                    PrimaryButtonText = "Delete",
                    CloseButtonText = "Cancel",
                    XamlRoot = App.CurrentWindow.Content.XamlRoot
                };

                var result = await confirmDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    IsLoading = true;

                    var success = await newsService.DeleteUserArticleAsync(previewId);
                    if (success)
                    {
                        var dialog = new ContentDialog
                        {
                            Title = "Success",
                            Content = "Article has been deleted.",
                            CloseButtonText = "OK",
                            XamlRoot = App.CurrentWindow.Content.XamlRoot
                        };
                        await dialog.ShowAsync();
                        NavigationService.Instance.GoBack();
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Failed to delete article. Please try again.",
                    CloseButtonText = "OK",
                    XamlRoot = App.CurrentWindow.Content.XamlRoot
                };
                await dialog.ShowAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <inheritdoc/>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
