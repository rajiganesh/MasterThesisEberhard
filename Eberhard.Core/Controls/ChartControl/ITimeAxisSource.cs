using Eberhard.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;

namespace Eberhard.Core.Controls.ChartControl
{
    public interface ITimeAxisSource : ICollection<IChartTimeBlock>
    {
        int TotalMinutes { get; }

        DateTime StartingDate { get; }

        double GetPositionOfMinute(DateTime date);
    }

    public class TimeAxisSource : NotifingCollection<IChartTimeBlock>, ITimeAxisSource
    {
        public TimeAxisSource(IEnumerable<IChartTimeBlock> timeBlcok)
            : base(timeBlcok)
        {
            //Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(
            //    this, "CollectionChanged")
            //    .Where(eve => eve.EventArgs.Action == NotifyCollectionChangedAction.Add)
            //    .Subscribe(args =>
            //    {
            //    });
        }

        public TimeAxisSource()
            : base()
        {
        }

        public int TotalMinutes => Count * 60;

        public DateTime StartingDate => this.FirstOrDefault().Date;

        /// <summary>
        /// Gets the position of a given minute on the Y-axis in the chart.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>the position of the minute in the colleciton
        /// multiplies by 5 pixels (each minute is represented by 5 pixs
        /// </returns>
        public double GetPositionOfMinute(DateTime date) =>
            Math.Round(date.Subtract(StartingDate).TotalSeconds * 5.0);


    }
}