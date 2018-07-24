using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace Eberhard.Core.Utilities
{
    public class BindingProxy : Freezable
    {
        #region Fields

        /// <summary>
        /// The source property
        /// </summary>
        public static readonly DependencyProperty SourceProperty;

        /// <summary>
        /// The target property
        /// </summary>
        public static readonly DependencyProperty TargetProperty;

        /// <summary>
        /// The data property
        /// </summary>
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(
            "Data",
            typeof(object),
            typeof(BindingProxy),
            new UIPropertyMetadata(null));

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="BindingProxy"/> class.
        /// </summary>
        static BindingProxy()
        {
            var sourceMetadata = new FrameworkPropertyMetadata(
            delegate (DependencyObject p, DependencyPropertyChangedEventArgs args)
            {
                if (null != BindingOperations.GetBinding(p, TargetProperty))
                {
                    (p as BindingProxy).Target = args.NewValue;
                }
            });

            sourceMetadata.BindsTwoWayByDefault = false;
            sourceMetadata.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            SourceProperty = DependencyProperty.Register(
                "Source",
                typeof(object),
                typeof(BindingProxy),
                sourceMetadata);

            var targetMetadata = new FrameworkPropertyMetadata(
                delegate (DependencyObject p, DependencyPropertyChangedEventArgs args)
                {
                    ValueSource source = DependencyPropertyHelper.GetValueSource(p, args.Property);
                    if (source.BaseValueSource != BaseValueSource.Local)
                    {
                        var proxy = p as BindingProxy;
                        object expected = proxy.Source;
                        if (!object.ReferenceEquals(args.NewValue, expected))
                        {
                            Dispatcher.CurrentDispatcher.BeginInvoke(
                                DispatcherPriority.DataBind,
                                new Action(() =>
                                {
                                    proxy.Target = proxy.Source;
                                }));
                        }
                    }
                });

            targetMetadata.BindsTwoWayByDefault = true;
            targetMetadata.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            TargetProperty = DependencyProperty.Register(
                "Target",
                typeof(object),
                typeof(BindingProxy),
                targetMetadata);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public object Data
        {
            get
            {
                return (object)this.GetValue(DataProperty);
            }

            set
            {
                this.SetValue(DataProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public object Source
        {
            get
            {
                return this.GetValue(SourceProperty);
            }

            set
            {
                this.SetValue(SourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public object Target
        {
            get
            {
                return this.GetValue(TargetProperty);
            }

            set
            {
                this.SetValue(TargetProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable" /> derived class.
        /// </summary>
        /// <returns>
        /// The new instance.
        /// </returns>
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        #endregion Methods
    }
}
