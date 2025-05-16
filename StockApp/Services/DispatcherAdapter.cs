namespace StockApp.Services
{
    using Microsoft.UI.Dispatching;

    public class DispatcherAdapter : IDispatcher
    {
        private readonly DispatcherQueue dispatcherQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherAdapter"/> class.
        /// </summary>
        public DispatcherAdapter() => this.dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        /// <summary>
        /// Enqueues the specified handler to be executed on the dispatcher queue.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public bool TryEnqueue(DispatcherQueueHandler handler)
            => this.dispatcherQueue.TryEnqueue(handler);
    }
}
