using System;
using System.Globalization;
using Xamarin.Forms;

namespace Thewissen.PancakeViewSample.Converters
{
    public class IntToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int colorInt)
            {
                var color = Color.FromUint((uint)colorInt);
                return new Color(color.R, color.G, color.B, 1);
            }

            return Color.Default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
