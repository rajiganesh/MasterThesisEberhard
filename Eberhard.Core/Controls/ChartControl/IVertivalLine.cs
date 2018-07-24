namespace Eberhard.Core.Controls.ChartControl
{
    public interface IVertivalLine
    {
        double Top { get; set; }
        LineType Type { get; set; }
    }

    public class VertivalLine : IVertivalLine
    {
        public LineType Type { get; set; }
        public double Top { get; set; }
    }
}