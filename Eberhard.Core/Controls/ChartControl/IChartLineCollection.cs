using Eberhard.Core.Utilities;
using System.Collections.Generic;

namespace Eberhard.Core.Controls.ChartControl
{
    public interface IChartLineCollection : ICollection<IChartLine>
    {
    }

    public class ChartLineCollection : NotifingCollection<IChartLine>, IChartLineCollection
    {
        public ChartLineCollection()
        {

        }

        public ChartLineCollection(IEnumerable<IChartLine> collection)
            :base(collection)
        {

        }
    }
}