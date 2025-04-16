using Microsoft.UI.Xaml.Data;
using System;

namespace StockApp.Converters
{
    public partial class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object initialValue, Type targetType, object parameter, string language)
        {
            string format = parameter as string ?? "MMMM dd, yyyy";

            return initialValue is DateTime initialDateTime
                ? initialDateTime.ToString(format)
                : string.Empty;
        }

        public object ConvertBack(object initialValue, Type targetType, object parameter, string language)
        {
            return initialValue is string initialString
                ? ParseOrDefault(initialString)
                : DateTime.Now;
        }

        private static DateTime ParseOrDefault(string input)
        {
            return DateTime.TryParse(input, out DateTime result)
                ? result
                : DateTime.Now;
        }
    }
}
