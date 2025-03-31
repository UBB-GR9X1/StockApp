using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GemStore.Models
{
    public class GemDeal : INotifyPropertyChanged
    {
        public string Title { get; set; }
        public int GemAmount { get; set; }
        public double Price { get; set; }
        public bool IsSpecial { get; set; }
        public int? DurationMinutes { get; set; } // Duration in minutes for which the deal is available
        public DateTime? ExpirationTime { get; set; } // Time when the deal expires

        public string FormattedPrice => $"{Price}€";

        public GemDeal(string title, int gemAmount, double price, bool isSpecial = false, int? durationMinutes = null)
        {
            Title = title;
            GemAmount = gemAmount;
            Price = price;
            IsSpecial = isSpecial;
            DurationMinutes = durationMinutes;
            if (isSpecial && durationMinutes.HasValue)
            {
                ExpirationTime = DateTime.Now.AddMinutes(durationMinutes.Value);
            }
        }

        public bool IsAvailable()
        {
            if (IsSpecial && ExpirationTime.HasValue)
            {
                return DateTime.Now <= ExpirationTime.Value;
            }
            return true;
        }

        public string ExpirationTimeFormatted => ExpirationTime?.ToString("HH:mm:ss") ?? string.Empty;

        public string GetTitle() => Title;
        public int GetGemAmount() => GemAmount;
        public double GetPrice() => Price;
        public bool GetIsSpecial() => IsSpecial;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}