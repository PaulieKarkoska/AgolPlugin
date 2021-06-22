using Autocad = Autodesk.AutoCAD.Colors;
using Media = System.Windows.Media;

namespace AgolPlugin.Converters.Cs
{
    public static class ColorExtensions
    {
        public static Media.Color ToMediaColor(this Autocad.Color color)
        {
            return Media.Color.FromArgb(color.ColorValue.A,
                                        color.ColorValue.R,
                                        color.ColorValue.G,
                                        color.ColorValue.B);
        }

        public static Autocad.Color ToAcadColor(this Media.Color color)
        {
            return Autocad.Color.FromColor(color);
        }

        public static Autocad.Color WhiteColor()
        {
            return Autocad.Color.FromColorIndex(Autocad.ColorMethod.ByAci, 7);
        }
    }
}