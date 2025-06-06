﻿using StockApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using LiveChartsCore.SkiaSharpView.WinUI;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Painting;
using SkiaSharp;
using StocksApp;
using Catel.Services;


namespace StockApp.StockPage
{
    class StockPageViewModel : INotifyPropertyChanged
    {
        private string _stockName;
        private string _stockSymbol;
        private StockPageService _service;
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

        public StockPageViewModel(String stock_name, TextBlock priceLabel, TextBlock increaseLabel, TextBlock ownedStocks, CartesianChart stockChart)
        {
            this._service = new StockPageService(stock_name);
            this._priceLabel = priceLabel;
            this._increaseLabel = increaseLabel;
            this._ownedStocks = ownedStocks;
            this._stockChart = stockChart;

            IsGuest = _service.IsGuest();

            StockName = _service.GetStockName();
            StockSymbol = _service.GetStockSymbol();

            updateStockValue();

            IsFavorite = _service.getFavorite();
        }

        public void updateStockValue()
        {
            if (!_service.IsGuest())
            {
                UserGems = _service.GetUserBalance();
                _ownedStocks.Text = "Owned: " + _service.GetOwnedStocks().ToString();
            }
            List<int> stockHistory = _service.GetStockHistory();
            _priceLabel.Text = stockHistory.Last().ToString() + " ❇️ Gems";
            if (stockHistory.Count > 1)
            {
                int increasePerc = ((stockHistory.Last() - stockHistory[stockHistory.Count - 2]) * 100) / stockHistory[stockHistory.Count - 2];
                _increaseLabel.Text = increasePerc + "%";
                if (increasePerc > 0)
                {
                    _increaseLabel.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    _increaseLabel.Foreground = new SolidColorBrush(Colors.IndianRed);
                }
            }
            _stockChart.UpdateLayout();
            _stockChart.Series = new ISeries[]
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
            get { return _isFavorite; }
            set
            {
                if (_isFavorite == value) return;
                _isFavorite = value;
                _service.toggleFavorite(_isFavorite);
                if (_isFavorite)
                {
                    FavoriteButtonColor = "#ff0000"; // Red color for favorite
                }
                else
                {
                    FavoriteButtonColor = "#ffff5c"; // Default color
                }
                OnPropertyChanged(nameof(IsFavorite));
            }
        }

        public string FavoriteButtonColor
        {
            get { return _favoriteButtonColor; }
            set
            {
                _favoriteButtonColor = value;
                OnPropertyChanged(nameof(FavoriteButtonColor));
            }
        }

        public string StockName
        {
            get { return _stockName; }
            set
            {
                if (_stockName != value)
                {
                    _stockName = value;
                    OnPropertyChanged(nameof(StockName));
                }
            }
        }

        public string StockSymbol
        {
            get { return _stockSymbol; }
            set
            {
                if (_stockSymbol != value)
                {
                    _stockSymbol = value;
                    OnPropertyChanged(nameof(StockSymbol));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ToggleFavorite()
        {
            IsFavorite = !IsFavorite;
        }

        public bool IsGuest
        {
            get { return _isGuest; }
            set
            {
                _isGuest = value;
                GuestVisibility = _isGuest ? "Collapsed" : "Visible";
                OnPropertyChanged(nameof(IsGuest));
            }
        }

        public string GuestVisibility
        {
            get { return _guestVisibility; }
            set
            {
                _guestVisibility = value;
                OnPropertyChanged(nameof(GuestVisibility));
            }
        }

        public int UserGems
        {
            get { return _userGems; }
            set
            {
                _userGems = value;
                UserGemsText = $"{_userGems} ❇️ Gems";
                OnPropertyChanged(nameof(UserGems));
            }
        }

        public string UserGemsText
        {
            get { return _userGemsText; }
            set
            {
                _userGemsText = value;
                OnPropertyChanged(nameof(UserGemsText));
            }
        }

        public bool BuyStock(int quantity)
        {
            bool res = _service.BuyStock(quantity);
            updateStockValue();
            return res;
        }

        public bool SellStock(int quantity)
        {
            bool res = _service.SellStock(quantity);
            updateStockValue();
            return res;
        }

        public string getStockAuthor()
        {
            return _service.getStockAuthor();
        }
    }
}