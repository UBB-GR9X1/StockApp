namespace StockApp.Converters
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;

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

            return inputBoolean
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object inputValue, Type targetType, object parameter, string language)
        {
            if (inputValue is not Visibility inputVisibility)
            {
                return false;
            }

            return ShouldInvert(parameter)
                ? inputVisibility != Visibility.Visible
                : inputVisibility == Visibility.Visible;
        }

        private static bool ShouldInvert(object parameter)
        {
            return parameter is string paramString && paramString.Equals("Inverse", StringComparison.OrdinalIgnoreCase);
        }
    }
}