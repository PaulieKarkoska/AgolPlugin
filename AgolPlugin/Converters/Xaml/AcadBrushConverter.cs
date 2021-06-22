using AgolPlugin.Converters.Cs;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Acad = Autodesk.AutoCAD.Colors;

namespace AgolPlugin.Converters.Xaml
{
    [ValueConversion(typeof(Acad.Color), typeof(System.Drawing.Color))]
    public class AcadBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
#if DEBUG
            try
            {
#endif
                return value == null ?
                Brushes.Transparent :
                new SolidColorBrush(((Acad.Color)value).ToMediaColor());
#if DEBUG
            }
            catch { return new object(); }
#endif
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}