﻿using StockApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.StockPage
{
    class StockPageViewModel : INotifyPropertyChanged
    {
        Repository _repo = Repository.Instance;
        Stock _stock;

        private string _stockName;
        private string _stockSymbol;

        public void SetStockByName(string stockName)
        {
            _repo.getStockList().ForEach(stock =>
            {
                if (stock.Name == stockName)
                {
                    _stock = stock;
                    this.StockName = stock.Name;
                    this.StockSymbol = stock.Symbol;
                }
            });
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
    }
}
