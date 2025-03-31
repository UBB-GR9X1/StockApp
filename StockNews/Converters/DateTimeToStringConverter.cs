using Microsoft.UI.Xaml.Data;
using System;

namespace StockNewsPage.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime)
            {
                string format = parameter as string ?? "MMMM dd, yyyy";
                return dateTime.ToString(format);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string dateString && DateTime.TryParse(dateString, out DateTime result))
            {
                return result;
            }

            return DateTime.Now;
        }
    }
}