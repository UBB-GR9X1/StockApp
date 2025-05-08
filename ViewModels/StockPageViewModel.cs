namespace StockApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Catel.Services;
    using LiveChartsCore;
    using LiveChartsCore.SkiaSharpView;
    using LiveChartsCore.SkiaSharpView.Painting;
    using LiveChartsCore.SkiaSharpView.WinUI;
    using Microsoft.UI;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Media;
    using SkiaSharp;
    using StockApp.Models;
    using StockApp.Services;

    /// <summary>
    /// ViewModel managing data and operations for the stock detail page.
    /// </summary>
    public class StockPageViewModel : INotifyPropertyChanged, IDisposable
    {
        private string stockName;
        private string stockSymbol;
        private readonly IStockPageService stockPageService;
        private bool isFavorite = false;
        private string favoriteButtonColor = "#ffff5c";
        private bool isGuest = false;
        private string guestVisibility = "Visible";
        private int userGems = 0;
        private string userGemsText = "0 ❇️ Gems";
        private bool isLoading = false;

        private readonly ITextBlock priceLabel;
        private readonly ITextBlock increaseLabel;
        private readonly ITextBlock ownedStocks;
        private readonly IChart stockChart;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockPageViewModel"/> class with dependencies.
        /// </summary>
        /// <param name="service">Service for retrieving stock data.</param>
        /// <param name="selectedStock">The stock object selected by the user.</param>
        /// <param name="priceLabel">Adapter for displaying the current price.</param>
        /// <param name="increaseLabel">Adapter for displaying price change percentage.</param>
        /// <param name="ownedStocks">Adapter for displaying number of owned stocks.</param>
        /// <param name="stockChart">Adapter for displaying the stock history chart.</param>
        public StockPageViewModel(
            IStockPageService service,
            Stock selectedStock,
            ITextBlock priceLabel,
            ITextBlock increaseLabel,
            ITextBlock ownedStocks,
            IChart stockChart)
        {
            this.stockPageService = service ?? throw new ArgumentNullException(nameof(service));
            this.priceLabel = priceLabel ?? throw new ArgumentNullException(nameof(priceLabel));
            this.increaseLabel = increaseLabel ?? throw new ArgumentNullException(nameof(increaseLabel));
            this.ownedStocks = ownedStocks ?? throw new ArgumentNullException(nameof(ownedStocks));
            this.stockChart = stockChart ?? throw new ArgumentNullException(nameof(stockChart));

            this.stockPageService.SelectStock(selectedStock);
            this.isGuest = this.stockPageService.IsGuest();
            this.stockName = this.stockPageService.GetStockName();
            this.stockSymbol = this.stockPageService.GetStockSymbol();
            this.isFavorite = this.stockPageService.GetFavorite();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StockPageViewModel"/> class with default adapters.
        /// </summary>
        /// <param name="selectedStock">The stock object selected by the user.</param>
        /// <param name="priceLabel">TextBlock control for price display.</param>
        /// <param name="increaseLabel">TextBlock control for percentage change display.</param>
        /// <param name="ownedStocks">TextBlock control for owned stocks display.</param>
        /// <param name="stockChart">CartesianChart control for history display.</param>
        public StockPageViewModel(
            Stock selectedStock,
            TextBlock priceLabel,
            TextBlock increaseLabel,
            TextBlock ownedStocks,
            CartesianChart stockChart)
            : this(
                  new StockPageApiService(new HttpClient(), "http://localhost:5000"),
                  selectedStock,
                  new TextBlockAdapter(priceLabel),
                  new TextBlockAdapter(increaseLabel),
                  new TextBlockAdapter(ownedStocks),
                  new ChartAdapter(stockChart))
        {
        }

        /// <summary>
        /// Initializes the view model asynchronously.
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;
                await UpdateStockValueAsync();
            }
            catch (Exception ex)
            {
                // Log the error or handle it appropriately
                throw new Exception("Failed to initialize stock page", ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Updates all displayed stock values asynchronously.
        /// </summary>
        public async Task UpdateStockValueAsync()
        {
            try
            {
                IsLoading = true;
                if (!this.stockPageService.IsGuest())
                {
                    this.userGems = await this.stockPageService.GetUserBalanceAsync();
                    this.ownedStocks.Text = "Owned: " + (await this.stockPageService.GetOwnedStocksAsync()).ToString();
                }

                List<int> stockHistory = await this.stockPageService.GetStockHistoryAsync();
                if (stockHistory == null || !stockHistory.Any())
                {
                    this.priceLabel.Text = "No data available";
                    this.increaseLabel.Text = "N/A";
                    return;
                }

                this.priceLabel.Text = stockHistory.Last().ToString() + " ❇️ Gems";
                if (stockHistory.Count > 1)
                {
                    int increasePerc = (stockHistory.Last() - stockHistory[^2]) * 100 / stockHistory[^2];
                    this.increaseLabel.Text = increasePerc + "%";
                    this.increaseLabel.Foreground = new SolidColorBrush(increasePerc > 0 ? Colors.Green : Colors.IndianRed);
                }

                this.stockChart.UpdateLayout();
                this.stockChart.Series = new ISeries[]
                {
                    new LineSeries<int>
                    {
                        Values = stockHistory.TakeLast(30).ToArray(),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColor.Parse("#4169E1"), 5),
                        GeometryStroke = new SolidColorPaint(SKColor.Parse("#4169E1"), 5),
                    },
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update stock value", ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the view model is currently loading data.
        /// </summary>
        public bool IsLoading
        {
            get => this.isLoading;
            set
            {
                if (this.isLoading != value)
                {
                    this.isLoading = value;
                    this.OnPropertyChanged(nameof(this.IsLoading));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this stock is in the user's favorites.
        /// </summary>
        public bool IsFavorite
        {
            get => this.isFavorite;
            set
            {
                if (this.isFavorite == value)
                {
                    return;
                }

                this.isFavorite = value;
                this.stockPageService.ToggleFavorite(this.isFavorite);
                if (this.isFavorite)
                {
                    this.favoriteButtonColor = "#ff0000"; // Red color for favorite
                }
                else
                {
                    this.favoriteButtonColor = "#ffff5c"; // Default color
                }

                this.OnPropertyChanged(nameof(this.IsFavorite));
            }
        }

        /// <summary>
        /// Gets or sets the color of the favorite button.
        /// </summary>
        public string FavoriteButtonColor
        {
            get => this.favoriteButtonColor;
            set
            {
                this.favoriteButtonColor = value;
                this.OnPropertyChanged(nameof(this.FavoriteButtonColor));
            }
        }

        /// <summary>
        /// Gets or sets the display name of the stock.
        /// </summary>
        public string StockName
        {
            get => this.stockName;
            set
            {
                if (this.stockName != value)
                {
                    this.stockName = value;
                    this.OnPropertyChanged(nameof(this.StockName));
                }
            }
        }

        /// <summary>
        /// Gets or sets the trading symbol of the stock.
        /// </summary>
        public string StockSymbol
        {
            get => this.stockSymbol;
            set
            {
                if (this.stockSymbol != value)
                {
                    this.stockSymbol = value;
                    this.OnPropertyChanged(nameof(this.StockSymbol));
                }
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Toggles the favorite state of the stock asynchronously.
        /// </summary>
        public async Task ToggleFavoriteAsync()
        {
            try
            {
                IsLoading = true;
                this.IsFavorite = !this.IsFavorite;
                await this.stockPageService.ToggleFavoriteAsync(this.IsFavorite);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to toggle favorite status", ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user is a guest.
        /// </summary>
        public bool IsGuest
        {
            get => this.isGuest;
            set
            {
                this.isGuest = value;
                this.guestVisibility = this.isGuest ? "Collapsed" : "Visible";
                this.OnPropertyChanged(nameof(this.IsGuest));
            }
        }

        /// <summary>
        /// Gets or sets the visibility of guest-only UI elements.
        /// </summary>
        public string GuestVisibility
        {
            get => this.guestVisibility;
            set
            {
                this.guestVisibility = value;
                this.OnPropertyChanged(nameof(this.GuestVisibility));
            }
        }

        /// <summary>
        /// Gets or sets the user's current gem balance.
        /// </summary>
        public int UserGems
        {
            get => this.userGems;
            set
            {
                this.userGems = value;
                this.userGemsText = $"{this.userGems} ❇️ Gems";
                this.OnPropertyChanged(nameof(this.UserGems));
            }
        }

        /// <summary>
        /// Gets or sets the display text for the user's gem balance.
        /// </summary>
        public string UserGemsText
        {
            get => this.userGemsText;
            set
            {
                this.userGemsText = value;
                this.OnPropertyChanged(nameof(this.UserGemsText));
            }
        }

        /// <summary>
        /// Buys the specified quantity of stocks asynchronously.
        /// </summary>
        public async Task<bool> BuyStockAsync(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than 0", nameof(quantity));
            }

            try
            {
                IsLoading = true;
                return await this.stockPageService.BuyStockAsync(quantity);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to buy stock", ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Sells the specified quantity of stocks asynchronously.
        /// </summary>
        public async Task<bool> SellStockAsync(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than 0", nameof(quantity));
            }

            try
            {
                IsLoading = true;
                return await this.stockPageService.SellStockAsync(quantity);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to sell stock", ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Gets the stock author asynchronously.
        /// </summary>
        public async Task<User> GetStockAuthorAsync()
        {
            try
            {
                IsLoading = true;
                return await this.stockPageService.GetStockAuthorAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get stock author", ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Disposes of the view model resources.
        /// </summary>
        public void Dispose()
        {
            // Clean up any resources if needed
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
