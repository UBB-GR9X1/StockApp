namespace StockApp.Services
{
    using StockApp.Models;

    public interface IAppState
    {
        User CurrentUser { get; set; }
    }
}
