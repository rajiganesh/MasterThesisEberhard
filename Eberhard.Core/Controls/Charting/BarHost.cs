using Eberhard.Core.Controls.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using static Eberhard.Core.Controls.Helpers.ColorHelper;

namespace Eberhard.Core.Controls.Charting
{
    /// <summary>
    /// Provides drawing bars for the <see cref="Histogram"/>.
    /// </summary>
    internal class BarHost : GraphHost
    {
        #region Fields

        private double _maxPercentage;
        private List<Bar> _bars;
        private Histogram _parent;

        #endregion

        #region Constructor

        public BarHost()
        {
            _bars = new List<Bar>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the margin between each bar.
        /// </summary>
        public double BarMargin
        {
            get { return (double)GetValue(BarMarginProperty); }
            set { SetValue(BarMarginProperty, value); }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty BarMarginProperty =
            DependencyProperty.Register("BarMargin", typeof(double), typeof(BarHost), new PropertyMetadata(0.5));

        #endregion

        #region Methods

        public void SetParent(Histogram parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Wrapper for updating the data asynchronously.
        /// </summary>
        /// <param name="values">The data.</param>
        /// <param name="errorTolerance">The error tolerance.</param>
        /// <param name="warningTolerance">The warning tolerance.</param>
        /// <returns></returns>
        public Task<GraphStatus> UpdateAsync(double[] values, double errorTolerance, double warningTolerance)
        {
            return Task.Run(() => UpdateSource(values, errorTolerance, warningTolerance));
        }

        private GraphStatus UpdateSource(double[] graphValues, double errorTolerance, double warningTolerance)
        {

            // this method calculates all bars in a three iterations:
            //  - filter out NaN values
            //  - sort
            //  - add bars

            // FILTERING
            var values = graphValues.Where(val => !double.IsNaN(val)).ToArray();

            GraphStatus graphStatus;

            if (graphValues.Length > values.Length)
                graphStatus = GraphStatus.NotAvilable;
            else
                graphStatus = GraphStatus.Normal;

            ////// TEST DATA
            ////// Testdata for bar validation with static values
            ////double[] myTestValues = { 0.321, 0.31, 0.219, 0.185, 0.1, -0.1, -0.1, -0.1, -0.1, -0.239 };
            ////values = myTestValues;

            // SORTING
            // sort array in place from large to small values
            Array.Sort(values, (a, b) => Math.Sign(b - a));

            // ADD BARS

            int i;                          // holds the index of the current value
            double valueCountPerBar = 0,    // holds the amount of values that account for a single bar
                    current = 0;              // holds the current value obtained from values with the index i

            _bars = new List<Bar>();

            // FIRST BAR
            // contains all values that are greater than the error tolerance
            for (i = 0; i < values.Length; i++)
            {
                current = values[i];

                if (current >= errorTolerance)
                    valueCountPerBar++;
                else
                    break;
            }
            AddBar(GraphStatus.Error);

            // SECOND to NEXT-TO-LAST BARS
            // contains all values within positive and negative error tolerance

            // we have to convert to decimal to prevent weird behavior due to doubl arithmetic.
            decimal error = (decimal)errorTolerance;
            decimal warning = (decimal)warningTolerance;
            decimal minTicksDec = (decimal)ScaleCache.Instance.GlobalMinTicks;
            decimal currentScaleTick = error;

            while (currentScaleTick >= -error)
            {

                double x = Convert.ToDouble(currentScaleTick - minTicksDec);
                bool isWarningColorTrue = false;

                for (; i < values.Length; i++)
                {
                    current = values[i];

                    if (current < -errorTolerance)
                        break;

                    if (current > x)
                    {
                        // if the warning tolerance is between bar borders check for the correct color
                        if (!isWarningColorTrue && Math.Abs(current) > warningTolerance)
                            isWarningColorTrue = true;

                        valueCountPerBar++;
                    }
                    else
                        break;

                }

                GraphStatus status = Math.Abs(currentScaleTick) >= warning && isWarningColorTrue ? GraphStatus.Warning : GraphStatus.Normal;

                AddBar(status);
                currentScaleTick -= minTicksDec;
            }

            // LAST BAR
            // contains all values that are smaller than the negative error tolerance
            for (; i < values.Length; i++)
            {
                current = values[i];

                if (current < -errorTolerance)
                    valueCountPerBar++;
                else
                    break;
            }
            AddBar(GraphStatus.Error);


            void AddBar(GraphStatus barStatus)
            {
                var percentage = valueCountPerBar / values.Length;

                if (percentage > _maxPercentage)
                    _maxPercentage = percentage;

                _bars.Add(new Bar(percentage, barStatus));

                if (graphStatus < GraphStatus.Error && barStatus > GraphStatus.Normal && valueCountPerBar > 0)
                    graphStatus = barStatus;

                valueCountPerBar = 0;
            }

            var result = graphValues.Any(val => double.IsNaN(val)) ? GraphStatus.NotAvilable :
                values.Any(val => Math.Abs(val) > errorTolerance) ? GraphStatus.Error :
                    values.Any(val => Math.Abs(val) > warningTolerance) ? GraphStatus.Warning :
                        GraphStatus.Normal;
            return result;
        }

        private DrawingVisual CreateVisual(ScaleHelper scale)
        {
            // calculate the total amount of bars and the margin to the top according to the scale difference
            int barAmount = (int)((scale.Maximum + scale.TickSize) / ScaleCache.Instance.GlobalMinTicks) * 2;
            double barHeight = ActualHeight / barAmount - BarMargin;
            double placeholderBarAmount = ((scale.Maximum + scale.TickSize - scale.OriginalMaximum) / ScaleCache.Instance.GlobalMinTicks) - 1;
            double placeholderHeight = (barHeight + BarMargin) * placeholderBarAmount;

            // start drawing bars with respect to placeholder.
            // offset by a half bar, so that each bar corresponds to a value on an axis.
            double currentY = placeholderHeight - barHeight / 2.0;

            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                _bars.ForEach(bar =>
                {
                    // normalize
                    bar.Percentage = bar.Percentage / _maxPercentage;
                    context.DrawRectangle(GetBrushForStatus(bar.Status), null, new Rect(0, currentY, bar.Percentage * ActualWidth, barHeight));
                    currentY += barHeight + BarMargin;
                });
            }

            return visual;
        }

        protected override Visual DrawGraphVisual()
        {
            return CreateVisual(ScaleCache.Instance.Scales[_parent.GraphSize]);
        }

        protected override Visual DrawToleranceVisual()
        {
            var currentScale = ScaleCache.Instance.Scales[_parent.GraphSize];

            var errorHeight = ActualHeight * _parent.ErrorTolerance * 2 / (2 * currentScale.Maximum + 2 * currentScale.TickSize);
            var warningHeight = errorHeight * _parent.WarningTolerance * 2 / (_parent.ErrorTolerance * 2);

            var errorLinePositiveY = (ActualHeight - errorHeight) / 2d;
            var errorLineNegativeY = errorHeight + errorLinePositiveY;

            var warningLinePositiveY = (ActualHeight - warningHeight) / 2d;
            var warningLineNegativeY = warningHeight + warningLinePositiveY;

            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                // draw baseline
                context.DrawLine(Neutral, new Point(0, ActualHeight / 2d), new Point(ActualWidth, ActualHeight / 2d));

                // draw error and warning tolerance line above baseline
                context.DrawLine(Error, new Point(0, errorLinePositiveY), new Point(ActualWidth, errorLinePositiveY));
                context.DrawLine(Error, new Point(0, errorLineNegativeY), new Point(ActualWidth, errorLineNegativeY));

                // draw error and warning tolerance line below baseline
                context.DrawLine(Warning, new Point(0, warningLinePositiveY), new Point(ActualWidth, warningLinePositiveY));
                context.DrawLine(Warning, new Point(0, warningLineNegativeY), new Point(ActualWidth, warningLineNegativeY));
            }

            return visual;
        }

        protected override Brush GetBrushForStatus(GraphStatus status)
        {
            switch (status)
            {
                case GraphStatus.Error:
                    return Brushes.Red;
                case GraphStatus.Normal:
                    return Green;
                case GraphStatus.Warning:
                    return Brushes.Yellow;
                case GraphStatus.NotAvilable:
                    return Brushes.Gray;
                default:
                    return null;
            }
        }

        public bool IsCompletelyWrong()
        {
            return VisualChildrenCount == 0;
        }

        #endregion

    }
}
