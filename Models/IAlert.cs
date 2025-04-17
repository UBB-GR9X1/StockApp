namespace StockApp.Models
{
    public interface IAlert
    {
        int AlertId { get; set; }

        string StockName { get; set; }

        string Name { get; set; }

        decimal UpperBound { get; set; }

        decimal LowerBound { get; set; }

        bool ToggleOnOff { get; set; }
    }
}
