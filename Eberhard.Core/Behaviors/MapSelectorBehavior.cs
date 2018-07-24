using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Eberhard.Core.Behaviors
{
    public class MapSelectorBehavior : Behavior<FrameworkElement>
    {
        private IDisposable _eventObservers;

        protected override void OnAttached()
        {
            base.OnAttached();

            _eventObservers = Observable.FromEventPattern<MouseButtonEventArgs>(AssociatedObject, "MouseDown")
                .Where(eve => Keyboard.IsKeyDown(Key.LeftCtrl) ||
                    Keyboard.IsKeyDown(Key.LeftAlt))
                .Subscribe(eve =>
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyUp(Key.LeftAlt) && Keyboard.IsKeyUp(Key.RightAlt))
                    {
                        this.IsSelected = true;
                    }

                    else if (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyUp(Key.LeftCtrl) && Keyboard.IsKeyUp(Key.RightCtrl))
                    {
                        this.IsSelected = false;
                    }
                    eve.EventArgs.Handled = true;
                });
        }

      

        protected override void OnDetaching()
        {
            base.OnDetaching();
            _eventObservers.Dispose();
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected",
                typeof(bool),
                typeof(MapSelectorBehavior),
                new PropertyMetadata(false));

       
    }
}