namespace StockApp.Services
{
    using System;
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameAdapter"/> class.
    /// </summary>
    /// <param name="frame"></param>
    public class FrameAdapter(Frame frame) : INavigationFrame
    {
        private readonly Frame frame = frame;

        /// <summary>
        /// Navigates to the specified page type with the given parameter.
        /// </summary>
        /// <param name="pageType"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool Navigate(Type pageType, object parameter) => this.frame.Navigate(pageType, parameter);

    /// <summary>
    /// Navigates back in the frame's navigation history.
    /// </summary>
    public void GoBack() => this.frame.GoBack();

    /// <summary>
    /// Checks if the frame can navigate back in its navigation history.
    /// </summary>
    public bool CanGoBack => this.frame.CanGoBack;
    }
}
