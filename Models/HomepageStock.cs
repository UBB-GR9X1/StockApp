namespace StockApp.Models
{
    using Microsoft.UI;
    using Microsoft.UI.Xaml.Media;

    public class HomepageStock : IHomepageStock
    {
        public string Symbol { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int Price { get; set; }

        public string Change { get; set; } = string.Empty;

        public bool IsFavorite { get; set; }

        public SolidColorBrush ChangeColor
            => Change.StartsWith('+')
                ? new SolidColorBrush(Colors.Green)
                : new SolidColorBrush(Colors.Red);

        public string FavoriteStar
            => IsFavorite ? "★" : "☆";
    }
}
