using Eberhard.Core.Utilities;
using System;

namespace Eberhard.Core.Controls.ChartControl
{
    public interface IChartTimeBlock
    {
        bool CanShowDate { get; set; }
        DateTime Date { get; set; }
    }

    public class ChartTimeBlock : NotifingObject, IChartTimeBlock
    {
        private DateTime _date;

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                FirePropertyChanged();
            }
        }

        private bool _canShowDate;

        public bool CanShowDate
        {
            get { return _canShowDate; }
            set
            {
                _canShowDate = value;
                FirePropertyChanged();
            }
        }
    }
}