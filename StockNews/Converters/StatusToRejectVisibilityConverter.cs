using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace StockNewsPage.Converters
{
    public class StatusToRejectVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string status)
            {
                // reject button only if status is not already "Rejected"
                return status != "Rejected" ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}