using System;
using System.Collections.Generic;

namespace BankApi.Models
{
    public class StockPage
    {
        public int Id { get; set; }
        public string StockName { get; set; }
        public string StockSymbol { get; set; }
        public string AuthorCNP { get; set; }
        public decimal CurrentPrice { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public User Author { get; set; }
        public Stock Stock { get; set; }
        public ICollection<StockValue> StockValues { get; set; }
        public ICollection<UserStock> UserStocks { get; set; }
        public ICollection<FavoriteStock> FavoriteStocks { get; set; }
    }
} 