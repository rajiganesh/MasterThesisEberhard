namespace Eberhard.Core.Utilities.Localization
{
	using System;
	using System.Collections.ObjectModel;
	using System.Windows;
	using System.Windows.Markup;

	/// <summary>
	/// A base class for localization
	/// </summary>
	public static class LocalizationBase
	{
		/// <summary>
		/// Gets the cultures.
		/// </summary>
		public static Collection<Culture> Cultures
		{
			get
			{
				return Localizer.Cultures;
			}
		}

		#region Methods

		/// <summary>
		/// Tries to retrieve a dependency property and framework element.
		/// </summary>
		/// <param name="serviceProvider">The service provider.</param>
		/// <param name="element">The element.</param>
		/// <param name="property">The property.</param>
		/// <returns>success flag</returns>
		internal static bool TryGetTarget(IServiceProvider serviceProvider, out FrameworkElement element, out object property)
		{
			if (serviceProvider != null)
			{
				IProvideValueTarget service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

				if (service != null)
				{
					property = service.TargetProperty;
					element = service.TargetObject as FrameworkElement;

					if (element != null)
					{
						return true;
					}
				}
			}

			element = null;
			property = null;

			return false;
		}

		#endregion Methods
	}
}