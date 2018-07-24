using Eberhard.Core.Controls.Charting;
using Eberhard.Core.Controls.Helpers;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Shapes;

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
    ///     <MyNamespace:PointCloud/>
    ///
    /// </summary>
    [TemplatePart(Name = PART_Points, Type = typeof(PointHost))]
    [StyleTypedProperty(Property = "PointStyle", StyleTargetType = typeof(Ellipse))]
    public class PointCloud : GraphBase
    {
        #region Constructor

        static PointCloud()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PointCloud), new FrameworkPropertyMetadata(typeof(PointCloud)));
        }

        #endregion Constructor

        #region Fields

        private const string PART_Points = "PART_Points";

        private PointHost _points;

        internal Rect _warningZoneValidation = Rect.Empty;
        internal Rect _errorZoneValidation = Rect.Empty;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the tolerance for errors on the x-axis.
        /// </summary>
        public double ErrorToleranceX
        {
            get { return (double)GetValue(ErrorToleranceXProperty); }
            set { SetValue(ErrorToleranceXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the tolerance for errors on the y-axis.
        /// </summary>
        public double ErrorToleranceY
        {
            get { return (double)GetValue(ErrorToleranceYProperty); }
            set { SetValue(ErrorToleranceYProperty, value); }
        }

        /// <summary>
        /// Gets or sets the tolerance for warnings on the x-axis.
        /// </summary>
        public double WarningToleranceX
        {
            get { return (double)GetValue(WarningToleranceXProperty); }
            set { SetValue(WarningToleranceXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the tolerance for warnings on the y-axis.
        /// </summary>
        public double WarningToleranceY
        {
            get { return (double)GetValue(WarningToleranceYProperty); }
            set { SetValue(WarningToleranceYProperty, value); }
        }

        #endregion Properties

        #region Dependency Properties

        public static readonly DependencyProperty WarningToleranceXProperty =
            DependencyProperty.Register("WarningToleranceX", typeof(double), typeof(PointCloud), new PropertyMetadata(0d, (s, e) => (s as PointCloud).OnWarningToleranceChanged()));

        public static readonly DependencyProperty WarningToleranceYProperty =
            DependencyProperty.Register("WarningToleranceY", typeof(double), typeof(PointCloud), new PropertyMetadata(0d, (s, e) => (s as PointCloud).OnWarningToleranceChanged()));

        public static readonly DependencyProperty ErrorToleranceXProperty =
            DependencyProperty.Register("ErrorToleranceX", typeof(double), typeof(PointCloud), new PropertyMetadata(0d, (s, e) => (s as PointCloud).OnErrorToleranceChanged()));

        public static readonly DependencyProperty ErrorToleranceYProperty =
            DependencyProperty.Register("ErrorToleranceY", typeof(double), typeof(PointCloud), new PropertyMetadata(0d, (s, e) => (s as PointCloud).OnErrorToleranceChanged()));

        #endregion Dependency Properties

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _points = GetTemplateChild(PART_Points) as PointHost;
            _points.SetParent(this);

            Observable.FromEventPattern<DependencyPropertyChangedEventArgs>(this, "IsVisibleChanged")
                .Where(eve => eve.EventArgs.NewValue.Equals(true))
                .ObserveOnDispatcher()
                .SubscribeOnDispatcher(System.Windows.Threading.DispatcherPriority.ContextIdle)
                .Subscribe(_ =>
                {
                });
        }

        protected override void OnWarningToleranceChanged()
        {
            if (WarningToleranceY == 0 || WarningToleranceX == 0)
                return;
            _warningZoneValidation = new Rect(-WarningToleranceX, -WarningToleranceY, 2 * WarningToleranceX, 2 * WarningToleranceY);
            Draw();
        }

        protected override void OnErrorToleranceChanged()
        {
            if (ErrorToleranceX == 0 || ErrorToleranceY == 0)
                return;
            _errorZoneValidation = new Rect(-ErrorToleranceX, -ErrorToleranceY, 2 * ErrorToleranceX, 2 * ErrorToleranceY);
            Draw();
        }

        protected override void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnDataSourceChanged(oldValue, newValue);
            _points?.RedrawSource(GraphSize);
        }

        protected override void OnNotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            _points?.RedrawSource(GraphSize);
        }



        protected override void OnGraphSizeTransitionStarted()
        {
            _points?.BeginAnimation(OpacityProperty, AnimationHelper.ToTransparent);
        }

        protected override void OnGraphSizeTransitionFinished()
        {
            _points?.Redraw(GraphSize);
            _points?.BeginAnimation(OpacityProperty, AnimationHelper.ToOpaque);
        }

        protected override void SetupDraw()
        {
            _errorZoneValidation = new Rect(-ErrorToleranceX, -ErrorToleranceY, 2 * ErrorToleranceX, 2 * ErrorToleranceY);
            _warningZoneValidation = new Rect(-WarningToleranceX, -WarningToleranceY, 2 * WarningToleranceX, 2 * WarningToleranceY);
        }

        protected override void Draw()
        {
            _points?.Redraw(GraphSize);
        }

        #endregion Methods
    }
}