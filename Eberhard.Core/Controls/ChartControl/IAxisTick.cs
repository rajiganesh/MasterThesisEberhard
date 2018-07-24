namespace Eberhard.Core.Controls.ChartControl
{
    public interface IAxisTick
    {
        double Label { get; set; }
        double Top { get; set; }
    }

    public class AxisTick : IAxisTick
    {
        public double Label { get; set; }
        public double Top { get; set; }
    }
}