namespace StockApp.Converters
{
    using System;
    using StockApp.Exceptions;

    public partial class DateTimeToStringConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            string format = parameter as string ?? "MMMM dd, yyyy";

            if (value is not DateTime dateTimeValue)
            {
                throw new InvalidCastException("Expected DateTime value for DateTimeToStringConverter.");
            }

            return dateTimeValue.ToString(format);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is not string stringValue)
            {
                throw new InvalidCastException("Expected a string value for date conversion.");
            }

            return ParseOrDefault(stringValue);
        }

        private static DateTime ParseOrDefault(string input)
        {
            if (DateTime.TryParse(input, out DateTime result))
            {
                return result;
            }

            throw new ConverterException($"Invalid date format: '{input}'. Unable to parse.");
        }
    }
}
