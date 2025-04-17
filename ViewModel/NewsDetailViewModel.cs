namespace StockApp.ViewModel
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Microsoft.UI.Dispatching;
    using Microsoft.UI.Xaml.Controls;
    using StockApp.Command;
    using StockApp.Models;
    using StockApp.Service;

    public class NewsDetailViewModel : ViewModelBase
    {
        private readonly NewsService newsService;
        private readonly DispatcherQueue dispatcherQueue;
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

        public NewsArticle Article
        {
            get => article;
            set => SetProperty(ref article, value);
        }

        public bool IsLoading
        {
            get => isLoading;
            set => SetProperty(ref isLoading, value);
        }

        public bool HasRelatedStocks
        {
            get => hasRelatedStocks;
            set => SetProperty(ref hasRelatedStocks, value);
        }

        // admin preview mode properties
        public bool IsAdminPreview
        {
            get => isAdminPreview;
            set => SetProperty(ref isAdminPreview, value);
        }

        public string ArticleStatus
        {
            get => articleStatus;
            set => SetProperty(ref articleStatus, value);
        }

        public bool CanApprove
        {
            get => canApprove;
            set => SetProperty(ref canApprove, value);
        }

        public bool CanReject
        {
            get => canReject;
            set => SetProperty(ref canReject, value);
        }

        // commands
        public ICommand BackCommand { get; }

        public ICommand ApproveCommand { get; }

        public ICommand RejectCommand { get; }

        public ICommand DeleteCommand { get; }

        // constructor
        public NewsDetailViewModel()
        {
            newsService = new NewsService();
            dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            // init commands
            BackCommand = new StockNewsRelayCommand(() => NavigationService.Instance.GoBack());
            ApproveCommand = new StockNewsRelayCommand(async () => await ApproveArticleAsync());
            RejectCommand = new StockNewsRelayCommand(async () => await RejectArticleAsync());
            DeleteCommand = new StockNewsRelayCommand(async () => await DeleteArticleAsync());
        }

        public async void LoadArticle(string articleId)
        {
            if (string.IsNullOrWhiteSpace(articleId))
                throw new ArgumentNullException(nameof(articleId));

            IsLoading = true;

            try
            {
                // if this is a preview
                if (articleId.StartsWith("preview:"))
                {
                    isPreviewMode = true;
                    previewId = articleId.Substring(8); // Remove "preview:"
                    currentArticleId = previewId;

                    // admin preview
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
                            HasRelatedStocks = article.RelatedStocks != null && article.RelatedStocks.Any();
                            System.Diagnostics.Debug.WriteLine($"Related stocks count: {article.RelatedStocks?.Count ?? 0}");
                        }
                        else
                        {
                            // Article not found
                            Article = new NewsArticle
                            {
                                Title = "Article Not Found",
                                Summary = "The requested preview article could not be found.",
                                Content = "The preview article you are looking for may no longer be available."
                            };
                            HasRelatedStocks = false;
                            System.Diagnostics.Debug.WriteLine("Preview article not found");
                        }

                        IsLoading = false;
                    });
                }
                else
                {
                    isPreviewMode = false;
                    currentArticleId = articleId;
                    IsAdminPreview = false;
                }

                // For non-preview articles, continue with existing logic
                var regularArticle = await newsService.GetNewsArticleByIdAsync(articleId);

                dispatcherQueue.TryEnqueue(async () =>
                {
                    if (regularArticle != null)
                    {
                        Article = regularArticle;
                        HasRelatedStocks = regularArticle.RelatedStocks != null && regularArticle.RelatedStocks.Any();

                        // Mark as read if not in preview mode
                        if (!isPreviewMode)
                        {
                            await newsService.MarkArticleAsReadAsync(articleId);
                        }
                    }
                    else
                    {
                        // Article not found
                        Article = new NewsArticle
                        {
                            Title = "Article Not Found",
                            Summary = "The requested article could not be found.",
                            Content = "The article you are looking for may have been removed or is no longer available."
                        };
                    }

                    IsLoading = false;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading article: {ex.Message}");

                dispatcherQueue.TryEnqueue(() =>
                {
                    Article = new NewsArticle
                    {
                        Title = "Error Loading Article",
                        Summary = "There was an error loading the article.",
                        Content = "Please try again later or contact support if the problem persists."
                    };

                    IsLoading = false;
                });
            }
        }

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
                    // update the status locally
                    ArticleStatus = "Approved";
                    CanApprove = false;
                    CanReject = true;

                    // success message
                    var dialog = new ContentDialog
                    {
                        Title = "Success",
                        Content = "Article has been approved and will now appear in the news list.",
                        CloseButtonText = "OK",
                        XamlRoot = App.CurrentWindow.Content.XamlRoot
                    };

                    await dialog.ShowAsync();

                    // nav back
                    NavigationService.Instance.GoBack();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error approving article: {ex.Message}");

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
                    // update status locally
                    ArticleStatus = "Rejected";
                    CanApprove = true;
                    CanReject = false;

                    // success message
                    var dialog = new ContentDialog
                    {
                        Title = "Success",
                        Content = "Article has been rejected.",
                        CloseButtonText = "OK",
                        XamlRoot = App.CurrentWindow.Content.XamlRoot
                    };

                    await dialog.ShowAsync();

                    // nav back
                    NavigationService.Instance.GoBack();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error rejecting article: {ex.Message}");

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

        private async Task DeleteArticleAsync()
        {
            if (!isPreviewMode || string.IsNullOrEmpty(previewId))
                return;

            try
            {
                // confirm deletion
                var confirmDialog = new ContentDialog
                {
                    Title = "Confirm Deletion",
                    Content = "Are you sure you want to delete this article? This action cannot be undone.",
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
                        // success message
                        var dialog = new ContentDialog
                        {
                            Title = "Success",
                            Content = "Article has been deleted.",
                            CloseButtonText = "OK",
                            XamlRoot = App.CurrentWindow.Content.XamlRoot
                        };

                        await dialog.ShowAsync();

                        // nav back
                        NavigationService.Instance.GoBack();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting article: {ex.Message}");

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

        // method to set property and raise PropertyChanged event
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
