using Eberhard.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Eberhard.Core.Controls.ChartControl
{
    public interface IChartLine : ICollection<IChartPoint>
    {

        Brush LineColor { get; }

        string PinName { get; }
    }
    public class ChartLine : NotifingCollection<IChartPoint>, IChartLine
    {
        public ChartLine(string name, IEnumerable<IChartPoint> points, Brush lineColor)
            : base(points)
        {
            LineColor = lineColor;
            PinName = name;

        }

        public Brush LineColor { get; private set; }

        public string PinName { get; private set; }
    }


}
