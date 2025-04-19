namespace StockApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
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
    public class StockPageViewModel : INotifyPropertyChanged
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

        private ITextBlock priceLabel;
        private ITextBlock increaseLabel;
        private ITextBlock ownedStocks;
        private IChart stockChart;

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
            this.UpdateStockValue();
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
                  new StockPageService(),
                  selectedStock,
                  new TextBlockAdapter(priceLabel),
                  new TextBlockAdapter(increaseLabel),
                  new TextBlockAdapter(ownedStocks),
                  new ChartAdapter(stockChart))
        {
        }

        /// <summary>
        /// Updates all displayed stock values, including price, change percentage, owned count, and chart.
        /// </summary>
        public void UpdateStockValue()
        {
            if (!this.stockPageService.IsGuest())
            {
                this.userGems = this.stockPageService.GetUserBalance();
                this.ownedStocks.Text = "Owned: " + this.stockPageService.GetOwnedStocks().ToString();
            }
            List<int> stockHistory = this.stockPageService.GetStockHistory();
            this.priceLabel.Text = stockHistory.Last().ToString() + " ❇️ Gems";
            if (stockHistory.Count > 1)
            {
                int increasePerc = (stockHistory.Last() - stockHistory[stockHistory.Count - 2]) * 100 / stockHistory[stockHistory.Count - 2];
                this.increaseLabel.Text = increasePerc + "%";
                if (increasePerc > 0)
                {
                    this.increaseLabel.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    this.increaseLabel.Foreground = new SolidColorBrush(Colors.IndianRed);
                }
            }
            this.stockChart.UpdateLayout();
            this.stockChart.Series = new ISeries[]
            {
                new LineSeries<int>
                {
                    Values = stockHistory.TakeLast(30).ToArray(),
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColor.Parse("#4169E1"), 5), // FIXME: make stroke color configurable
                    GeometryStroke = new SolidColorPaint(SKColor.Parse("#4169E1"), 5),
                },
            };
            // TODO: handle case where stockHistory is empty to prevent exceptions
        }

        /// <summary>
        /// Gets or sets a value indicating whether this stock is in the user's favorites.
        /// </summary>
        public bool IsFavorite
        {
            get => this.isFavorite;
            set
            {
                if (this.isFavorite == value) return;
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
        /// Toggles the favorite state of the stock.
        /// </summary>
        public void ToggleFavorite()
        {
            this.IsFavorite = !this.IsFavorite;
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
        /// Attempts to buy a specified quantity of the stock.
        /// </summary>
        /// <param name="quantity">The number of shares to buy.</param>
        /// <returns><c>true</c> if the purchase succeeded; otherwise, <c>false</c>.</returns>
        public bool BuyStock(int quantity)
        {
            bool res = this.stockPageService.BuyStock(quantity);
            this.UpdateStockValue();
            return res;
        }

        /// <summary>
        /// Attempts to sell a specified quantity of the stock.
        /// </summary>
        /// <param name="quantity">The number of shares to sell.</param>
        /// <returns><c>true</c> if the sale succeeded; otherwise, <c>false</c>.</returns>
        public bool SellStock(int quantity)
        {
            bool res = this.stockPageService.SellStock(quantity);
            this.UpdateStockValue();
            return res;
        }

        /// <summary>
        /// Gets the author (owner) of the stock.
        /// </summary>
        /// <returns>A <see cref="User"/> object representing the author.</returns>
        public User GetStockAuthor()
        {
            return this.stockPageService.GetStockAuthor();
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
