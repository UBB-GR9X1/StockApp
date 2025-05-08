namespace StockApp.Services
{
    using Microsoft.UI.Dispatching;

    public interface IDispatcher
    {
        bool TryEnqueue(DispatcherQueueHandler handler);
    }
}
