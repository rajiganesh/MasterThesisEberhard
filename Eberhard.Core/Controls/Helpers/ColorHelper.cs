using System.Windows.Media;

namespace Eberhard.Core.Controls.Helpers
{
    /// <summary>
    /// Provides optimized (frozen) pens and brushes.
    /// </summary>
    internal static class ColorHelper
    {
        public static readonly Pen Error = CreatePen(Colors.Red, 2d, 0.3);
        public static readonly Pen Warning = CreatePen(Colors.Yellow, 1d, 0.3);
        public static readonly Pen Neutral = CreatePen(Color.FromRgb(107, 107, 107), 1);
        public static readonly Pen White = CreatePen(Colors.White, 1d);

        public static readonly Brush Green = CreateBrush(Color.FromRgb(22, 122, 77), Color.FromRgb(0, 142, 49));

        private static Brush CreateBrush(Color color1, Color color2)
        {
            var brush = new LinearGradientBrush(color1, color2, 0d);
            brush.Freeze();
            return brush;
        }

        private static Pen CreatePen(Color color, double width, double opacity = 1d)
        {
            var brush = new SolidColorBrush(color) { Opacity = opacity };
            brush.Freeze();

            var pen = new Pen(brush, width);
            pen.Freeze();

            return pen;
        }
    }
}
