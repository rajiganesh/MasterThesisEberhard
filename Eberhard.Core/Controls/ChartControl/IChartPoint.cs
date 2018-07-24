using Eberhard.Core.BaseTypes;
using Eberhard.Core.Utilities;
using System;
using System.Windows;

namespace Eberhard.Core.Controls.ChartControl
{
    public interface IChartPoint
    {
        double Value { get; }
        DateTime TimeStamp { get; }
        Point DisplayPosition { get; set; }
        string PinName { get; set; }

        string PinType { get; set; }
    }

    public class ChartPoint : NotifingObject, IChartPoint
    {
       
        public ChartPoint(double value, DateTime timeStamp)
        {
            TimeStamp = timeStamp;
            Value = value;
        }
        public ChartPoint(double value, DateTime timeStamp, Point position)
            :this(value, timeStamp)
        {
            DisplayPosition = position;

        }

        public double Value { get; private set; }

        public DateTime TimeStamp { get; private set; }

        public Point DisplayPosition { get; set; }

        public string PinName { get; set; }

        public string PinType { get; set; }
    }
}