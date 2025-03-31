using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Threading.Tasks;
using GemStore.Models;

namespace GemStore.ViewModels
{
    public class StoreViewModel : INotifyPropertyChanged
    {
        private int _userGems = 10000; // Get the actual value from the user's account
        private string _userType = "Registered";
        private ObservableCollection<GemDeal> _availableDeals = new ObservableCollection<GemDeal>();
        private List<GemDeal> _possibleDeals = new List<GemDeal>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public StoreViewModel()
        {
            LoadGemDeals();
            LoadPossibleDeals();
            GenerateRandomDeals();
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

        public string UserType
        {
            get => _userType;
            set
            {
                _userType = value;
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
                new GemDeal("🔥 SPECIAL DEAL", 2, 2.0, true, 1) // Example special deal with duration in minutes
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
                await Task.Delay(TimeSpan.FromSeconds(60)); // add new random deal each 10s for faster testing

                // Select a random deal from the possible deals
                var randomDeal = _possibleDeals[random.Next(_possibleDeals.Count)];
                var specialDeal = new GemDeal(randomDeal.Title, randomDeal.GemAmount, randomDeal.Price, randomDeal.IsSpecial, randomDeal.DurationMinutes);
                _availableDeals.Add(specialDeal);
                SortDeals();
                OnPropertyChanged(nameof(AvailableDeals));

            }
        }

        private async void CheckAndRemoveExpiredDeals()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(60));


                // Remove expired deals
                _availableDeals = new ObservableCollection<GemDeal>(_availableDeals.Where(deal => deal.IsAvailable()));
                SortDeals();
                OnPropertyChanged(nameof(AvailableDeals));
            }
        }

        private void SortDeals()
        {
            var sortedDeals = _availableDeals.OrderBy(deal => deal.ExpirationTime ?? DateTime.MaxValue).ToList();
            _availableDeals = new ObservableCollection<GemDeal>(sortedDeals);
            OnPropertyChanged(nameof(AvailableDeals));
        }

        public bool IsGuest()
        {
            return UserType == "Guest";
        }

        public string BuyGems(GemDeal deal, string selectedBankAccount)
        {
            if (IsGuest())
                return "Guests cannot buy gems.";

            if (string.IsNullOrEmpty(selectedBankAccount))
                throw new ArgumentNullException(nameof(selectedBankAccount));

            // Deduct money (simulate transaction)
            double price = deal.Price;

            UserGems += deal.GemAmount;
            OnPropertyChanged(nameof(UserGems)); // Real-time update

            // Remove the deal if it is a special deal
            if (deal.IsSpecial)
            {
                _availableDeals.Remove(deal);
                OnPropertyChanged(nameof(AvailableDeals));
            }

            return $"Purchase successful! You bought {deal.GemAmount} Gems.";
        }

        public string SellGems(int amount, string selectedBankAccount)
        {
            if (IsGuest())
                return "Guests cannot sell gems.";

            if (string.IsNullOrEmpty(selectedBankAccount))
                throw new ArgumentNullException(nameof(selectedBankAccount));

            if (amount <= 0)
                return "Invalid amount.";

            if (amount > UserGems)
                return "Not enough Gems.";

            // Convert Gems to money
            double moneyEarned = amount / 100.0; // 100 Gems = 1€

            UserGems -= amount;
            OnPropertyChanged(nameof(UserGems)); // Real-time update

            return $"Sale successful! You earned {moneyEarned}€.";
        }

        public List<string> GetUserBankAccounts()
        {
            return new List<string> { "Account 1", "Account 2", "Account 3" };
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}