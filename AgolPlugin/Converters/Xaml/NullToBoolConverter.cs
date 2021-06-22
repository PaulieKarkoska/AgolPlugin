using System;
using System.Globalization;
using System.Windows.Data;

namespace AgolPlugin.Converters.Xaml
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object param, CultureInfo culture)
        {
            if (param is string p && p == "Reverse")
                return value == null;
            else
                return value != null;
        }

        public object ConvertBack(object value, Type targetType, object param, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
