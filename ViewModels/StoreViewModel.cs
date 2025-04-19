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
        private readonly StoreService storeService = new StoreService();

        private bool testMode = false; // Set to true for testing without the database

        private int _userGems;
        private string _currentUserCnp;
        private ObservableCollection<GemDeal> _availableDeals = new ObservableCollection<GemDeal>();
        private List<GemDeal> _possibleDeals = new List<GemDeal>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public StoreViewModel()
        {
            //PopulateHardcodedCnps();
            //PopulateUserTable();

            _currentUserCnp = storeService.GetCnp();
            LoadUserData();
            LoadGemDeals();
            LoadPossibleDeals();
            GenerateRandomDeals();
        }

        //public void PopulateHardcodedCnps() => storeService.PopulateHardcodedCnps();

        //public void PopulateUserTable() => storeService.PopulateUserTable();

        public bool IsGuest()
        {
            if (testMode)
                return false;

            bool guest = storeService.IsGuest(_currentUserCnp);
            return guest;
        }

        public int UserGems
        {
            get => _userGems;
            set
            {
                _userGems = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<GemDeal> AvailableDeals
        {
            get => _availableDeals;
            set
            {
                _availableDeals = value;
                OnPropertyChanged();
            }
        }

        public async void LoadUserData()
        {
            if (testMode)
            {
                UserGems = 1234; // Mocked gem balance
            }
            else
            {
                bool guest = storeService.IsGuest(_currentUserCnp);
                if (guest)
                {
                    UserGems = 0;
                }
                else
                {
                    UserGems = await Task.Run(() => storeService.GetUserGemBalance(_currentUserCnp));
                }
            }
        }


        public async Task<string> BuyGemsAsync(GemDeal deal, string selectedBankAccount)
        {
            if (string.IsNullOrEmpty(selectedBankAccount))
                return "No bank account selected.";

            if (testMode)
            {
                UserGems += deal.GemAmount;
                if (deal.IsSpecial)
                    AvailableDeals.Remove(deal);
                OnPropertyChanged(nameof(UserGems));
                OnPropertyChanged(nameof(AvailableDeals));
                return $"(TEST) Bought {deal.GemAmount} gems.";
            }

            var result = await storeService.BuyGems(_currentUserCnp, deal, selectedBankAccount);
            if (result.StartsWith("Successfully"))
            {
                UserGems += deal.GemAmount;
                if (deal.IsSpecial)
                    AvailableDeals.Remove(deal);
                OnPropertyChanged(nameof(UserGems));
                OnPropertyChanged(nameof(AvailableDeals));
            }
            return result;
        }


        public async Task<string> SellGemsAsync(int amount, string selectedBankAccount)
        {
            if (string.IsNullOrEmpty(selectedBankAccount))
                return "No bank account selected.";

            if (amount <= 0)
                return "Invalid amount.";

            if (amount > UserGems)
                return "Not enough Gems.";

            if (testMode)
            {
                UserGems -= amount;
                OnPropertyChanged(nameof(UserGems));
                return $"(TEST) Sold {amount} gems for {amount / 100.0}€.";
            }

            var result = await storeService.SellGems(_currentUserCnp, amount, selectedBankAccount);
            if (result.StartsWith("Successfully"))
            {
                UserGems -= amount;
                OnPropertyChanged(nameof(UserGems));
            }
            return result;
        }


        public List<string> GetUserBankAccounts()
        {
            return new List<string> { "Account 1", "Account 2", "Account 3" };
        }

        private void LoadGemDeals()
        {
            _availableDeals = new ObservableCollection<GemDeal>
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
            SortDeals();
        }

        private void LoadPossibleDeals()
        {
            _possibleDeals = new List<GemDeal>
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
            CheckAndRemoveExpiredDeals();
            var random = new Random();
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(15));
                var randomDeal = _possibleDeals[random.Next(_possibleDeals.Count)];
                var specialDeal = new GemDeal(randomDeal.Title, randomDeal.GemAmount, randomDeal.Price, true, randomDeal.DurationMinutes);
                AvailableDeals.Add(specialDeal);
                SortDeals();
                OnPropertyChanged(nameof(AvailableDeals));
            }
        }

        private async void CheckAndRemoveExpiredDeals()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(60));
                AvailableDeals = new ObservableCollection<GemDeal>(this.AvailableDeals.Where(deal => deal.IsAvailable));
                SortDeals();
                OnPropertyChanged(nameof(AvailableDeals));
            }
        }

        private void SortDeals()
        {
            var sortedDeals = AvailableDeals.OrderBy(deal => deal.ExpirationTime).ToList();
            AvailableDeals = new ObservableCollection<GemDeal>(sortedDeals);
            OnPropertyChanged(nameof(AvailableDeals));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
