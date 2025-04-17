namespace StockApp.Models
{
    public class TriggeredAlert : ITriggeredAlert
    {
        public string StockName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
