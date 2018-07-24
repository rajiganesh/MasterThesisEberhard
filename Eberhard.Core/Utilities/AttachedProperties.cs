using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Eberhard.Core.Controls;
using System.Windows.Controls;

namespace Eberhard.Core.Utilities
{
    public class AttachedProperties
    {
        public static object GetSubContent(DependencyObject obj)
        {
            return (object)obj.GetValue(SubContentProperty);
        }

        public static void SetSubContent(DependencyObject obj, object value)
        {
            obj.SetValue(SubContentProperty, value);
        }

        // Using a DependencyProperty as the backing store for SubContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubContentProperty =
            DependencyProperty.RegisterAttached("SubContent", typeof(object), typeof(AttachedProperties), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));


        public static Brush GetButtonIcon(DependencyObject obj)
        {
            return (Brush)obj.GetValue(ButtonIconProperty);
        }

        public static void SetButtonIcon(DependencyObject obj, Brush value)
        {
            obj.SetValue(ButtonIconProperty, value);
        }

        // Using a DependencyProperty as the backing store for ButtonIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonIconProperty =
            DependencyProperty.RegisterAttached("ButtonIcon", typeof(Brush), typeof(AttachedProperties), new PropertyMetadata(null));




        public static double GetHorizontalBarWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(HorizontalBarWidthProperty);
        }

        public static void SetHorizontalBarWidth(DependencyObject obj, double value)
        {
            obj.SetValue(HorizontalBarWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for HorizontalBarWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalBarWidthProperty =
            DependencyProperty.RegisterAttached("HorizontalBarWidth", 
                typeof(double),
                typeof(AttachedProperties),
                new PropertyMetadata(0.0));


        public static bool GetShowHorizontalScrollToEndButtons(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowHorizontalScrollToEndButtonsProperty);
        }

        public static void SetShowHorizontalScrollToEndButtons(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowHorizontalScrollToEndButtonsProperty, value);
        }

        // Using a DependencyProperty as the backing store for ShowHorizontalScrollToEndButtons.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowHorizontalScrollToEndButtonsProperty =
            DependencyProperty.RegisterAttached("ShowHorizontalScrollToEndButtons", typeof(bool), typeof(AttachedProperties), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
        

        public static bool GetShowFunctionArea(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowFunctionAreaProperty);
        }

        public static void SetShowFunctionArea(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowFunctionAreaProperty, value);
        }

        // Using a DependencyProperty as the backing store for ShowFunctionArea.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowFunctionAreaProperty =
            DependencyProperty.RegisterAttached("ShowFunctionArea", typeof(bool), typeof(AttachedProperties), new PropertyMetadata(false));


        public static ICommand GetZoomOutCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ZoomOutCommandProperty);
        }

        public static void SetZoomOutCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ZoomOutCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for ZoomOutCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomOutCommandProperty =
            DependencyProperty.RegisterAttached("ZoomOutCommand", typeof(ICommand), typeof(AttachedProperties), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));


        public static ICommand GetZoomInCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ZoomInCommandProperty);
        }

        public static void SetZoomInCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ZoomInCommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for ZoomInCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomInCommandProperty =
            DependencyProperty.RegisterAttached("ZoomInCommand", typeof(ICommand), typeof(AttachedProperties), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public static int GetRowsCount(DependencyObject obj)
        {
            return (int)obj.GetValue(RowsCountProperty);
        }

        public static void SetRowsCount(DependencyObject obj, int value)
        {
            obj.SetValue(RowsCountProperty, value);
        }

        // Using a DependencyProperty as the backing store for RowsCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowsCountProperty =
            DependencyProperty.RegisterAttached("RowsCount",
                typeof(int),
                typeof(AttachedProperties),
                new PropertyMetadata(1, OnRowsChanged));

        private static void OnRowsChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is Grid grid && (int)e.NewValue > 0)
            {
                grid.RowDefinitions.Clear();
                for (int i = 0; i < (int)e.NewValue; i++)
                    grid.RowDefinitions.Add(
                        new RowDefinition()
                        {                            
                            Height = new GridLength(1, GridUnitType.Auto)
                        });
            }
        }

        public static int GetColumnsCount(DependencyObject obj)
        {
            return (int)obj.GetValue(ColumnsCountProperty);
        }

        public static void SetColumnsCount(DependencyObject obj, int value)
        {
            obj.SetValue(ColumnsCountProperty, value);
        }

        // Using a DependencyProperty as the backing store for ColumnsCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsCountProperty =
            DependencyProperty.RegisterAttached("ColumnsCount",
                typeof(int),
                typeof(AttachedProperties),
                new PropertyMetadata(1, OnColumnsChanged));

        private static void OnColumnsChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is Grid grid && (int)e.NewValue > 0)
            {
                grid.ColumnDefinitions.Clear();
                for (int i = 0; i < (int)e.NewValue; i++)
                    grid.ColumnDefinitions.Add(
                        new ColumnDefinition()
                        {
                            Width = new GridLength(1, GridUnitType.Auto)
                        });
            }
        }   

        //public static GraphSize GetGraphSize(DependencyObject obj)
        //{
        //    return (GraphSize)obj.GetValue(GraphSizeProperty);
        //}

        //public static void SetGraphSize(DependencyObject obj, GraphSize value)
        //{
        //    obj.SetValue(GraphSizeProperty, value);
        //}

        //// Using a DependencyProperty as the backing store for GraphSize.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty GraphSizeProperty =
        //    DependencyProperty.RegisterAttached("GraphSize", typeof(GraphSize), typeof(AttachedProperties), new FrameworkPropertyMetadata(GraphSize.Small, FrameworkPropertyMetadataOptions.Inherits));

    }
}
