namespace StockApp.Views.Converters
{
    using System;
    using Microsoft.UI;
    using Microsoft.UI.Xaml.Data;
    using Microsoft.UI.Xaml.Media;
    using Windows.UI;

    internal class BoolToFavoriteColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string lang)
        {
            if (value is bool boolValue)
            {
                if (parameter is string colorNames)
                {
                    try
                    {
                        var colors = colorNames.Split('|');
                        if (boolValue)
                        {
                            return new SolidColorBrush(ConvertStringToColor(colors[0]));
                        }
                        else
                        {
                            return new SolidColorBrush(ConvertStringToColor(colors[1]));
                        }
                    }
                    catch
                    {
                        return new SolidColorBrush(Colors.Transparent);
                    }
                }
                else
                {
                    // Default colors if no parameter is provided
                    if (boolValue)
                    {
                        return new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        return new SolidColorBrush(Colors.Red);
                    }
                }
            }

            throw new InvalidOperationException("Value must be a boolean.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string lang)
        {
            throw new NotImplementedException("ConvertBack is not supported.");
        }

        private static Color ConvertStringToColor(string colorName)
        {
            // Use a predefined method to convert string to Color
            var colorProperty = typeof(Colors).GetProperty(colorName);
            if (colorProperty != null && colorProperty.GetValue(null) is Color colorValue)
            {
                return colorValue;
            }

            throw new ArgumentException($"Invalid color name: {colorName}");
        }
    }
}
