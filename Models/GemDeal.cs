namespace StockApp.Models
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class GemDeal : IGemDeal
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Title { get; } = title;

        public int GemAmount { get; } = gemAmount;

        public double Price { get; } = price;

        public bool IsSpecial { get; } = isSpecial;

        public int? DurationMinutes { get; } = durationMinutes;

        public DateTime ExpirationTime
            => IsSpecial && DurationMinutes.HasValue
                ? DateTime.UtcNow.AddMinutes(DurationMinutes.Value)
                : DateTime.MaxValue;

        public bool IsAvailable
            => !IsSpecial
               || DurationMinutes is null
               || DateTime.UtcNow <= ExpirationTime;

        public string FormattedPrice
            => $"{Price:0.00}€";

        public string ExpirationTimeFormatted
            => ExpirationTime == DateTime.MaxValue
                ? string.Empty
                : ExpirationTime.ToString("HH:mm:ss");

        public GemDeal(
            string title,
            int gemAmount,
            double price,
            bool isSpecial = false,
            int? durationMinutes = null)
        {
            Title = title;
            GemAmount = gemAmount;
            Price = price;
            IsSpecial = isSpecial;
            DurationMinutes = durationMinutes;
        }

        protected void OnPropertyChanged([CallerMemberName] string? prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
