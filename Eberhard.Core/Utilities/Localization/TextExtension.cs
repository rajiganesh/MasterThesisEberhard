namespace Eberhard.Core.Utilities.Localization
{
	using System;
	using System.Windows;
	using System.Windows.Data;
	using System.Windows.Markup;

	/// <summary>
	/// Localizes a fixed key
	/// </summary>
	public class TextExtension : MarkupExtension
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TextExtension"/> class.
		/// </summary>
		public TextExtension()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TextExtension"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		public TextExtension(string key)
		{
			this.Key = key;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <value>
		/// The key.
		/// </value>
		public string Key
		{
			get;
			private set;
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
		/// </summary>
		/// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
		/// <returns>
		/// The object value to set on the property where the extension is applied.
		/// </returns>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			object property;
			FrameworkElement element;

			if (LocalizationBase.TryGetTarget(serviceProvider, out element, out property))
			{
				LocalizedString context = Localizer.GetString(this.Key);

				if (property is DependencyProperty)
				{
					Application.Current.Dispatcher.BeginInvoke(
						new Action(() => element.SetBinding(
							(DependencyProperty)property,
							new Binding
							{
								Source = context,
								Path = new PropertyPath("Value"),
								Mode = BindingMode.OneWay
							})));
				}

				return context.Value;
			}

			return this;
		}

		/// <summary>
		/// Returns a <see cref="string" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="string" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return Localizer.Localize(this.Key);
		}

		#endregion Methods
	}
}