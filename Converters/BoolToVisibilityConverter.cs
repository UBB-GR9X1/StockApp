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

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is not Visibility visibilityValue)
            {
                return false;
            }

            return ShouldInvert(parameter)
                ? visibilityValue != Visibility.Visible
                : visibilityValue == Visibility.Visible;
        }
    }
}
