namespace StockApp.Services
{
    using System;
    using Microsoft.UI.Xaml.Controls;

    public class NavigationService : INavigationService
    {
        private static readonly Lazy<NavigationService> instance = new(() => new NavigationService());

        public static NavigationService Instance => instance.Value;

        private static INavigationFrame rootFrame;

        // Private constructor to enforce singleton pattern
        private NavigationService()
        {
        }

        public static void Initialize(INavigationFrame frame)
        {
            rootFrame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        public bool Navigate(Type pageType, object? parameter = null)
        {
            if (rootFrame == null)
            {
                throw new InvalidOperationException("NavigationService not initialized. Call Initialize first.");
            }

            return rootFrame.Navigate(pageType, parameter);
        }

        public void GoBack()
        {
            if (rootFrame == null)
            {
                throw new InvalidOperationException("NavigationService not initialized. Call Initialize first.");
            }

            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        public bool CanGoBack => rootFrame?.CanGoBack ?? false;

        public void NavigateToArticleDetail(string articleId)
        {
            if (rootFrame == null)
            {
                throw new InvalidOperationException("NavigationService not initialized. Call Initialize first.");
            }

            this.Navigate(typeof(StockApp.Views.NewsArticleView), articleId);
        }
    }
}