using System.Collections.Generic;

namespace Eberhard.Core.Controls.ChartControl
{
    public interface IChartVerticalAxis
    {
        double ErrorTolerance { get; }
        IEnumerable<IAxisTick> LabelSource { get; }
        IEnumerable<IVertivalLine> LinesSource { get; }
        double PointToPixelRatio { get; }
        double TickRatio { get; }
        double WarningTolerance { get; }
        double ZeroPosition { get; }
        double MiniTick { get; }
    }

    public class ChartVerticalAxis : IChartVerticalAxis
    {
        public IEnumerable<IAxisTick> LabelSource { get; internal set; }

        public IEnumerable<IVertivalLine> LinesSource { get; internal set; }
        public double ZeroPosition { get; internal set; }
        public double TickRatio { get; internal set; }
        public double PointToPixelRatio { get; internal set; }

        public double ErrorTolerance { get; internal set; }
        public double WarningTolerance { get; internal set; }
        public double MiniTick { get; internal set; }
    }
}