namespace StockApp.Models
{
    using Microsoft.UI;
    using Microsoft.UI.Xaml.Media;

    public class HomepageStock()
    {
        public string Symbol { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        public string Change { get; set; }

        public bool IsFavorite { get; set; }

        public SolidColorBrush ChangeColor => this.Change.StartsWith('+')
                ? new SolidColorBrush(Colors.Green)
                : new SolidColorBrush(Colors.Red);

        public string FavoriteStar => this.IsFavorite ? "★" : "☆";
    }
}
