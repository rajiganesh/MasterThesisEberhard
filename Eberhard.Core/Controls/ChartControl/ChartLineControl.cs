using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Eberhard.Core.Controls.ChartControl
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Eberhard.Core.Controls.ChartControl"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Eberhard.Core.Controls.ChartControl;assembly=Eberhard.Core.Controls.ChartControl"
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
    ///     <MyNamespace:ChartLineControl/>
    ///
    /// </summary>
    public class ChartLineControl : Control
    {
        static ChartLineControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartLineControl), new FrameworkPropertyMetadata(typeof(ChartLineControl)));
        }

        public Point StartingPoint
        {
            get { return (Point)GetValue(StartingPointProperty); }
            set { SetValue(StartingPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartingPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartingPointProperty =
            DependencyProperty.Register("StartingPoint", typeof(Point), typeof(ChartLineControl), new PropertyMetadata(new Point()));

        public double LineThickness
        {
            get { return (double)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register("LineThickness",
                typeof(double),
                typeof(ChartLineControl), new PropertyMetadata(1.0));

        public PointCollection ChartPointsSource
        {
            get { return (PointCollection)GetValue(ChartPointsSourceProperty); }
            set { SetValue(ChartPointsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChartPointsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChartPointsSourceProperty =
            DependencyProperty.Register("ChartPointsSource",
                typeof(PointCollection),
                typeof(ChartLineControl),
                new PropertyMetadata(new PointCollection()));

        public IChartLine Points
        {
            get { return (IChartLine)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Points.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points",
                typeof(IChartLine),
                typeof(ChartLineControl),
                new PropertyMetadata(null, (s, e) =>
                    (s as ChartLineControl).OnPointsChanged((IChartLine)e.NewValue)));

        private void OnPointsChanged(IChartLine newValue)
        {
            if (newValue == null || !newValue.Any())
                return;
            StartingPoint = newValue.FirstOrDefault().DisplayPosition;
            var displayPoints = newValue.Select(point => point.DisplayPosition);
            ChartPointsSource = new PointCollection(displayPoints);
            var geoetryCollection =
                new GeometryCollection(displayPoints.Select(point => new EllipseGeometry(point, 4, 4)));

            geoetryCollection.Freeze();
            PointsCollection = geoetryCollection;
        }

        public GeometryCollection PointsCollection
        {
            get { return (GeometryCollection)GetValue(PointsCollectionProperty); }
            set { SetValue(PointsCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PointsCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointsCollectionProperty =
            DependencyProperty.Register("PointsCollection",
                typeof(GeometryCollection),
                typeof(ChartLineControl),
                new PropertyMetadata(new GeometryCollection()));

        public bool IsTipVisibile
        {
            get { return (bool)GetValue(IsTipVisibileProperty); }
            set { SetValue(IsTipVisibileProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTipVisibile.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTipVisibileProperty =
            DependencyProperty.Register("IsTipVisibile",
                typeof(bool),
                typeof(ChartLineControl),
                new PropertyMetadata(false));

        public IChartPoint SelectedPoint
        {
            get { return (IChartPoint)GetValue(SelectedPointProperty); }
            set { SetValue(SelectedPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedPointProperty =
            DependencyProperty.Register("SelectedPoint",
                typeof(IChartPoint),
                typeof(ChartLineControl), new PropertyMetadata(null));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var host = GetTemplateChild("PART_LineHost") as Canvas;

            var path = GetTemplateChild("PART_PointsPath") as Path;
            if (path == null)
                return;
            // switch on the tip for the point!
            Observable.FromEventPattern<MouseEventArgs>(path, "PreviewMouseMove")
                 .Where(_ => SelectedPoint == null)
                 .Select(eve => eve.EventArgs.GetPosition(host))
                 .DistinctUntilChanged()
                 .Subscribe(position =>
                    SelectedPoint = Points.FirstOrDefault(point => point.DisplayPosition.X + 5 > position.X));

            Observable.FromEventPattern<MouseEventArgs>(path, "MouseLeave")
                .Where(_ => SelectedPoint != null)
              .Subscribe(_ => SelectedPoint = null);
        }

       
    }
}