using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;

namespace StockApp.Views
{
    public class Stock
    {
        public Stock(string name, string symbol, float quantity, float price, string authorCNP)
        {
            Name = name;
            Symbol = symbol;
            Quantity = quantity;
            Price = price;
            AuthorCNP = authorCNP;
        }

        public string Name { get; set; }
        public string Symbol { get; set; }
        public float Quantity { get; set; }
        public float Price { get; set; }
        public string AuthorCNP { get; set; }
    }
}
