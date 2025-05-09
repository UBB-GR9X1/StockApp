namespace StockApp.Views.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class FavoriteToStarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isFavorite)
            {
                return isFavorite ? "★" : "☆";
            }

            return "☆"; // Default to outline star
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
