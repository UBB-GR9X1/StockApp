using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Model
{
    public class HomepageStock
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Change { get; set; }
        public bool isFavorite { get; set; }
        public HomepageStock()
        {
            Symbol = "";
            Name = "";
            Price = 0;
            Change = "";
            isFavorite = false;
        }
        public SolidColorBrush ChangeColor
        {
            get => Change.StartsWith("+") ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
        }
        public string FavoriteStar
        {
            get => isFavorite ? "★" : "☆";
        }
    }
}


