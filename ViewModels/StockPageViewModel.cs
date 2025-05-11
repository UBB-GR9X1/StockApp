namespace StockApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using LiveChartsCore;
    using LiveChartsCore.SkiaSharpView;
    using LiveChartsCore.SkiaSharpView.Painting;
    using SkiaSharp;
    using StockApp.Models;
    using StockApp.Services;

    /// <summary>
    /// ViewModel managing data and operations for the stock detail page.
    /// </summary>
    public class StockPageViewModel : INotifyPropertyChanged
    {
        private readonly IStockPageService stockPageService;
        private readonly IUserService userService;
        private int userGems = 0;
        private Stock? selectedStock;

        public ISeries[] Series { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="StockPageViewModel"/> class with dependencies.
        /// </summary>
        /// <param name="stockPageService">The stock page service.</param>
        /// <param name="userService">The user service.</param>
        public StockPageViewModel(IUserService userService, IStockPageService stockPageService)
        {
            this.stockPageService = stockPageService ?? throw new ArgumentNullException(nameof(IStockPageService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(IUserService));
        }

        /// <summary>
        /// Updates all displayed stock values, including price, change percentage, owned count, and chart.
        /// </summary>
        public async Task UpdateStockValue()
        {
            if (!this.userService.IsGuest())
            {
                this.UserGems = await this.stockPageService.GetUserBalanceAsync();
                //this.ownedStocks.Text = "Owned: " + this.stockPageService.GetOwnedStocksAsync().ToString();
            }

            List<int> stockHistory = await this.stockPageService.GetStockHistoryAsync();
            //this.priceLabel.Text = stockHistory.Last().ToString() + " ❇️ Gems";
            if (stockHistory.Count > 1)
            {
                int increasePerc = (stockHistory.Last() - stockHistory[^2]) * 100 / stockHistory[^2];
                //this.increaseLabel.Text = increasePerc + "%";
                if (increasePerc > 0)
                {
                    //this.increaseLabel.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    //this.increaseLabel.Foreground = new SolidColorBrush(Colors.IndianRed);
                }
            }

            this.Series = [
                new LineSeries<int>
                {
                    Values = stockHistory.TakeLast(30).ToArray(),
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColor.Parse("#4169E1"), 5), // FIXME: make stroke color configurable
                    GeometryStroke = new SolidColorPaint(SKColor.Parse("#4169E1"), 5),
                },
            ];

            // TODO: handle case where stockHistory is empty to prevent exceptions
        }

        /// <summary>
        /// Gets or sets the selected stock.
        /// </summary>
        public Stock? SelectedStock
        {
            get => this.selectedStock;
            set
            {
                this.selectedStock = value;
                this.stockPageService.SelectStock(this.selectedStock);
                this.OnPropertyChanged(nameof(this.SelectedStock));
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
            throw new NotImplementedException("ToggleFavorite method is not implemented.");
            //this.IsFavorite = !this.IsFavorite;
        }

        /// <summary>
        /// Gets a value indicating whether the user is a guest.
        /// </summary>
        public bool IsGuest => this.userService.IsGuest();

        /// <summary>
        /// Gets or sets the user's current gem balance.
        /// </summary>
        public int UserGems
        {
            get => this.userGems;
            set
            {
                this.userGems = value;
                this.OnPropertyChanged(nameof(this.UserGems));
            }
        }

        /// <summary>
        /// Attempts to buy a specified quantity of the stock.
        /// </summary>
        /// <param name="quantity">The number of shares to buy.</param>
        /// <returns><c>true</c> if the purchase succeeded; otherwise, <c>false</c>.</returns>
        public async Task<bool> BuyStock(int quantity)
        {
            bool res = await this.stockPageService.BuyStockAsync(quantity);
            await this.UpdateStockValue();
            return res;
        }

        /// <summary>
        /// Attempts to sell a specified quantity of the stock.
        /// </summary>
        /// <param name="quantity">The number of shares to sell.</param>
        /// <returns><c>true</c> if the sale succeeded; otherwise, <c>false</c>.</returns>
        public async Task<bool> SellStock(int quantity)
        {
            bool res = await this.stockPageService.SellStockAsync(quantity);
            await this.UpdateStockValue();
            return res;
        }

        /// <summary>
        /// Gets the author (owner) of the stock.
        /// </summary>
        /// <returns>A <see cref="User"/> object representing the author.</returns>
        public async Task<User> GetStockAuthor()
        {
            return await this.stockPageService.GetStockAuthorAsync();
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
