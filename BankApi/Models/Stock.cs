using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApi.Models
{
    /// <summary>
    /// Represents a stock holding, including its name, symbol, author, price per share, and quantity.
    /// </summary>
    /// <param name="name">The display name of the stock.</param>
    /// <param name="symbol">The trading symbol of the stock.</param>
    /// <param name="authorCNP">The CNP identifier of the author who created this entry.</param>
    /// <param name="price">The purchase price of each share.</param>
    /// <param name="quantity">The number of shares held.</param>
    public class Stock
    {
        public Stock()
        {
            StockValues = new List<StockValue>();
            UserStocks = new List<UserStock>();
            FavoriteStocks = new List<FavoriteStock>();
        }

        public Stock(string stockName, string stockSymbol, string authorCNP, int currentPrice, int quantity)
            : this()
        {
            StockName = stockName;
            StockSymbol = stockSymbol;
            AuthorCNP = authorCNP;
            CurrentPrice = currentPrice;
            Quantity = quantity;
        }

        [Key]
        public string StockName { get; set; }
        public string StockSymbol { get; set; }
        public string AuthorCNP { get; set; }
        public int CurrentPrice { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        
        [ForeignKey("AuthorCNP")]
        public User Author { get; set; }
        
        public ICollection<StockValue> StockValues { get; set; }
        public ICollection<UserStock> UserStocks { get; set; }
        public ICollection<FavoriteStock> FavoriteStocks { get; set; }

        /// <summary>
        /// Returns a string that represents the current stock,
        /// including its name, symbol, quantity, and price.
        /// </summary>
        public override string ToString()
        {
            return $"{StockName} ({StockSymbol}) - x{Quantity} at {CurrentPrice}";
        }
    }

    public class StockValue
    {
        [Key]
        public int Id { get; set; }
        public string StockName { get; set; }
        public int Price { get; set; }
        public DateTime Timestamp { get; set; }
        
        [ForeignKey("StockName")]
        public Stock Stock { get; set; }
    }

    public class FavoriteStock
    {
        [Key]
        public int Id { get; set; }
        public string UserCNP { get; set; }
        public string StockName { get; set; }
        
        [ForeignKey("UserCNP")]
        public User User { get; set; }
        
        [ForeignKey("StockName")]
        public Stock Stock { get; set; }
    }
}
