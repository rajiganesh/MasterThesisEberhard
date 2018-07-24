namespace Eberhard.Core.Utilities.Localization
{
    using Eberhard.Core.Utilities;
    using System.Globalization;

    /// <summary>
    /// Wrapper for cultures
    /// </summary>
    public class Culture : NotifingObject
    {
		#region Fields

		/// <summary>
		/// stores whether selected
		/// </summary>
		private bool isSelected;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Culture"/> class.
		/// </summary>
		/// <param name="cultureId">The culture ID.</param>
		/// <param name="localizationKey">The localization key.</param>
		public Culture(string cultureId, string localizationKey)
		{
			this.Identifier = cultureId;
			this.LocalizationKey = localizationKey;

			this.Info = CultureInfo.GetCultureInfo(cultureId);
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets the operation system language.
		/// </summary>
		/// <value>
		/// The operation system language.
		/// </value>
		public static Language OperationSystemLanguage
		{
			get
			{
				switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
				{
					case "de":
						return Language.DE;
                    case "fr":
                        return Language.FR;
                    case "es":
                        return Language.ES;
                    case "en":
					default:
						return Language.EN;
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is selected.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
		/// </value>
		public bool IsSelected
		{
			get
			{
				return this.isSelected;
			}

			set
			{
				if (value)
				{
					Localizer.ChangeCulture(this.Identifier);
				}
			}
		}

		/// <summary>
		/// Gets the info.
		/// </summary>
		public CultureInfo Info
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the ID.
		/// </summary>
		public string Identifier
		{
			get;
			internal set;
		}

		/// <summary>
		/// Gets the localization key.
		/// </summary>
		public string LocalizationKey
		{
			get;
			internal set;
		}

		/// <summary>
		/// Gets the name of the localized.
		/// </summary>
		/// <value>
		/// The name of the localized.
		/// </value>
		public string LocalizedName
		{
			get;
			private set;
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Returns a <see cref="string" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="string" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return this.Identifier;
		}

		/// <summary>
		/// Updates the name.
		/// </summary>
		internal void UpdateName()
		{
			this.LocalizedName = Localizer.Localize(this.LocalizationKey);

			this.FirePropertyChanged("LocalizedName");
		}

		/// <summary>
		/// Updates the IsSelected property.
		/// </summary>
		/// <param name="selected">if set to <c>true</c> [is selected].</param>
		internal void UpdateSelected(bool selected)
		{
			this.isSelected = selected;

			this.FirePropertyChanged("IsSelected");
		}

		#endregion Methods
	}
}