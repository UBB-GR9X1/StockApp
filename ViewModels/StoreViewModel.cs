namespace StockApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using StockApp.Models;
    using StockApp.Services;

    /// <summary>
    /// ViewModel for the store page, managing gem deals and user gem balance.
    /// </summary>
    public class StoreViewModel : INotifyPropertyChanged
    {
        private readonly IStoreService storeService;

        private bool testMode = false; // Set to true for testing without the database

        private int userGems;
        private string currentUserCnp;
        private ObservableCollection<GemDeal> availableDeals = new ObservableCollection<GemDeal>();
        private List<GemDeal> possibleDeals = new List<GemDeal>();

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreViewModel"/> class with the specified service.
        /// </summary>
        /// <param name="service">Service used to retrieve and update store data.</param>
        public StoreViewModel(IStoreService service)
        {
            this.storeService = service ?? throw new ArgumentNullException(nameof(service));
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreViewModel"/> class with default service.
        /// </summary>
        public StoreViewModel()
            : this(new StoreService())
        { }

        /// <summary>
        /// Initializes user data and gem deals.
        /// </summary>
        private void Initialize()
        {
            this.currentUserCnp = this.storeService.GetCnp();
            this.LoadUserData();
            this.LoadGemDeals();
            this.LoadPossibleDeals();
            this.GenerateRandomDeals();
        }

        /// <summary>
        /// Determines whether the current user is a guest.
        /// </summary>
        /// <returns><c>true</c> if the user is a guest; otherwise, <c>false</c>.</returns>
        public bool IsGuest()
        {
            if (this.testMode)
                return false;

            bool guest = this.storeService.IsGuest(this.currentUserCnp);
            return guest;
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
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the collection of available gem deals.
        /// </summary>
        public ObservableCollection<GemDeal> AvailableDeals
        {
            get => this.availableDeals;
            set
            {
                this.availableDeals = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Loads the user gem balance asynchronously.
        /// </summary>
        // FIXME: Change async void to async Task for better error handling
        public async void LoadUserData()
        {
            if (this.testMode)
            {
                // Inline: use mocked balance in test mode
                this.UserGems = 1234;
            }
            else
            {
                bool guest = this.storeService.IsGuest(this.currentUserCnp);
                if (guest)
                {
                    this.UserGems = 0;
                }
                else
                {
                    this.UserGems = await Task.Run(() => this.storeService.GetUserGemBalance(this.currentUserCnp));
                }
            }
        }

        /// <summary>
        /// Buys gems asynchronously using the specified deal and bank account.
        /// </summary>
        /// <param name="deal">The gem deal to purchase.</param>
        /// <param name="selectedBankAccount">The bank account identifier to use.</param>
        /// <returns>A result message indicating success or failure.</returns>
        public async Task<string> BuyGemsAsync(GemDeal deal, string selectedBankAccount)
        {
            if (string.IsNullOrEmpty(selectedBankAccount))
                return "No bank account selected.";

            if (this.testMode)
            {
                // Inline: simulate purchase in test mode
                this.UserGems += deal.GemAmount;
                if (deal.IsSpecial)
                    this.AvailableDeals.Remove(deal);
                this.OnPropertyChanged(nameof(this.UserGems));
                this.OnPropertyChanged(nameof(this.AvailableDeals));
                return $"(TEST) Bought {deal.GemAmount} gems.";
            }

            var result = await this.storeService.BuyGems(this.currentUserCnp, deal, selectedBankAccount);
            if (result.StartsWith("Successfully"))
            {
                this.UserGems += deal.GemAmount;
                if (deal.IsSpecial)
                    this.AvailableDeals.Remove(deal);
                this.OnPropertyChanged(nameof(this.UserGems));
                this.OnPropertyChanged(nameof(this.AvailableDeals));
            }
            return result;
        }

        /// <summary>
        /// Sells a specified amount of gems asynchronously using the given bank account.
        /// </summary>
        /// <param name="amount">The number of gems to sell.</param>
        /// <param name="selectedBankAccount">The bank account identifier to receive funds.</param>
        /// <returns>A result message indicating success or failure.</returns>
        public async Task<string> SellGemsAsync(int amount, string selectedBankAccount)
        {
            if (string.IsNullOrEmpty(selectedBankAccount))
                return "No bank account selected.";

            if (amount <= 0)
                return "Invalid amount.";

            if (amount > this.UserGems)
                return "Not enough Gems.";

            if (this.testMode)
            {
                // Inline: simulate sell in test mode
                this.UserGems -= amount;
                this.OnPropertyChanged(nameof(this.UserGems));
                return $"(TEST) Sold {amount} gems for {amount / 100.0}€.";
            }

            var result = await this.storeService.SellGems(this.currentUserCnp, amount, selectedBankAccount);
            if (result.StartsWith("Successfully"))
            {
                this.UserGems -= amount;
                this.OnPropertyChanged(nameof(this.UserGems));
            }
            return result;
        }

        /// <summary>
        /// Gets a list of the user's linked bank accounts.
        /// </summary>
        /// <returns>A list of bank account names.</returns>
        public List<string> GetUserBankAccounts()
        {
            // TODO: Replace with real data source
            return new List<string> { "Account 1", "Account 2", "Account 3" };
        }

        /// <summary>
        /// Populates the initial set of gem deals.
        /// </summary>
        private void LoadGemDeals()
        {
            this.AvailableDeals = new ObservableCollection<GemDeal>
            {
                new GemDeal("LEGENDARY DEAL!!!!", 4999, 100.0),
                new GemDeal("MYTHIC DEAL!!!!", 3999, 90.0),
                new GemDeal("INSANE DEAL!!!!", 3499, 85.0),
                new GemDeal("GIGA DEAL!!!!", 3249, 82.0),
                new GemDeal("WOW DEAL!!!!", 3000, 80.0),
                new GemDeal("YAY DEAL!!!!", 2500, 50.0),
                new GemDeal("YUPY DEAL!!!!", 2000, 49.0),
                new GemDeal("HELL NAH DEAL!!!", 1999, 48.0),
                new GemDeal("BAD DEAL!!!!", 1000, 45.0),
                new GemDeal("MEGA BAD DEAL!!!!", 500, 40.0),
                new GemDeal("BAD DEAL!!!!", 1, 35.0),
                new GemDeal("🔥 SPECIAL DEAL", 2, 2.0, true, 1)
            };
            this.SortDeals();
        }

        /// <summary>
        /// Loads the pool of possible special deals.
        /// </summary>
        private void LoadPossibleDeals()
        {
            this.possibleDeals = new List<GemDeal>
            {
                new GemDeal("🔥 Limited Deal!", 6000, 120.0, true, 1),
                new GemDeal("🔥 Flash Sale!", 5000, 100.0, true, 60),
                new GemDeal("🔥 Mega Discount!", 4000, 80.0, true, 30),
                new GemDeal("🔥 Special Offer!", 3000, 60.0, true, 5),
                new GemDeal("🔥 Exclusive Deal!", 2000, 40.0, true, 1)
            };
        }

        /// <summary>
        /// Continuously generates random special deals.
        /// </summary>
        // FIXME: Consider adding cancellation token to stop the loop
        private async void GenerateRandomDeals()
        {
            this.CheckAndRemoveExpiredDeals();
            var random = new Random();
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(15));
                var randomDeal = this.possibleDeals[random.Next(this.possibleDeals.Count)];
                var specialDeal = new GemDeal(randomDeal.Title, randomDeal.GemAmount, randomDeal.Price, true, randomDeal.DurationMinutes);
                this.AvailableDeals.Add(specialDeal);
                this.SortDeals();
                this.OnPropertyChanged(nameof(this.AvailableDeals));
            }
        }

        /// <summary>
        /// Periodically removes expired deals from available deals.
        /// </summary>
        private async void CheckAndRemoveExpiredDeals()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(60));
                // Inline: filter out expired deals
                this.AvailableDeals = new ObservableCollection<GemDeal>(this.AvailableDeals.Where(deal => deal.IsAvailable));
                this.SortDeals();
                this.OnPropertyChanged(nameof(this.AvailableDeals));
            }
        }

        /// <summary>
        /// Sorts the available deals by expiration time.
        /// </summary>
        private void SortDeals()
        {
            var sortedDeals = this.AvailableDeals.OrderBy(deal => deal.ExpirationTime).ToList();
            this.AvailableDeals = new ObservableCollection<GemDeal>(sortedDeals);
            this.OnPropertyChanged(nameof(this.AvailableDeals));
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the changed property.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
