using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace StockNewsPage.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string stringValue)
            {
                bool isEmpty = string.IsNullOrEmpty(stringValue);

                // if parameter is "Inverse", invert the logic
                if (parameter is string param && param == "Inverse")
                {
                    return isEmpty ? Visibility.Visible : Visibility.Collapsed;
                }

                return isEmpty ? Visibility.Collapsed : Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}