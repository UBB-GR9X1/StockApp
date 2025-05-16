using System;
using Microsoft.UI.Xaml.Data;

namespace StockApp.Converters
{
    public class DateTimeOffsetToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset.DateTime;
            }
            if (value is DateTime dateTime)
            {
                return dateTime;
            }
            return DateTime.Now;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime)
            {
                return new DateTimeOffset(dateTime);
            }
            if (value is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset;
            }
            return DateTimeOffset.Now;
        }
    }
} 