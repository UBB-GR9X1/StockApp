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

    public class StoreViewModel : INotifyPropertyChanged
    {
        private readonly IStoreService storeService;

        private bool testMode = false; // Set to true for testing without the database

        private int userGems;
        private string currentUserCnp;
        private ObservableCollection<GemDeal> availableDeals = new ObservableCollection<GemDeal>();
        private List<GemDeal> possibleDeals = new List<GemDeal>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public StoreViewModel(IStoreService service)
        {
            this.storeService = service ?? throw new ArgumentNullException(nameof(service));
            this.Initialize();
        }

        public StoreViewModel()
        : this(new StoreService())
        { }

        private void Initialize()
        {
            this.currentUserCnp = this.storeService.GetCnp();
            this.LoadUserData();
            this.LoadGemDeals();
            this.LoadPossibleDeals();
            this.GenerateRandomDeals();
        }

        public bool IsGuest()
        {
            if (this.testMode)
                return false;

            bool guest = this.storeService.IsGuest(this.currentUserCnp);
            return guest;
        }

        public int UserGems
        {
            get => this.userGems;
            set
            {
                this.userGems = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<GemDeal> AvailableDeals
        {
            get => this.availableDeals;
            set
            {
                this.availableDeals = value;
                this.OnPropertyChanged();
            }
        }

        public async void LoadUserData()
        {
            if (this.testMode)
            {
                this.UserGems = 1234; // Mocked gem balance
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

        public async Task<string> BuyGemsAsync(GemDeal deal, string selectedBankAccount)
        {
            if (string.IsNullOrEmpty(selectedBankAccount))
                return "No bank account selected.";

            if (this.testMode)
            {
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

        public List<string> GetUserBankAccounts()
        {
            return new List<string> { "Account 1", "Account 2", "Account 3" };
        }

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

        private async void CheckAndRemoveExpiredDeals()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(60));
                this.AvailableDeals = new ObservableCollection<GemDeal>(this.AvailableDeals.Where(deal => deal.IsAvailable));
                this.SortDeals();
                this.OnPropertyChanged(nameof(this.AvailableDeals));
            }
        }

        private void SortDeals()
        {
            var sortedDeals = this.AvailableDeals.OrderBy(deal => deal.ExpirationTime).ToList();
            this.AvailableDeals = new ObservableCollection<GemDeal>(sortedDeals);
            this.OnPropertyChanged(nameof(this.AvailableDeals));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
