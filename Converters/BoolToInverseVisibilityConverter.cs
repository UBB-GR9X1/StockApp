namespace StockApp.Converters
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;

    public partial class BoolToInverseVisibilityConverter : IValueConverter
    {
        public object Convert(object inputValue, Type targetType, object parameter, string language)
        {
            return inputValue is bool inputBoolean && inputBoolean
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public object ConvertBack(object inputValue, Type targetType, object parameter, string language)
        {
            return inputValue is Visibility inputVisibility && inputVisibility == Visibility.Collapsed;
        }
    }
}
