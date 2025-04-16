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
                return FontWeights.Normal;
            }

            // if read return normal, else return semi bold font for article title
            return statusIsReading
                ? FontWeights.Normal
                : FontWeights.SemiBold;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
