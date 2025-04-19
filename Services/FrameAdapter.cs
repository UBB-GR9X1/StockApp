namespace StockApp.Services
{
    using System;
    using Microsoft.UI.Xaml.Controls;

    public class FrameAdapter : INavigationFrame
    {
        private readonly Frame frame;

        public FrameAdapter(Frame frame)
        {
            this.frame = frame;
        }

        public bool Navigate(Type pageType, object parameter) => this.frame.Navigate(pageType, parameter);

        public void GoBack() => this.frame.GoBack();

        public bool CanGoBack => this.frame.CanGoBack;
    }
}
