using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AgolPlugin.Converters.Xaml
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NullToVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object param, CultureInfo culture)
        {
            var vis = value == null;
            vis = param as string == "Reverse" ? !vis : vis;

            return vis ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object param, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
