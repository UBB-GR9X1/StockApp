namespace StockApp.Models
{
    public class Alert(int alertId, string stockName, string name, int upperBound, int lowerBound, bool toggleOnOff)
    {
        required public int AlertId { get; set; } = alertId; // Primary Key

        public string StockName { get; set; } = stockName;

        public string Name { get; set; } = name;

        public int UpperBound { get; set; } = upperBound;

        public int LowerBound { get; set; } = lowerBound;

        public bool ToggleOnOff { get; set; } = toggleOnOff;
    }
}
