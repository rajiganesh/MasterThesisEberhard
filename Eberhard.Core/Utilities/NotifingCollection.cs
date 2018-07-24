using Eberhard.Core.BaseTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace Eberhard.Core.Utilities
{
    public class NotifingCollection<T> : ObservableCollection<T> 
    {
        public ObservableCollection<T> Parent { get; protected set; }

        public NotifingCollection(IEnumerable<T> collection) 
            : base(collection)
        {
            _collectionChangedObserver = 
                Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(this, "CollectionChanged");
        }

        public NotifingCollection()
            :this(Enumerable.Empty<T>())
        {

        }

        private IObservable<EventPattern<NotifyCollectionChangedEventArgs>> _collectionChangedObserver;
        public IObservable<EventPattern<NotifyCollectionChangedEventArgs>> CollectionChangedObservable =>
            _collectionChangedObserver;


        public void FirePropertyChanged([CallerMemberName] string propertyName = null) =>
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        public IObservable<string> PropertyChangedObserver
        {
            get
            {
                return Observable
                    .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                        h => PropertyChanged += h,
                        h => PropertyChanged -= h)
                    .Select(x => x.EventArgs.PropertyName);
            }
        }


        private T _selectedItem;

        public T SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value;
                FirePropertyChanged();
            }
        }
    }
}