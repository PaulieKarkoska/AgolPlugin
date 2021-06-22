using System;
using System.Windows.Data;
using System.Drawing;
using Acad = Autodesk.AutoCAD.Colors;
using System.Globalization;
using AgolPlugin.Converters.Cs;

namespace AgolPlugin.Converters.Xaml
{
    [ValueConversion(typeof(Acad.Color), typeof(Color))]
    public class AcadColorToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Acad.Color)value;

            return color.ToMediaColor();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}