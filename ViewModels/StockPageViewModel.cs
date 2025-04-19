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

    class StockPageViewModel : INotifyPropertyChanged
    {
        private string StockName;
        private string StockSymbol;
        private readonly IStockPageService StockPageService;
        private bool IsFavorite = false;
        private string FavoriteButtonColor = "#ffff5c";
        private bool IsGuest = false;
        private string GuestVisibility = "Visible";
        private int UserGems = 0;
        private string UserGemsText = "0 ❇️ Gems";

        private ITextBlock PriceLabel;
        private ITextBlock IncreaseLabel;
        private ITextBlock OwnedStocks;
        private IChart StockChart;


        public StockPageViewModel(
            IStockPageService service,
            Stock selectedStock,
            ITextBlock priceLabel,
            ITextBlock increaseLabel,
            ITextBlock ownedStocks,
            IChart stockChart)
        {
            this.StockPageService = service ?? throw new ArgumentNullException(nameof(service));
            this.PriceLabel = priceLabel ?? throw new ArgumentNullException(nameof(priceLabel));
            this.IncreaseLabel = increaseLabel ?? throw new ArgumentNullException(nameof(increaseLabel));
            this.OwnedStocks = ownedStocks ?? throw new ArgumentNullException(nameof(ownedStocks));
            this.StockChart = stockChart ?? throw new ArgumentNullException(nameof(stockChart));

            this.StockPageService.SelectStock(selectedStock);
            this.IsGuest = this.StockPageService.IsGuest();
            this.StockName = this.StockPageService.GetStockName();
            this.StockSymbol = this.StockPageService.GetStockSymbol();
            this.UpdateStockValue();
            this.IsFavorite = this.StockPageService.GetFavorite();
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
            if (!this.StockPageService.IsGuest())
            {
                this.UserGems = this.StockPageService.GetUserBalance();
                this.OwnedStocks.Text = "Owned: " + this.StockPageService.GetOwnedStocks().ToString();
            }
            List<int> stockHistory = this.StockPageService.GetStockHistory();
            this.PriceLabel.Text = stockHistory.Last().ToString() + " ❇️ Gems";
            if (stockHistory.Count > 1)
            {
                int increasePerc = (stockHistory.Last() - stockHistory[stockHistory.Count - 2]) * 100 / stockHistory[stockHistory.Count - 2];
                this.IncreaseLabel.Text = increasePerc + "%";
                if (increasePerc > 0)
                {
                    this.IncreaseLabel.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    this.IncreaseLabel.Foreground = new SolidColorBrush(Colors.IndianRed);
                }
            }
            this.StockChart.UpdateLayout();
            this.StockChart.Series = new ISeries[]
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
            get
            {
                return this.IsFavorite;
            }

            set
            {
                if (this.IsFavorite == value) return;
                this.IsFavorite = value;
                this.StockPageService.ToggleFavorite(this.IsFavorite);
                if (this.IsFavorite)
                {
                    this.FavoriteButtonColor = "#ff0000"; // Red color for favorite
                }
                else
                {
                    this.FavoriteButtonColor = "#ffff5c"; // Default color
                }
                this.OnPropertyChanged(nameof(this.IsFavorite));
            }
        }

        public string FavoriteButtonColor
        {
            get
            {
                return this.FavoriteButtonColor;
            }

            set
            {
                this.FavoriteButtonColor = value;
                this.OnPropertyChanged(nameof(this.FavoriteButtonColor));
            }
        }

        public string StockName
        {
            get
            {
                return this.StockName;
            }

            set
            {
                if (this.StockName != value)
                {
                    this.StockName = value;
                    this.OnPropertyChanged(nameof(this.StockName));
                }
            }
        }

        public string StockSymbol
        {
            get
            {
                return this.StockSymbol;
            }

            set
            {
                if (this.StockSymbol != value)
                {
                    this.StockSymbol = value;
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
            get
            {
                return this.IsGuest;
            }

            set
            {
                this.IsGuest = value;
                this.GuestVisibility = this.IsGuest ? "Collapsed" : "Visible";
                this.OnPropertyChanged(nameof(this.IsGuest));
            }
        }

        public string GuestVisibility
        {
            get
            {
                return this.GuestVisibility;
            }

            set
            {
                this.GuestVisibility = value;
                this.OnPropertyChanged(nameof(this.GuestVisibility));
            }
        }

        public int UserGems
        {
            get
            {
                return this.UserGems;
            }

            set
            {
                this.UserGems = value;
                this.UserGemsText = $"{this.UserGems} ❇️ Gems";
                this.OnPropertyChanged(nameof(this.UserGems));
            }
        }

        public string UserGemsText
        {
            get
            {
                return this.UserGemsText;
            }

            set
            {
                this.UserGemsText = value;
                this.OnPropertyChanged(nameof(this.UserGemsText));
            }
        }

        public bool BuyStock(int quantity)
        {
            bool res = this.StockPageService.BuyStock(quantity);
            this.UpdateStockValue();
            return res;
        }

        public bool SellStock(int quantity)
        {
            bool res = this.StockPageService.SellStock(quantity);
            this.UpdateStockValue();
            return res;
        }

        public User GetStockAuthor()
        {
            return this.StockPageService.GetStockAuthor();
        }
    }
}