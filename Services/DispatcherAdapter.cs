namespace StockApp.Services
{
    using Microsoft.UI.Dispatching;

    public class DispatcherAdapter : IDispatcher
    {
        private readonly DispatcherQueue dispatcherQueue;

        public DispatcherAdapter() => this.dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        public bool TryEnqueue(DispatcherQueueHandler handler)
            => this.dispatcherQueue.TryEnqueue(handler);
    }
}
