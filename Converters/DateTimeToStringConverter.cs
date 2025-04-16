namespace StockApp.Converters
{
    using System;

    public partial class DateTimeToStringConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            string format = parameter as string ?? "MMMM dd, yyyy";

            return value is DateTime dateTimeValue
                ? dateTimeValue.ToString(format)
                : string.Empty;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is string stringValue
                ? ParseOrDefault(stringValue)
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
