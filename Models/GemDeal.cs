namespace StockApp.Models
{
    public class GemDeal(string title, int gemAmount, double price, bool isSpecial)
    {
        public string Title { get; set; } = title;

        public int GemAmount { get; set; } = gemAmount;

        public double Price { get; set; } = price;

        public bool IsSpecial { get; set; } = isSpecial;
    }
}
