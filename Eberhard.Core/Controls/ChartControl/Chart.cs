using Eberhard.Core.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Eberhard.Core.Controls.ChartControl
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Eberhard.Messe.Views.Segments.Charts"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Eberhard.Messe.Views.Segments.Charts;assembly=Eberhard.Messe.Views.Segments.Charts"
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
    ///     <MyNamespace:ChartControl/>
    ///
    /// </summary>
    public class Chart : Control
    {
        static Chart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Chart), new FrameworkPropertyMetadata(typeof(Chart)));
        }

        public Chart()
        {
        }

        public double ErrorTolerance
        {
            get { return (double)GetValue(ErrorToleranceProperty); }
            set { SetValue(ErrorToleranceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorToleranceX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorToleranceProperty =
            DependencyProperty.Register("ErrorTolerance",
                typeof(double),
                typeof(Chart),
                 new PropertyMetadata(0.0,
                (s, e) => (s as Chart).OnToleranceChanged()));

        public double WarningTolerancePercentage
        {
            get { return (double)GetValue(WarningTolerancePercentageProperty); }
            set { SetValue(WarningTolerancePercentageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WarningTolerancePercentage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WarningTolerancePercentageProperty =
            DependencyProperty.Register("WarningTolerancePercentage",
                typeof(double),
                typeof(Chart),
                new PropertyMetadata(80.0));

        public double GraphTick
        {
            get { return (double)GetValue(GraphTickProperty); }
            set { SetValue(GraphTickProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GraphTick.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GraphTickProperty =
            DependencyProperty.Register("GraphTick",
                typeof(double),
                typeof(Chart),
                new PropertyMetadata(0.025));

        public double GraphMiniTick
        {
            get { return (double)GetValue(GraphMiniTickProperty); }
            set { SetValue(GraphMiniTickProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GraphMiniTick.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GraphMiniTickProperty =
            DependencyProperty.Register("GraphMiniTick",
                typeof(double), typeof(Chart),
                new PropertyMetadata(0.01));

        public IEnumerable DataSource
        {
            get { return (IEnumerable)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(IEnumerable),
                typeof(Chart),
                new PropertyMetadata(null,
                    (s, e) => (s as Chart).OnSourceChanged(e.NewValue as IEnumerable)));

        private void OnSourceChanged(IEnumerable newSource)
        {
            var source = newSource as ChartLineCollection;
            if (source == null)
                return;
            HorizontalAxisSource.Clear();

            source.CollectionChangedObservable
                  .Where(eve => eve.EventArgs.Action == NotifyCollectionChangedAction.Add)
                  .SelectMany(eve => eve.EventArgs.NewItems.Cast<IChartLine>())
                  .Where(line => line != null && line.Any() && TimeLineSource.Any())
                  .Subscribe(line =>
                  {
                      HorizontalAxisSource.Add(new ChartLine(
                              line.PinName,
                              line.Select(point =>  new ChartPoint(
                                  point.Value,
                                  point.TimeStamp,
                                  GetDisplayPoint(point))
                              { PinName = point.PinName, PinType = point.PinType }),
                              line.LineColor));
                  });

            source.CollectionChangedObservable
                .Where(eve => eve.EventArgs.Action == NotifyCollectionChangedAction.Remove &&
                              HorizontalAxisSource.Any())
                .Select(eve => eve.EventArgs.OldStartingIndex)
                .Subscribe(index =>
                {
                    var element = HorizontalAxisSource.ElementAt(index);
                    if (element != null)
                        HorizontalAxisSource.Remove(element);
                });
        }

        public IEnumerable TimeDataSource
        {
            get { return (IEnumerable)GetValue(TimeDataSourceProperty); }
            set { SetValue(TimeDataSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeDataSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeDataSourceProperty =
            DependencyProperty.Register("TimeDataSource",
                typeof(IEnumerable),
                typeof(Chart),
                new PropertyMetadata(null,
                    (s, e) => (s as Chart).OnTimeDataSourceChanged()));

        private void OnTimeDataSourceChanged()
        {
            var source = TimeDataSource as NotifingCollection<DateTime>;
            TimeLineSource.Clear();

            if (source != null)
            {
                source.CollectionChangedObservable.
                    Where(eve => eve.EventArgs.Action == NotifyCollectionChangedAction.Add)
                    .Select(eve => eve.EventArgs.NewItems.Cast<DateTime>())
                    .Subscribe(dateTimeCollection =>
                    {
                        dateTimeCollection.ToObservable()
                         .Select(date => date.Subtract(TimeSpan.FromSeconds(date.Second)))
                         .Where(date => !TimeLineSource.Any() || TimeLineSource.LastOrDefault().Date.Equals(date) == false)
                         .Subscribe(date =>
                         {
                             TimeLineSource.Add(new ChartTimeBlock() { Date = date });
                         });
                    });

                if (source.Any())
                {
                    var startingTime = source.Min();
                    var endTime = source.Max();
                    int timeDiffernce = (int)endTime.Subtract(startingTime).TotalMinutes + 1;

                    // Set the timeline
                    TimeLineSource = new TimeAxisSource(
                        Enumerable.Range(0, timeDiffernce)
                        .Select(index => new ChartTimeBlock()
                        {
                            Date = new DateTime(startingTime.Year,
                                    startingTime.Month,
                                    startingTime.Day,
                                    startingTime.Hour,
                                    startingTime.Minute, 0)
                                        .AddMinutes(index),
                            CanShowDate = index == 0
                        }));
                }
            }
        }

        public IChartLineCollection HorizontalAxisSource
        {
            get { return (IChartLineCollection)GetValue(HorizontalAxisSourceProperty); }
            set { SetValue(HorizontalAxisSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalAxisSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalAxisSourceProperty =
            DependencyProperty.Register("HorizontalAxisSource",
                typeof(IChartLineCollection),
                typeof(Chart),
                new PropertyMetadata(new ChartLineCollection()));

        public IChartVerticalAxis VerticalAxis
        {
            get { return (IChartVerticalAxis)GetValue(VerticalAxisProperty); }
            set { SetValue(VerticalAxisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalAxis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalAxisProperty =
            DependencyProperty.Register("VerticalAxis",
                typeof(IChartVerticalAxis),
                typeof(Chart),
                new PropertyMetadata(null));

        public ITimeAxisSource TimeLineSource
        {
            get { return (ITimeAxisSource)GetValue(TimeLineSourceProperty); }
            set { SetValue(TimeLineSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeLineSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeLineSourceProperty =
            DependencyProperty.Register("TimeLineSource",
                typeof(ITimeAxisSource),
                typeof(Chart),
                new PropertyMetadata(new TimeAxisSource()));

        public object FilterContent
        {
            get { return (object)GetValue(FilterContentProperty); }
            set { SetValue(FilterContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilterContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterContentProperty =
            DependencyProperty.Register("FilterContent",
                typeof(object),
                typeof(Chart),
                new PropertyMetadata(null));

        public DataTemplate FitlerContentTemplate
        {
            get { return (DataTemplate)GetValue(FitlerContentTemplateProperty); }
            set { SetValue(FitlerContentTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FitlerContentTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FitlerContentTemplateProperty =
            DependencyProperty.Register("FitlerContentTemplate",
                typeof(DataTemplate),
                typeof(Chart),
                new PropertyMetadata(new DataTemplate()));

        public string DrawerToggleHeader
        {
            get { return (string)GetValue(DrawerToggleHeaderProperty); }
            set { SetValue(DrawerToggleHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DrawerToggleHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrawerToggleHeaderProperty =
            DependencyProperty.Register("DrawerToggleHeader",
                typeof(string),
                typeof(Chart),
                new PropertyMetadata(string.Empty));

        public IChartPoint SelectedPoint
        {
            get { return (IChartPoint)GetValue(SelectedPointProperty); }
            set { SetValue(SelectedPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedPointProperty =
            DependencyProperty.Register("SelectedPoint",
                typeof(IChartPoint),
                typeof(Chart),
                new PropertyMetadata(null));

        public bool IsLiveCharting
        {
            get { return (bool)GetValue(IsLiveChartingProperty); }
            set { SetValue(IsLiveChartingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLiveCharting.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLiveChartingProperty =
            DependencyProperty.Register("IsLiveCharting",
                typeof(bool),
                typeof(Chart),
                new PropertyMetadata(false,
                    (s, e) => (s as Chart).OnLineChartingChanged()));

        private void OnLineChartingChanged()
        {
        }

        public string DrawerToggleSubHeader
        {
            get { return (string)GetValue(DrawerToggleSubHeaderProperty); }
            set { SetValue(DrawerToggleSubHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DrawerToggleSubHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrawerToggleSubHeaderProperty =
            DependencyProperty.Register("DrawerToggleSubHeader",
                typeof(string),
                typeof(Chart),
                new PropertyMetadata(string.Empty));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private Point GetDisplayPoint(IChartPoint chartPoint)
        {
            if (VerticalAxis == null || TimeLineSource == null)
                return new Point();

            var ration = (Math.Abs(chartPoint.Value) * VerticalAxis.PointToPixelRatio) / VerticalAxis.MiniTick;
            var pointY = chartPoint.Value < 0 ?
                       VerticalAxis.ZeroPosition + ration :
                        VerticalAxis.ZeroPosition - ration;
            var pointX = TimeLineSource.GetPositionOfMinute(chartPoint.TimeStamp);

            // if the point is out of tollerance give it a new Y coordinate
            if (Math.Abs(chartPoint.Value) > VerticalAxis.ErrorTolerance)
                pointY = chartPoint.Value > 0 ?
                    VerticalAxis.LinesSource.FirstOrDefault().Top - 30 : // bellow the button red line
                    VerticalAxis.LinesSource.FirstOrDefault().Top + 30; // above the top red line
            return new Point(pointX, pointY);
        }

        private void OnToleranceChanged()
        {
            // if the ticks are can be divided equallt then the ticks will be devided
            // over the ticks on the positive and negative (*2). other wise add an extra tick
            // on both sides the positive and negative
            var numberOfTicks = ErrorTolerance % GraphTick == 0 ?
             (int)(ErrorTolerance / GraphTick) :
             (int)(ErrorTolerance / GraphTick) + 1;

            double topLineTick = Math.Round(numberOfTicks * GraphTick, 4);

            var avilableHeight = this.ActualHeight - 200; // - 100 px for the margins up and down
            var pointToPixelRatio = Math.Round((avilableHeight * GraphMiniTick)
                / (topLineTick * 2));

            var tickRation = pointToPixelRatio *
                (GraphTick / GraphMiniTick);

            var ticksCollection = Enumerable.Range(0, (numberOfTicks * 2) + 1)
               .Select(index => topLineTick - GraphTick * index)
               .Select((label, index) => new AxisTick
               {
                   Label = Math.Round(label, 3, MidpointRounding.AwayFromZero),
                   Top = (index * tickRation) + 50,
               });

            var zeroLine = ticksCollection.FirstOrDefault(tick => tick.Label == 0).Top;
            var warning = (ErrorTolerance * WarningTolerancePercentage) / 100;

            VerticalAxis = new ChartVerticalAxis
            {
                ZeroPosition = zeroLine,
                TickRatio = tickRation,
                MiniTick = GraphMiniTick,
                PointToPixelRatio = pointToPixelRatio,
                ErrorTolerance = ErrorTolerance,
                WarningTolerance = warning,
                LabelSource = ticksCollection,
                LinesSource = new List<VertivalLine>
                {
                    new VertivalLine{
                        Type = LineType.Error,
                        Top = GetTopForPoint(ErrorTolerance, zeroLine, pointToPixelRatio) },
                    new VertivalLine{
                        Type = LineType.Warning,
                        Top = GetTopForPoint(warning, zeroLine, pointToPixelRatio) },
                    new VertivalLine{
                        Type = LineType.Center,
                        Top = zeroLine },
                    new VertivalLine{
                        Type = LineType.Warning,
                        Top = GetTopForPoint(-warning, zeroLine, pointToPixelRatio) },
                    new VertivalLine{
                        Type = LineType.Error,
                        Top = GetTopForPoint(-ErrorTolerance, zeroLine, pointToPixelRatio) }
                }
            };
        }

        public double GetTopForPoint(double point, double zeroPostion, double pixelRatio) =>
            point >= 0 ?
            zeroPostion + ((point * pixelRatio) / GraphMiniTick) :
            zeroPostion - ((point * -pixelRatio) / GraphMiniTick);
    }
}