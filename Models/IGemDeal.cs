using System;
using System.ComponentModel;

namespace StockApp.Models
{
    public interface IGemDeal : INotifyPropertyChanged
    {
        string Title { get; }

        int GemAmount { get; }

        double Price { get; }

        bool IsSpecial { get; }

        int? DurationMinutes { get; }

        DateTime ExpirationTime { get; }

        bool IsAvailable { get; }

        string FormattedPrice { get; }

        string ExpirationTimeFormatted { get; }
    }
}
