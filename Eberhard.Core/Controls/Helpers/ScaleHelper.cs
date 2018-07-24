using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Eberhard.Core.Controls.Helpers
{
    internal class ScaleCache
    {
        private const string FORMAT = "+ 0.00#;- 0.00#;        0";

        private double oldMin = 0, oldMax = 0;
        private Dictionary<GraphSize, ScaleHelper> scales;
        private Dictionary<GraphSize, List<string>> labels;
        private static ScaleCache instance;

        public static ScaleCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new ScaleCache();
                return instance;
            }
        }

        public IReadOnlyDictionary<GraphSize, ScaleHelper> Scales { get { return scales; } }

        public IReadOnlyDictionary<GraphSize, List<string>> Labels { get { return labels; } }

        public double GlobalMinTicks { get; private set; }

        public void Initialize(double minimum, double maximum)
        {
            if (minimum == 0 && maximum == 0)
                return;
            if(minimum != oldMin || maximum != oldMax)
            {
                scales = new Dictionary<GraphSize, ScaleHelper>
                {
                    { GraphSize.Small, new ScaleHelper(minimum, maximum, 5) },
                    { GraphSize.Medium, new ScaleHelper(minimum, maximum, 10) }
                };

                labels = new Dictionary<GraphSize, List<string>>()
                {
                    { GraphSize.Small, CreateLablesForScale(scales[GraphSize.Small]) },
                    { GraphSize.Medium, CreateLablesForScale(scales[GraphSize.Medium]) }
                };

                // no special scale for large size
                scales.Add(GraphSize.Large, scales[GraphSize.Medium]);
                labels.Add(GraphSize.Large, labels[GraphSize.Medium]);

                // keeps a consistent amount of bars through all scales.
                GlobalMinTicks = scales[GraphSize.Small].TickSize/ 5d;

                oldMin = minimum;
                oldMin = maximum;
            }
        }

        private List<string> CreateLablesForScale(ScaleHelper scale)
        {
            var tick = (decimal)scale.TickSize;
            var current = (decimal)scale.Maximum + tick;
            var min = (decimal)scale.Minimum - tick;

            var labels = new List<string>();

            while (current >= min)
            {
                labels.Add(current.ToString(FORMAT));
                current -= tick;
            }

            return labels;
        } 
    }

    /// <summary>
    /// A helper class to find 'nice' numbers on a scale.
    /// </summary>
    internal class ScaleHelper
    {
        /// <summary>
        /// Creates a new scale helper.
        /// </summary>
        /// <param name="minimum">The original minimum.</param>
        /// <param name="maximum">The original maximum.</param>
        /// <param name="maximumTicks">The maximum amount of steps between minimum and maximum.</param>
        public ScaleHelper(double minimum, double maximum, double maximumTicks = 10)
        {
            min = minimum;
            max = maximum;
            maxTicks = maximumTicks;

            CalculateScale();
        }

        private void CalculateScale()
        {
            range = Calculate(max - min);
            TickSize = Calculate(range / (maxTicks - 1));
            Minimum = Math.Round(min / TickSize, MidpointRounding.AwayFromZero) * TickSize;
            Maximum = Math.Round(max / TickSize, MidpointRounding.AwayFromZero) * TickSize;
        }

        private double Calculate(double range)
        {
            double exponent, fraction, niceFraction;

            exponent = Math.Floor(Math.Log10(range));
            fraction = range / Math.Pow(10, exponent);

            if (fraction < 1.5)
                niceFraction = 1;
            else if (fraction < 3)
                niceFraction = 2;
            else if (fraction < 7)
                niceFraction = 5;
            else
                niceFraction = 10;

            return niceFraction * Math.Pow(10, exponent);
        }

        private double min, max, range, maxTicks;

        /// <summary>
        /// Gets the rounded maximum.
        /// </summary>
        public double Maximum { get; private set; }

        /// <summary>
        /// Gets the original maxium.
        /// </summary>
        public double OriginalMaximum => max;

        /// <summary>
        /// Gets the rounded minimum.
        /// </summary>
        public double Minimum { get; private set; }

        /// <summary>
        /// Gets the step size of a tick.
        /// </summary>
        public double TickSize { get; private set; }
    }
}