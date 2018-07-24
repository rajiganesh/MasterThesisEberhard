using Eberhard.Core.Controls.Charting;
using Eberhard.Core.Controls.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Eberhard.Core.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Eberhard.Core.Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Eberhard.Core.Controls;assembly=Eberhard.Core.Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:Histogram/>
    ///
    /// </summary>
    [TemplatePart(Name = PART_Bars, Type = typeof(BarHost))]
    [TemplatePart(Name = PART_Axis, Type = typeof(Axis))]
    public class Histogram : GraphBase
    {
        #region Constructor

        static Histogram()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Histogram), new FrameworkPropertyMetadata(typeof(Histogram)));
        }

        #endregion Constructor

        #region Fields

        private const string PART_Bars = "PART_Bars";
        private const string PART_Axis = "PART_Axis";

        private BarHost _bars;
        private Axis _labels;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the absolute tolerance when to show the error visual.
        /// </summary>
        public double ErrorTolerance
        {
            get { return (double)GetValue(ErrorToleranceProperty); }
            set { SetValue(ErrorToleranceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the absolute tolerance when to show the warning visual.
        /// </summary>
        public double WarningTolerance
        {
            get { return (double)GetValue(WarningToleranceProperty); }
            set { SetValue(WarningToleranceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the orientation. The default representation is a vertical axis.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        #endregion Properties

        #region Dependency Properties

        public static readonly DependencyProperty ErrorToleranceProperty =
            DependencyProperty.Register("ErrorTolerance",
                typeof(double),
                typeof(Histogram),
                new PropertyMetadata(0d,
                    (s, e) => (s as Histogram).OnErrorToleranceChanged()));

        public static readonly DependencyProperty WarningToleranceProperty =
            DependencyProperty.Register("WarningTolerance",
                typeof(double),
                typeof(Histogram),
                new PropertyMetadata(0d, 
                    (s, e) => (s as Histogram).OnWarningToleranceChanged()));

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation",
                typeof(Orientation),
                typeof(Histogram),
                new PropertyMetadata(Orientation.Vertical));

        #endregion Dependency Properties

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _labels = GetTemplateChild(PART_Axis) as Axis;
            _bars = GetTemplateChild(PART_Bars) as BarHost;

            _bars.SetParent(this);

            ScaleCache.Instance.Initialize(-ErrorTolerance, ErrorTolerance);
        }

        #endregion Methods

        override protected async void OnDataSourceChanged(IEnumerable old, IEnumerable newValue)
        {
            base.OnDataSourceChanged(old, newValue);
            await UpdateBarsAsync();
        }

        private async System.Threading.Tasks.Task UpdateBarsAsync()
        {
            if (_bars == null || DataSource == null)
                return;

            Status = await _bars.UpdateAsync(DataSource.Cast<double>().ToArray(), ErrorTolerance, WarningTolerance);
            _bars.RedrawSource(GraphSize);
        }

        protected override void OnWarningToleranceChanged()
        {
            _labels?.Redraw(GraphSize);
            _bars?.Redraw(GraphSize);
        }

        protected override void OnErrorToleranceChanged()
        {
            ScaleCache.Instance.Initialize(-ErrorTolerance, ErrorTolerance);
            _labels?.Redraw(GraphSize);
            _bars?.Redraw(GraphSize);
        }

        protected override async void OnNotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            await UpdateBarsAsync();
        }

        protected override void OnGraphSizeTransitionStarted()
        {
            _labels?.BeginAnimation(OpacityProperty, AnimationHelper.ToTransparent);
            _bars?.BeginAnimation(OpacityProperty, AnimationHelper.ToTransparent);
        }

        protected override void OnGraphSizeTransitionFinished()
        {
            _bars.Redraw(GraphSize);
            _labels.Redraw(GraphSize);

            _labels.BeginAnimation(OpacityProperty, AnimationHelper.ToOpaque);
            _bars.BeginAnimation(OpacityProperty, AnimationHelper.ToOpaque);
        }

        protected async override void Draw()
        {
            if (DataSource == null || DataSource.Count() == 0)
                return;
            Status = await _bars.UpdateAsync(DataSource.Cast<double>().ToArray(), ErrorTolerance, WarningTolerance);
            _bars.Redraw(GraphSize);
            _labels.Redraw(GraphSize);
        }

        protected override void SetupDraw()
        {
        }
    }
}