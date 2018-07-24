using System;
using System.Collections;
using System.Collections.Specialized;
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
    ///     <MyNamespace:GraphBase/>
    ///
    /// </summary>
    ///
    [TemplateVisualState(GroupName = "SizeStates", Name = "Small")]
    [TemplateVisualState(GroupName = "SizeStates", Name = "Medium")]
    [TemplateVisualState(GroupName = "SizeStates", Name = "Large")]
    public abstract class GraphBase : Control
    {
        #region Constructor

        static GraphBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphBase), new FrameworkPropertyMetadata(typeof(GraphBase)));
        }

        public GraphBase()
        {
            var loaded = Observable.FromEventPattern<RoutedEventArgs>(this, "Loaded")
                .Where(_ => ScrollViewer != null)
                .ObserveOnDispatcher()
                .Do(_ => _scrollViewerBonds = new Rect(0.0, 0.0, ScrollViewer.ActualWidth, ScrollViewer.ActualHeight))
                .Subscribe(_ =>
                {
                    IsInView = IsElementInView();
                    Observable.FromEventPattern<ScrollChangedEventArgs>(ScrollViewer, "ScrollChanged")
                    .Subscribe(eve => IsInView = IsElementInView());

                    bool IsElementInView()
                    {
                        //Rect bounds = TransformToAncestor(ScrollViewer).TransformBounds(
                        //    new Rect(0.0, 0.0, ActualWidth, ActualHeight));
                        Rect bounds = new Rect(0.0, 0.0, ActualWidth, ActualHeight);
                        return _scrollViewerBonds.Contains(bounds) && Visibility == Visibility.Visible;                        
                    }
                });
        }

        #endregion Constructor

        #region Fields

        private Rect _scrollViewerBonds;

        #endregion Fields

        #region Properties

        public bool IsInView
        {
            get { return (bool)GetValue(IsInViewProperty); }
            set { SetValue(IsInViewProperty, value); }
        }

        public ScrollViewer ScrollViewer
        {
            get { return (ScrollViewer)GetValue(ScrollViewerProperty); }
            set { SetValue(ScrollViewerProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush for <see cref="GraphStatus.Normal"/>.
        /// </summary>
        public Brush NormalBrush
        {
            get { return (Brush)GetValue(NormalBrushProperty); }
            set { SetValue(NormalBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush for <see cref="GraphStatus.Warning"/>.
        /// </summary>
        public Brush WarningBrush
        {
            get { return (Brush)GetValue(WarningBrushProperty); }
            set { SetValue(WarningBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the brush for <see cref="GraphStatus.Error"/>.
        /// </summary>
        public Brush ErrorBrush
        {
            get { return (Brush)GetValue(ErrorBrushProperty); }
            set { SetValue(ErrorBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets a predefined size of the graph
        /// </summary>
        public GraphSize GraphSize
        {
            get { return (GraphSize)GetValue(GraphSizeProperty); }
            set { SetValue(GraphSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the status, related to tolerance.
        /// </summary>
        public GraphStatus Status
        {
            get { return (GraphStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        /// <summary>
        /// Gets or sets the actual values to display.
        /// </summary>
        public IEnumerable DataSource
        {
            get { return (IEnumerable)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        #endregion Properties

        #region Dependency Properties

        public static readonly DependencyProperty ScrollViewerProperty =
            DependencyProperty.Register("ScrollViewer",
                typeof(ScrollViewer),
                typeof(GraphBase),
                new PropertyMetadata(null));

        public static readonly DependencyProperty NormalBrushProperty =
            DependencyProperty.Register("NormalBrush",
                typeof(Brush),
                typeof(GraphBase),
                new PropertyMetadata(null));

        public static readonly DependencyProperty WarningBrushProperty =
            DependencyProperty.Register("WarningBrush",
                typeof(Brush),
                typeof(GraphBase),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ErrorBrushProperty =
            DependencyProperty.Register("ErrorBrush",
                typeof(Brush),
                typeof(GraphBase),
                new PropertyMetadata(null));

        public static readonly DependencyProperty IsInViewProperty =
            DependencyProperty.Register("IsInView",
                typeof(bool),
                typeof(GraphBase),
                new PropertyMetadata(false));

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource",
                typeof(IEnumerable),
                typeof(GraphBase),
                new PropertyMetadata(null, (sender, e) =>
                  (sender as GraphBase).OnDataSourceChanged(e.OldValue as IEnumerable, e.NewValue as IEnumerable)));

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header",
                typeof(string),
                typeof(GraphBase),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty GraphSizeProperty =
            DependencyProperty.Register("GraphSize",
                typeof(GraphSize),
                typeof(GraphBase),
                new PropertyMetadata(GraphSize.Small, (sender, e) => (sender as GraphBase).OnGraphSizeChanged((GraphSize)e.OldValue, (GraphSize)e.NewValue)));

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status",
                typeof(GraphStatus),
                typeof(GraphBase),
                new PropertyMetadata(GraphStatus.Normal));

        #endregion Dependency Properties

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var child = VisualTreeHelper.GetChild(this, 0) as FrameworkElement;
            var groups = VisualStateManager.GetVisualStateGroups(child);
            var group = groups.Cast<VisualStateGroup>().FirstOrDefault();
            
            if (group != null)

                // We use an observable with a throttle to prevent unnecessary calculations
                // (the "CurrentStateChanged" event fires multiple times).

                Observable
                .FromEventPattern<VisualStateChangedEventArgs>(group, "CurrentStateChanged")
                .Throttle(TimeSpan.FromMilliseconds(150))
                .ObserveOnDispatcher()
                .SubscribeOnDispatcher(System.Windows.Threading.DispatcherPriority.ContextIdle)
                .Subscribe(_ => OnGraphSizeTransitionFinished());

            


            Loaded += (s, e) =>
            {
                SetupDraw();
                Draw();
            };
        }

        

        abstract protected void OnWarningToleranceChanged();

        abstract protected void OnErrorToleranceChanged();

        abstract protected void Draw();

        virtual protected void SetupDraw()
        {
        }

        virtual protected void OnDataSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (oldValue is INotifyCollectionChanged oldCollectionChanged)
                oldCollectionChanged.CollectionChanged -= OnNotifyCollectionChanged;

            if (newValue is INotifyCollectionChanged newCollectionChanged)
                newCollectionChanged.CollectionChanged += OnNotifyCollectionChanged;
        }

        virtual protected void OnNotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            Draw();
        }

        virtual protected void OnGraphSizeChanged(GraphSize oldSize, GraphSize newSize)
        {
            OnGraphSizeTransitionStarted();
            VisualStateManager.GoToState(this, newSize.ToString(), true);
            
        }

        /// <summary>
        /// The transition a new graph size started.
        /// </summary>
        virtual protected void OnGraphSizeTransitionStarted() { }

        /// <summary>
        /// The transition to a new graph size finished.
        /// </summary>
        virtual protected void OnGraphSizeTransitionFinished() { }

        

        #endregion Methods
    }

    /// <summary>
    /// Predefined sizes for a graph.
    /// </summary>
    public enum GraphSize
    {
        Small = 1,
        Medium = 2,
        Large = 3,
    }

    /// <summary>
    /// Status of a graph based on its tolerances.
    /// </summary>
    public enum GraphStatus
    {
        Normal = 0,
        Warning = 1,
        Error = 2, 
        NotAvilable = 3,
    }
}