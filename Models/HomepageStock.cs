namespace StockApp.Models
{
    using Microsoft.UI;
    using Microsoft.UI.Xaml.Media;

    public class HomepageStock(string symbol, string name, int price, string change, bool isFavorite)
    {
        public string Symbol { get; set; } = symbol;

        public string Name { get; set; } = name;

        public int Price { get; set; } = price;

        public string Change { get; set; } = change;

        public bool IsFavorite { get; set; } = isFavorite;

        public SolidColorBrush ChangeColor => this.Change.StartsWith('+')
                ? new SolidColorBrush(Colors.Green)
                : new SolidColorBrush(Colors.Red);

        public string FavoriteStar => this.IsFavorite ? "★" : "☆";
    }
}
