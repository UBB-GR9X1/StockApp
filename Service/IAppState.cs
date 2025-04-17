namespace StockApp.Service
{
    using StockApp.Models;

    public interface IAppState
    {
        User CurrentUser { get; set; }
    }
}
