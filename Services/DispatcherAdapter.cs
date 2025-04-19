namespace StockApp.Services
{
    using Microsoft.UI.Dispatching;

    public class DispatcherAdapter : IDispatcher
    {
        private readonly DispatcherQueue _dq;

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherAdapter"/> class.
        /// </summary>
        public DispatcherAdapter() => _dq = DispatcherQueue.GetForCurrentThread();

        /// <summary>
        /// Enqueues the specified handler to be executed on the dispatcher queue.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public bool TryEnqueue(DispatcherQueueHandler handler)
            => _dq.TryEnqueue(handler);
    }
}
