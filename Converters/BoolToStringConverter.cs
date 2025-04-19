namespace StockApp.Converters
{
    using System;

    public partial class BoolToStringConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool booleanValue && parameter is string options)
            {
                var parts = options.Split('|');
                if (parts.Length == 2)
                {
                    return booleanValue ? parts[0] : parts[1];
                }
            }

            return value?.ToString() ?? string.Empty;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
