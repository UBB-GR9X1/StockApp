namespace StockApp.Converters
{
    using System;
    using Microsoft.UI.Xaml;

    public partial class BoolToVisibilityConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not bool booleanValue)
            {
                throw new InvalidCastException("Expected a boolean value for BoolToVisibilityConverter.");
            }

            if (ShouldInvert(parameter))
            {
                booleanValue = !booleanValue;
            }

            return booleanValue
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is not Visibility visibilityValue)
            {
                throw new InvalidCastException("Expected a Visibility value for BoolToVisibilityConverter.");
            }

            return ShouldInvert(parameter)
                ? visibilityValue != Visibility.Visible
                : visibilityValue == Visibility.Visible;
        }
    }
}
