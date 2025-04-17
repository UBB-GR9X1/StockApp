namespace StockApp.Models
{
    public interface ITriggeredAlert
    {
        string StockName { get; set; }

        string Message { get; set; }
    }
}
