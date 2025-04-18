namespace StockApp.Services
{
    using System;

    public interface INavigationFrame
    {
        bool Navigate(Type pageType, object parameter);

        void GoBack();

        bool CanGoBack { get; }
    }
}
