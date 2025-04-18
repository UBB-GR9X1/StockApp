namespace StockApp.Converters
{
    using System;
    using Microsoft.UI.Xaml;

    public partial class StringToVisibilityConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not string stringValue)
            {
                return Visibility.Collapsed;
            }

            bool isEmpty = string.IsNullOrEmpty(stringValue);

            // if parameter is "Inverse", invert the logic
            if (ShouldInvert(parameter))
            {
                isEmpty = !isEmpty;
            }

            return isEmpty
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("ConvertBack is not implemented in StringToVisibilityConverter.");
        }
    }
}
