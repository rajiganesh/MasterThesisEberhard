using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eberhard.Core.Utilities
{
    public class NotifingObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void FirePropertyChanged(
            [CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public IObservable<string> PropertyChangedObserver =>
                Observable
                    .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                        h => PropertyChanged += h,
                        h => PropertyChanged -= h)
                    .Select(x => x.EventArgs.PropertyName);

       
    }

    public static class ObservableExtensions
    {
        public static IObservable<T> PropertyChangedObserverBySource<T>(this T source, string propertyName) where T : INotifyPropertyChanged =>
            Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                               handler => handler.Invoke,
                               h => source.PropertyChanged += h,
                               h => source.PropertyChanged -= h)
                           .Where(eve => eve.EventArgs.PropertyName.Equals(propertyName))
                           .Select(_ => source);

    }
}
