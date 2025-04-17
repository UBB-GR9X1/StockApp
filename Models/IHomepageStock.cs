namespace StockApp.Models
{
    using Microsoft.UI.Xaml.Media;

    public interface IHomepageStock
    {
        string Symbol { get; set; }

        string Name { get; set; }

        int Price { get; set; }

        string Change { get; set; }

        bool IsFavorite { get; set; }

        SolidColorBrush ChangeColor { get; }

        string FavoriteStar { get; }
    }
}
