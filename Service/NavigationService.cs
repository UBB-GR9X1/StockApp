namespace StockApp.Service
{
    using System;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Navigation;

    public class NavigationService
    {
        private static readonly Lazy<NavigationService> instance = new (() => new NavigationService());

        public static NavigationService Instance => instance.Value;

        private Frame frame;

        // Private constructor to enforce singleton pattern
        private NavigationService()
        {
        }

        public void Initialize(Frame frame)
        {
            _frame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        public bool Navigate(Type pageType, object? parameter = null)
        {
            if (this.frame == null)
            {
                throw new InvalidOperationException("NavigationService not initialized. Call Initialize first.");
            }

            return this.frame.Navigate(pageType, parameter);
        }

        public void GoBack()
        {
            if (this.frame == null)
            {
                throw new InvalidOperationException("NavigationService not initialized. Call Initialize first.");
            }

            if (this.frame.CanGoBack)
            {
                this.frame.GoBack();
            }
        }

        public bool CanGoBack => this.frame?.CanGoBack ?? false;
    }
}