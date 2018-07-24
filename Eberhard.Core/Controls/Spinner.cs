using System;
using System.Windows;
using System.Windows.Controls;

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
    ///     <MyNamespace:Spinner/>
    ///
    /// </summary>
    public class Spinner : ContentControl
    {
        static Spinner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Spinner), new FrameworkPropertyMetadata(typeof(Spinner)));
        }

        public Spinner()
        {
            // by default not visible until active!
            Visibility = Visibility.Collapsed;
        }

      
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive",
                typeof(bool),
                typeof(Spinner),
                new PropertyMetadata(false, (s,e)=> {
                    if((bool)e.NewValue)
                        Application.Current.Dispatcher.Invoke(new Action(() => (s as Spinner).Visibility = Visibility.Visible));
                    else
                        Application.Current.Dispatcher.Invoke(new Action(() => (s as Spinner).Visibility = Visibility.Collapsed));

                }));
    }
}