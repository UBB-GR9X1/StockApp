namespace StockApp.Services
{
    using System;
    using Microsoft.UI.Xaml.Controls;

    public class NavigationService : INavigationService
    {
        private static readonly Lazy<NavigationService> instance = new(() => new NavigationService());

        /// <summary>
        /// Singleton instance of the NavigationService.
        /// </summary>
        public static NavigationService Instance => instance.Value;

        /// <summary>
        /// The root frame for navigation.
        /// </summary>
        private static INavigationFrame RootFrame;

        // Private constructor to enforce singleton pattern
        private NavigationService()
        {
        }

        /// <summary>
        /// Initializes the NavigationService with the specified frame.
        /// </summary>
        /// <param name="frame"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Initialize(INavigationFrame frame)
        {
            RootFrame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        /// <summary>
        /// Navigates to the specified page type with an optional parameter.
        /// </summary>
        /// <param name="pageType"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool Navigate(Type pageType, object? parameter = null)
        {
            if (RootFrame == null)
            {
                throw new InvalidOperationException("NavigationService not initialized. Call Initialize first.");
            }

            return RootFrame.Navigate(pageType, parameter);
        }

        /// <summary>
        /// Navigates back in the navigation stack.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void GoBack()
        {
            if (RootFrame == null)
            {
                throw new InvalidOperationException("NavigationService not initialized. Call Initialize first.");
            }

            if (RootFrame.CanGoBack)
            {
                RootFrame.GoBack();
            }
        }

        /// <summary>
        /// Checks if the navigation stack can go back.
        /// </summary>
        public bool CanGoBack => RootFrame?.CanGoBack ?? false;

        /// <summary>
        /// Navigates to the article detail page with the specified article ID.
        /// </summary>
        /// <param name="articleId"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void NavigateToArticleDetail(string articleId)
        {
            if (RootFrame == null)
            {
                throw new InvalidOperationException("NavigationService not initialized. Call Initialize first.");
            }

            Navigate(typeof(StockApp.Views.NewsArticleView), articleId);
        }
    }
}