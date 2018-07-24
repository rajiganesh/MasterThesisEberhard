using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Eberhard.Core.Behaviors
{
    public class AdornerBehavior : Behavior<FrameworkElement>
    {
        private TemplateAdorner _templateAdorner = null;
        private ContentControl _adornerControl;

        public AdornerBehavior()
        {
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            // Create adorner
            var adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
            if (adornerLayer == null)
                return;

            _adornerControl = new ContentControl();

            // Add to adorner
            _templateAdorner = new TemplateAdorner(AssociatedObject, _adornerControl);
            adornerLayer.Add(_templateAdorner);
            _adornerControl.Content = AdornerDataTemplate.LoadContent();

            //BindingOperations.SetBinding(_adornerControl,
            //    ContentControl.DataContextProperty,
            //    new Binding("DataContext")
            //    {
            //        Source = AssociatedObject,
            //    });

        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (null != _templateAdorner)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject as UIElement);
                adornerLayer.Remove(_templateAdorner);
                BindingOperations.ClearBinding(_templateAdorner, ContentControl.MarginProperty);
                _templateAdorner = null;
                _adornerControl = null;
            }
        }

        public DataTemplate AdornerDataTemplate
        {
            get { return (DataTemplate)GetValue(AdornerDataTemplateProperty); }
            set { SetValue(AdornerDataTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AdornerDataTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AdornerDataTemplateProperty =
            DependencyProperty.Register("AdornerDataTemplate",
                typeof(DataTemplate),
                typeof(AdornerBehavior),
                new PropertyMetadata(new DataTemplate()));

        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Margin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register("Margin",
                typeof(Thickness),
                typeof(AdornerBehavior),
                new PropertyMetadata(new Thickness(),
                    (s, e) =>
                    {
                        var sender = (s as AdornerBehavior);
                        if (sender._templateAdorner == null)
                            return;

                        sender._templateAdorner.Margin = (Thickness)e.NewValue;
                    }));

        public object DataContext
        {
            get { return (object)GetValue(DataContextProperty); }
            set { SetValue(DataContextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataContext.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register("DataContext",
                typeof(object),
                typeof(AdornerBehavior),
                new PropertyMetadata(null ,
                (s, e) =>
                    {
                        var sender = (s as AdornerBehavior);
                        if (sender._templateAdorner == null)
                            return;

                        sender._adornerControl.DataContext = e.NewValue;
                    }));

        public Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Visibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register("Visibility",
                typeof(Visibility),
                typeof(AdornerBehavior),
                new PropertyMetadata(Visibility.Visible,
                     (s, e) =>
                     {
                         var sender = (s as AdornerBehavior);
                         if (sender._templateAdorner == null)
                             return;

                         sender._templateAdorner.Visibility = (Visibility)e.NewValue;
                     }));
    }

    internal class TemplateAdorner : Adorner
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="adornedElement"></param>
        public TemplateAdorner(FrameworkElement adornedElement, FrameworkElement frameworkElementAdorner)
            : base(adornedElement)
        {
            // Assure we get mouse hits
            _frameworkElementAdorner = frameworkElementAdorner;
            AddVisualChild(_frameworkElementAdorner);
            AddLogicalChild(_frameworkElementAdorner);
        }

        /// <summary>
        ///
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            return _frameworkElementAdorner;
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _frameworkElementAdorner.Width = constraint.Width;
            _frameworkElementAdorner.Height = constraint.Height;

            return constraint;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _frameworkElementAdorner.Arrange(new Rect(new Point(0, 0), finalSize));
            return finalSize;
        }

        private FrameworkElement _frameworkElementAdorner;
    }
}