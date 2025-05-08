namespace StockApp.Services
{
    using System.Threading.Tasks;

    public interface IDialogService
    {
        Task ShowMessageAsync(string title, string content);
    }
}
