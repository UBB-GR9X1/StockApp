namespace StockApp.Converters
{
    using System;
    using Microsoft.UI.Xaml;

    internal partial class StatusToVisibilityConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not string status || parameter is not string expectedStatus)
            {
                return Visibility.Collapsed;
            }

            return status == expectedStatus
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("ConvertBack is not implemented in StatusToVisibilityConverter.");
        }
    }
}
