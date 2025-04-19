namespace StockApp.Converters
{
    using System;
    using Microsoft.UI.Text;

    public partial class ReadStatusToFontWeightConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not bool statusIsReading)
            {
                throw new InvalidCastException("Expected a boolean value for ReadStatusToFontWeightConverter.");
            }

            // if read return normal, else return semi bold font for article title
            return statusIsReading
                ? FontWeights.Normal
                : FontWeights.SemiBold;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("ConvertBack is not implemented in ReadStatusToFontWeightConverter.");
        }
    }
}
