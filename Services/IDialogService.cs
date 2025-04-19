namespace StockApp.Services
{
    using System.Threading.Tasks;

    internal interface IDialogService
    {
        Task ShowMessageAsync(string title, string content);
    }
}
