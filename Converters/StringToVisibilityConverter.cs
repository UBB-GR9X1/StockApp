namespace StockApp.Converters
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;

    public partial class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not string stringValue)
            {
                return Visibility.Collapsed;
            }

            bool isEmpty = string.IsNullOrEmpty(stringValue);

            // if parameter is "Inverse", invert the logic
            if (parameter is string param && param == "Inverse")
            {
                isEmpty = !isEmpty;
            }

            return isEmpty
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}