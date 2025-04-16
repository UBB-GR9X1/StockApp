using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace StockApp.Converters
{
    public partial class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object inputValue, Type targetType, object parameter, string language)
        {
            if (inputValue is not bool inputBoolean)
            {
                return Visibility.Collapsed;
            }

            if (ShouldInvert(parameter))
            {
                inputBoolean = !inputBoolean;
            }

            return inputBoolean ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object inputValue, Type targetType, object parameter, string language)
        {
            if (inputValue is not Visibility inputVisibility)
            {
                return false;
            }

            if (ShouldInvert(parameter))
            {
                return inputVisibility != Visibility.Visible;
            }

            return inputVisibility == Visibility.Visible;
        }

        private static bool ShouldInvert(object parameter)
        {
            if (parameter is string paramString)
            {
                return paramString.Equals("Inverse", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}