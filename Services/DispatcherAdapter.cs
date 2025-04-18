namespace StockApp.Services
{
    using Microsoft.UI.Dispatching;

    public class DispatcherAdapter : IDispatcher
    {
        private readonly DispatcherQueue _dq;

        public DispatcherAdapter() => _dq = DispatcherQueue.GetForCurrentThread();

        public bool TryEnqueue(DispatcherQueueHandler handler)
            => _dq.TryEnqueue(handler);
    }
}
