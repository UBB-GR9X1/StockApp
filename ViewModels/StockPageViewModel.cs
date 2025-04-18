namespace StockApp.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
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
        private string _stockName;
        private string _stockSymbol;
        private StockPageService stockPageService = new();
        private bool _isFavorite = false;
        private string _favoriteButtonColor = "#ffff5c";
        private bool _isGuest = false;
        private string _guestVisibility = "Visible";
        private int _userGems = 0;
        private string _userGemsText = "0 ❇️ Gems";

        private TextBlock _priceLabel;
        private TextBlock _increaseLabel;
        private TextBlock _ownedStocks;
        private CartesianChart _stockChart;

        public StockPageViewModel(Stock selectedStock, TextBlock priceLabel, TextBlock increaseLabel, TextBlock ownedStocks, CartesianChart stockChart)
        {
            this.stockPageService.SelectStock(selectedStock);
            this._priceLabel = priceLabel;
            this._increaseLabel = increaseLabel;
            this._ownedStocks = ownedStocks;
            this._stockChart = stockChart;

            this.IsGuest = this.stockPageService.IsGuest();

            this._stockName = this.stockPageService.GetStockName();
            this._stockSymbol = this.stockPageService.GetStockSymbol();

            this.UpdateStockValue();

            this.IsFavorite = this.stockPageService.GetFavorite();
        }

        public void UpdateStockValue()
        {
            if (!this.stockPageService.IsGuest())
            {
                this.UserGems = this.stockPageService.GetUserBalance();
                this._ownedStocks.Text = "Owned: " + this.stockPageService.GetOwnedStocks().ToString();
            }
            List<int> stockHistory = this.stockPageService.GetStockHistory();
            this._priceLabel.Text = stockHistory.Last().ToString() + " ❇️ Gems";
            if (stockHistory.Count > 1)
            {
                int increasePerc = (stockHistory.Last() - stockHistory[stockHistory.Count - 2]) * 100 / stockHistory[stockHistory.Count - 2];
                this._increaseLabel.Text = increasePerc + "%";
                if (increasePerc > 0)
                {
                    this._increaseLabel.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    this._increaseLabel.Foreground = new SolidColorBrush(Colors.IndianRed);
                }
            }
            this._stockChart.UpdateLayout();
            this._stockChart.Series = new ISeries[]
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
                return this._isFavorite;
            }

            set
            {
                if (this._isFavorite == value) return;
                this._isFavorite = value;
                this.stockPageService.ToggleFavorite(this._isFavorite);
                if (this._isFavorite)
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
                return this._favoriteButtonColor;
            }

            set
            {
                this._favoriteButtonColor = value;
                this.OnPropertyChanged(nameof(this.FavoriteButtonColor));
            }
        }

        public string StockName
        {
            get
            {
                return this._stockName;
            }

            set
            {
                if (this._stockName != value)
                {
                    this._stockName = value;
                    this.OnPropertyChanged(nameof(this.StockName));
                }
            }
        }

        public string StockSymbol
        {
            get
            {
                return this._stockSymbol;
            }

            set
            {
                if (this._stockSymbol != value)
                {
                    this._stockSymbol = value;
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
                return this._isGuest;
            }

            set
            {
                this._isGuest = value;
                this.GuestVisibility = this._isGuest ? "Collapsed" : "Visible";
                this.OnPropertyChanged(nameof(this.IsGuest));
            }
        }

        public string GuestVisibility
        {
            get
            {
                return this._guestVisibility;
            }

            set
            {
                this._guestVisibility = value;
                this.OnPropertyChanged(nameof(this.GuestVisibility));
            }
        }

        public int UserGems
        {
            get
            {
                return this._userGems;
            }

            set
            {
                this._userGems = value;
                this.UserGemsText = $"{this._userGems} ❇️ Gems";
                this.OnPropertyChanged(nameof(this.UserGems));
            }
        }

        public string UserGemsText
        {
            get
            {
                return this._userGemsText;
            }

            set
            {
                this._userGemsText = value;
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