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

    internal class StockPageViewModel : INotifyPropertyChanged
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
        { }

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
                    Stroke = new SolidColorPaint(SKColor.Parse("#4169E1"), 5),
                    GeometryStroke = new SolidColorPaint(SKColor.Parse("#4169E1"), 5),
                }
            };
        }

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

        public string FavoriteButtonColor
        {
            get => this.favoriteButtonColor;
            set
            {
                this.favoriteButtonColor = value;
                this.OnPropertyChanged(nameof(this.FavoriteButtonColor));
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ToggleFavorite()
        {
            this.IsFavorite = !this.IsFavorite;
        }

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

        public string GuestVisibility
        {
            get => this.guestVisibility;
            set
            {
                this.guestVisibility = value;
                this.OnPropertyChanged(nameof(this.GuestVisibility));
            }
        }

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

        public string UserGemsText
        {
            get => this.userGemsText;
            set
            {
                this.userGemsText = value;
                this.OnPropertyChanged(nameof(this.UserGemsText));
            }
        }

        public bool BuyStock(int quantity)
        {
            bool res = this.stockPageService.BuyStock(quantity);
            this.UpdateStockValue();
            return res;
        }

        public bool SellStock(int quantity)
        {
            bool res = this.stockPageService.SellStock(quantity);
            this.UpdateStockValue();
            return res;
        }

        public User GetStockAuthor()
        {
            return this.stockPageService.GetStockAuthor();
        }
    }
}