namespace StockApp.Converters
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;

    public partial class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not bool booleanValue)
            {
                return Visibility.Collapsed;
            }

            if (ShouldInvert(parameter))
            {
                booleanValue = !booleanValue;
            }

            return booleanValue
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is not Visibility visibilityValue)
            {
                return false;
            }

            return ShouldInvert(parameter)
                ? visibilityValue != Visibility.Visible
                : visibilityValue == Visibility.Visible;
        }

        private static bool ShouldInvert(object parameter)
        {
            return parameter is string stringParameter
                && stringParameter.Equals("Inverse", StringComparison.OrdinalIgnoreCase);
        }
    }
}