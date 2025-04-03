using StockApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;


namespace StockApp.StockPage
{
    class StockPageViewModel : INotifyPropertyChanged
    {
        private string _stockName;
        private string _stockSymbol;
        private StockPageService _service;
        private bool _isFavorite = false;
        private string _favoriteButtonColor = "#ffff5c";
        private bool _isGuest = true;
        private string _guestVisibility = "Visible";
        private int _userGems = 0;
        private string _userGemsText = "0 ❇️";

        public StockPageViewModel(String stock_name)
        {
            this._service = new StockPageService(stock_name);

            StockName = _service.GetStockName();
            StockSymbol = _service.GetStockSymbol();
        }


        public ISeries[] Series { get; set; } = [
            new LineSeries<int> {
                Values = new int[] { 4, 6, 5, 3, -3, -1, 2 }
            }
        ];

        public bool IsFavorite
        {
            get { return _isFavorite; }
            set
            {
                _isFavorite = value;
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
                GuestVisibility = _isGuest ? "Visible" : "Collapsed";
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
                UserGemsText = $"{_userGems} ❇️";
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
    }
}
