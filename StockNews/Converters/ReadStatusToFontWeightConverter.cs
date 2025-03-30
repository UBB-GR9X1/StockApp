using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Data;
using System;

namespace StockNewsPage.Converters
{
    public class ReadStatusToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // if read return normal, else return semi bold font for article title
            if (value is bool isRead)
            {
                return isRead ? FontWeights.Normal : FontWeights.SemiBold;
            }
            
            return FontWeights.Normal;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

