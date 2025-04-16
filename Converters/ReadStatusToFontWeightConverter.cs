namespace StockApp.Converters
{
    using System;
    using Microsoft.UI.Text;
    using Microsoft.UI.Xaml.Data;

    public partial class ReadStatusToFontWeightConverter : IValueConverter
    {
        public object Convert(object initialValue, Type targetType, object parameter, string language)
        {
            if (initialValue is not bool isReading)
            {
                return FontWeights.Normal;
            }

            // if read return normal, else return semi bold font for article title
            return isReading
                ? FontWeights.Normal
                : FontWeights.SemiBold;
        }

        public object ConvertBack(object initialValue, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}