namespace StockApp.Models
{
    public interface IUserStock : IBaseStock
    {
        int Quantity { get; set; }
    }
}
