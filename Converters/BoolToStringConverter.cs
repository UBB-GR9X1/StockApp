namespace StockApp.Converters
{
    using System;
    using Microsoft.UI.Xaml.Data;

    public partial class BoolToStringConverter : IValueConverter
    {
        public object Convert(object initialValue, Type targetType, object parameter, string language)
        {
            if (initialValue is bool initialBoolean && parameter is string options)
            {
                var parts = options.Split('|');
                if (parts.Length == 2)
                {
                    return initialBoolean ? parts[0] : parts[1];
                }
            }

            return initialValue?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}