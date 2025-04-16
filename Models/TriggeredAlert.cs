namespace StockApp.Models
{
    public class TriggeredAlert(string stockName, string message)
    {
        public string StockName { get; set; } = stockName;

        public string Message { get; set; } = message;
    }
}
