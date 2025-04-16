using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace StockApp.Converters
{
    public partial class BoolToInverseVisibilityConverter : IValueConverter
    {
        public object Convert(object inputValue, Type targetType, object parameter, string language)
        {
            if (inputValue is not bool inputBoolean)
            {
                return Visibility.Visible;
            }

            return inputBoolean ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object inputValue, Type targetType, object parameter, string language)
        {
            if (inputValue is not Visibility inputVisibility)
            {
                return false;
            }

            return inputVisibility == Visibility.Collapsed;
        }
    }
}