using Eberhard.Core.Controls.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Eberhard.Core.Controls.Charting
{
    internal class Axis : VisualHost
    {
        #region Fields
        // TODO: respect global styles?
        private static readonly Typeface arial = new Typeface("Arial");
        private readonly Func<string, FormattedText> builder = s => new FormattedText(s, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, arial, 12, Brushes.White);

        private readonly IDictionary<GraphSize, Visual> _cache = new Dictionary<GraphSize, Visual>();

        #endregion

        #region Properties

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Axis), new PropertyMetadata(Orientation.Vertical));

        #endregion

        #region Methods

        public void Draw(GraphSize size)
        {
            _visuals.Clear();

            if (_cache.ContainsKey(size))
                _visuals.Add(_cache[size]);
            else
            {
                var visual = CreateVisualFromScale(ScaleCache.Instance.Labels[size]);
                if(visual != null)
                {
                    _cache.Add(size, visual);
                    _visuals.Add(visual);
                }
            }
        }

        public void Redraw(GraphSize size)
        {
            _cache.Clear();
            Draw(size);
        }

        public Visual CreateVisualFromScale(ICollection<string> labels)
        {
            var visual = new DrawingVisual();
            var context = visual.RenderOpen();

            var stepY = ActualHeight / labels.Count;
            var currentY = stepY / 2d;

            foreach (var s in labels)
            {
                var format = builder(s);

                context.DrawText(format, new Point(0, currentY - (format.Height / 2d)));
                currentY += stepY;
            }

            context.Close();
            return visual;
        }

        #endregion
    }
}
