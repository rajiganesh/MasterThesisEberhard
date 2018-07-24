namespace Eberhard.Core.Utilities.Localization
{
    using Eberhard.Core.Utilities;
    using System;
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// data context for bound localized properties
    /// </summary>

    internal class LocalizedBinding : NotifingObject
    {
        #region Fields

        /// <summary>
        /// the bound FrameworkElement
        /// </summary>
        private FrameworkElement element;

        /// <summary>
        /// notifying context, if any
        /// </summary>
        private INotifyPropertyChanged context;

        /// <summary>
        /// the bound string
        /// </summary>
        private LocalizedString localized;

        /// <summary>
        /// the property path
        /// </summary>
        private string propertyPath;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedBinding" /> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="propertyPath">The property path.</param>
        public LocalizedBinding(FrameworkElement element, string propertyPath)
        {
            this.element = element;
            this.propertyPath = propertyPath;

            element.DataContextChanged += this.OnContextChanged;

            element.Unloaded += (s, e) => this.Detach();

            this.OnContextChanged(element.DataContext);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the localized string.
        /// </summary>
        /// <value>
        /// The localized string.
        /// </value>
        public LocalizedString Localized
        {
            get
            {
                return this.localized;
            }

            private set
            {
                if (this.localized != value)
                {
                    this.localized = value;
                    this.FirePropertyChanged("Localized");
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="propertyPath">The property path.</param>
        /// <returns>
        /// key or null
        /// </returns>
        internal static string ResolveKey(object context, string propertyPath)
        {
            object key = context;

            if (key != null && !IsPathless(propertyPath))
            {
                var steps = propertyPath.Split('.');

                foreach (var step in steps)
                {
                    var property = key
                        .GetType()
                        .GetProperty(step);

                    if (property != null)
                    {
                        key = property.GetValue(key, null);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return key != null ? key.ToString() : null;
        }

        /// <summary>
        /// Determines whether the specified property path is pathless.
        /// </summary>
        /// <param name="propertyPath">The property path.</param>
        /// <returns>
        ///   <c>true</c> if the specified property path is pathless; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsPathless(string propertyPath)
        {
            return string.IsNullOrWhiteSpace(propertyPath) || propertyPath.Trim() == ".";
        }

        /// <summary>
        /// Detaches this instance.
        /// </summary>
        private void Detach()
        {
            this.element.DataContextChanged -= this.OnContextChanged;

            if (this.context != null)
            {
                this.context.PropertyChanged -= this.OnPropertyChanged;
            }
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <param name="contextObject">The context.</param>
        /// <returns>key stored in context property</returns>
        private string GetKey(object contextObject)
        {
            return LocalizedBinding.ResolveKey(contextObject, this.propertyPath);
        }

        /// <summary>
        /// Called when context changed.
        /// </summary>
        /// <param name="newContext">The new context.</param>
        private void OnContextChanged(object newContext)
        {
            if (this.context != null)
            {
                this.context.PropertyChanged -= this.OnPropertyChanged;
            }

            this.context = newContext as INotifyPropertyChanged;

            if (this.context != null)
            {
                this.context.PropertyChanged += this.OnPropertyChanged;
            }

            this.UpdateLocalized(newContext);
        }

        /// <summary>
        /// Called when context changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private void OnContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Sentinel bug!
            if (e.NewValue != null && e.NewValue.ToString() == "{DisconnectedItem}")
            {
                return;
            }

            this.OnContextChanged(e.NewValue);
        }

        /// <summary>
        /// Called when a property of the context changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.propertyPath)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.UpdateLocalized(this.element.DataContext);
                    }));
            }
        }

        /// <summary>
        /// Updates the localized string.
        /// </summary>
        /// <param name="contextObject">The context.</param>
        private void UpdateLocalized(object contextObject)
        {
            var key = this.GetKey(contextObject);

            if (key != null)
            {
                this.Localized = Localizer.GetString(key);
            }
            else
            {
                this.Localized = null;
            }
        }

        #endregion Methods
    }
}