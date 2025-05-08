using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StockApp.Models;
using StockApp.Services;
using StockApp.ViewModels;
using System;
using System.Threading.Tasks;

namespace StockApp.Views.Pages
{
    public sealed partial class StockPageView : Page
    {
        public StockPageViewModel ViewModel { get; }
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IStockPageService _stockPageService;
        private readonly IAppState _appState;
        private ProgressRing _loadingRing;

        public StockPageView()
        {
            this.InitializeComponent();
            
            // Get services from App.Current
            _dialogService = App.Current.Services.GetService(typeof(IDialogService)) as IDialogService;
            _navigationService = App.Current.Services.GetService(typeof(INavigationService)) as INavigationService;
            _stockPageService = App.Current.Services.GetService(typeof(IStockPageService)) as IStockPageService;
            _appState = App.Current.Services.GetService(typeof(IAppState)) as IAppState;
            
            InitializeLoadingRing();
            InitializeViewModel();
        }

        private void InitializeLoadingRing()
        {
            _loadingRing = new ProgressRing
            {
                IsActive = true,
                Visibility = Visibility.Collapsed
            };
            MainGrid.Children.Add(_loadingRing);
            Grid.SetRowSpan(_loadingRing, 4);
        }

        private async void InitializeViewModel()
        {
            try
            {
                ShowLoading();
                // Get the selected stock from navigation parameters
                if (App.Current.Properties.TryGetValue("SelectedStock", out var selectedStock) && selectedStock is Stock stock)
                {
                    // Create the view model with injected service
                    ViewModel = new StockPageViewModel(
                        _stockPageService,
                        stock,
                        PriceLabel,
                        IncreaseLabel,
                        OwnedStocks,
                        StockChart
                    );

                    // Set the DataContext
                    this.DataContext = ViewModel;

                    // Initialize the view model
                    await ViewModel.InitializeAsync();
                }
                else
                {
                    await HandleNoStockSelected();
                }
            }
            catch (Exception ex)
            {
                await HandleError("Failed to initialize stock page", ex);
            }
            finally
            {
                HideLoading();
            }
        }

        private void ShowLoading()
        {
            _loadingRing.Visibility = Visibility.Visible;
            _loadingRing.IsActive = true;
        }

        private void HideLoading()
        {
            _loadingRing.Visibility = Visibility.Collapsed;
            _loadingRing.IsActive = false;
        }

        private async Task HandleNoStockSelected()
        {
            await _dialogService.ShowMessageDialogAsync(
                "Error",
                "No stock was selected. Please select a stock from the list.",
                "OK"
            );
            _navigationService.NavigateBack();
        }

        private async Task HandleError(string message, Exception ex)
        {
            await _dialogService.ShowMessageDialogAsync(
                "Error",
                $"{message}: {ex.Message}",
                "OK"
            );
            _navigationService.NavigateBack();
        }

        private async void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowLoading();
                if (!_appState.IsAuthenticated)
                {
                    await _dialogService.ShowMessageDialogAsync(
                        "Authentication Required",
                        "Please sign in to add stocks to favorites.",
                        "OK"
                    );
                    return;
                }

                await ViewModel.ToggleFavoriteAsync();
            }
            catch (Exception ex)
            {
                await HandleError("Failed to update favorite status", ex);
            }
            finally
            {
                HideLoading();
            }
        }

        private async void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowLoading();
                if (!_appState.IsAuthenticated)
                {
                    await _dialogService.ShowMessageDialogAsync(
                        "Authentication Required",
                        "Please sign in to buy stocks.",
                        "OK"
                    );
                    return;
                }

                if (!QuantityBox.Value.HasValue || QuantityBox.Value.Value <= 0)
                {
                    await _dialogService.ShowMessageDialogAsync(
                        "Invalid Quantity",
                        "Please enter a valid quantity greater than 0.",
                        "OK"
                    );
                    return;
                }

                int quantity = (int)QuantityBox.Value.Value;
                if (await ViewModel.BuyStockAsync(quantity))
                {
                    await ViewModel.UpdateStockValueAsync();
                    await _dialogService.ShowMessageDialogAsync(
                        "Success",
                        $"Successfully bought {quantity} shares of {ViewModel.StockName}",
                        "OK"
                    );
                }
            }
            catch (Exception ex)
            {
                await HandleError("Failed to buy stock", ex);
            }
            finally
            {
                HideLoading();
            }
        }

        private async void SellButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowLoading();
                if (!_appState.IsAuthenticated)
                {
                    await _dialogService.ShowMessageDialogAsync(
                        "Authentication Required",
                        "Please sign in to sell stocks.",
                        "OK"
                    );
                    return;
                }

                if (!QuantityBox.Value.HasValue || QuantityBox.Value.Value <= 0)
                {
                    await _dialogService.ShowMessageDialogAsync(
                        "Invalid Quantity",
                        "Please enter a valid quantity greater than 0.",
                        "OK"
                    );
                    return;
                }

                int quantity = (int)QuantityBox.Value.Value;
                if (await ViewModel.SellStockAsync(quantity))
                {
                    await ViewModel.UpdateStockValueAsync();
                    await _dialogService.ShowMessageDialogAsync(
                        "Success",
                        $"Successfully sold {quantity} shares of {ViewModel.StockName}",
                        "OK"
                    );
                }
            }
            catch (Exception ex)
            {
                await HandleError("Failed to sell stock", ex);
            }
            finally
            {
                HideLoading();
            }
        }

        protected override async void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                ShowLoading();
                if (ViewModel != null)
                {
                    await ViewModel.UpdateStockValueAsync();
                }
            }
            catch (Exception ex)
            {
                await HandleError("Failed to update stock value", ex);
            }
            finally
            {
                HideLoading();
            }
        }

        protected override void OnNavigatedFrom(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel?.Dispose();
        }
    }
} 