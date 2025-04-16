namespace StockApp.Models
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public partial class GemStoreGemDeal(
        string title,
        int gemAmount,
        double price,
        bool isSpecial = false,
        int? durationMinutes = null) : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Title { get; set; } = title;

        public int GemAmount { get; set; } = gemAmount;

        public double Price { get; set; } = price;

        public bool IsSpecial { get; set; } = isSpecial;

        public int? DurationMinutes { get; set; } = durationMinutes;

        public DateTime? ExpirationTime { get; set; } =
            isSpecial && durationMinutes.HasValue
                ? DateTime.Now.AddMinutes(durationMinutes.Value)
                : DateTime.MaxValue;

        public string FormattedPrice => $"{this.Price}€";

        public string ExpirationTimeFormatted => this.ExpirationTime?.ToString("HH:mm:ss") ?? string.Empty;

        public bool IsAvailable()
        {
            return !this.IsSpecial
                || !this.ExpirationTime.HasValue
                || DateTime.Now <= this.ExpirationTime.Value;
        }

        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}