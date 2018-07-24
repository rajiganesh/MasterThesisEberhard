namespace Eberhard.Core.Utilities.Localization
{
	using System;
	using System.Windows;
	using System.Windows.Data;
	using System.Windows.Markup;

	/// <summary>
	/// resolves localized value of bound property
	/// </summary>
	public class BindingExtension : MarkupExtension
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="BindingExtension"/> class.
		/// </summary>
		/// <param name="propertyPath">The property.</param>
		public BindingExtension(string propertyPath)
		{
			this.PropertyPath = propertyPath;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the property.
		/// </summary>
		public string PropertyPath
		{
			get;
			private set;
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Provides the value.
		/// </summary>
		/// <param name="serviceProvider">The service provider.</param>
		/// <returns>the value</returns>
		public override object ProvideValue(System.IServiceProvider serviceProvider)
		{
			object property;
			FrameworkElement element;

			if (LocalizationBase.TryGetTarget(serviceProvider, out element, out property))
			{
				if (property is DependencyProperty)
				{
					var context = new LocalizedBinding(element, this.PropertyPath);

					Application.Current.Dispatcher.BeginInvoke(
						new Action(() => element.SetBinding(
							(DependencyProperty)property,
							new Binding
							{
								Source = context,
								Path = new PropertyPath("Localized.Value"),
								Mode = BindingMode.OneWay
							})));
				}

				var key = this.GetKey(element);

				if (!string.IsNullOrWhiteSpace(key))
				{
					LocalizedString localized = Localizer.GetString(key);
					return localized.Value;
				}

				return null;
			}

			return this;
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="string"/> that represents this instance.
		/// </returns>
		public new virtual string ToString()
		{
			return "BindingExtension";
		}

		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <param name="element">The framework element.</param>
		/// <returns>key or null</returns>
		private string GetKey(FrameworkElement element)
		{
			return LocalizedBinding.ResolveKey(element.DataContext, this.PropertyPath);
		}

		#endregion Methods
	}
}