namespace Eberhard.Core.Utilities.Localization
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using System.Reflection;
	using System.Resources;
	using System.Threading;

	/// <summary>
	/// Localization settings
	/// </summary>
	public static class Localizer
	{
		#region Fields

		/// <summary>
		/// flag if this class is initialized
		/// </summary>
		private static bool initialized = InitializeLocalizer();

		/// <summary>
		/// flag if a default language is chosen
		/// </summary>
		private static bool defaultCultureChosen = false;

		/// <summary>
		/// stores the used strings
		/// </summary>
		private static Dictionary<string, LocalizedString> dictionary = new Dictionary<string, LocalizedString>();

		/// <summary>
		/// The resource list
		/// </summary>
		private static List<ResourceManager> resourceList;

		#endregion Fields

		#region Events

		/// <summary>
		/// Fires when culture changed.
		/// </summary>
		public static event EventHandler CultureChanged;

		#endregion Events

		#region Properties

		/// <summary>
		/// Gets the cultures.
		/// </summary>
		public static Collection<Culture> Cultures
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the current culture.
		/// </summary>
		public static Culture CurrentCulture
		{
			get;
			private set;
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Adds the culture.
		/// </summary>
		/// <param name="newCulture">The new culture.</param>
		public static void AddCulture(Culture newCulture)
		{
			

			if (Cultures.Contains(newCulture))
			{
				return;
			}

			Cultures.Add(newCulture);
		}

		/// <summary>
		/// Adds the resource manager.
		/// </summary>
		/// <param name="newManager">The new manager.</param>
		public static void AddResourceManager(ResourceManager newManager)
		{
			
			if (resourceList.Contains(newManager))
			{
				return;
			}

			resourceList.Add(newManager);

			if (!defaultCultureChosen)
			{
				ChangeCulture(Language.EN.ToString());
				defaultCultureChosen = true;
			}
		}

		/// <summary>
		/// Changes the culture.
		/// </summary>
		/// <param name="cultureId">The culture ID.</param>
		public static void ChangeCulture(string cultureId)
		{
			Culture newCulture = Cultures.FirstOrDefault(c => c.Identifier.ToUpper(CultureInfo.InvariantCulture) == cultureId.ToUpper(CultureInfo.InvariantCulture));

			Debug.Assert(newCulture != null, "new culture is null");

			if (newCulture != CurrentCulture)
			{
				if (CurrentCulture != null)
				{
					CurrentCulture.UpdateSelected(false);
				}

				Thread.CurrentThread.CurrentUICulture = newCulture.Info;

				newCulture.UpdateSelected(true);
				CurrentCulture = newCulture;

				foreach (var culture in Cultures)
				{
					culture.UpdateName();
				}

				if (CultureChanged != null)
				{
					CultureChanged(null, new EventArgs());
				}
			}
		}

		/// <summary>
		/// Localizes the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>the localized string</returns>
		public static string Localize(string key)
		{
			if (object.ReferenceEquals(null, resourceList))
			{
				InitializeResources();
			}

			return SearchForLocalization(key) ?? LocalizeNotFound(key);
		}

		/// <summary>
		/// Tries to localize the key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="result">The localized string</param>
		/// <returns>success flag</returns>
		public static bool TryLocalize(string key, out string result)
		{
			if (object.ReferenceEquals(null, resourceList))
			{
				InitializeResources();
			}

			result = SearchForLocalization(key);

			if (result == null)
			{
				result = LocalizeNotFound(key);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Gets the string context.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>string context</returns>
		public static LocalizedString GetString(string key)
		{
			LocalizedString localized;

			if (!dictionary.TryGetValue(key, out localized))
			{
				localized = new LocalizedString(key);

				dictionary.Add(key, localized);
			}

			return localized;
		}

		/// <summary>
		/// Initializes the localizer.
		/// </summary>
		/// <returns>true if the class is initialized, false otherwise</returns>
		private static bool InitializeLocalizer()
		{
			InitializeResources();

			Cultures = new Collection<Culture>();

			Cultures.Add(new Culture(Language.EN.ToString(), GetLanguageKey(Language.EN)));
			Cultures.Add(new Culture(Language.DE.ToString(), GetLanguageKey(Language.DE)));
            Cultures.Add(new Culture(Language.FR.ToString(), GetLanguageKey(Language.FR)));
            Cultures.Add(new Culture(Language.ES.ToString(), GetLanguageKey(Language.ES)));

            initialized = true;

			return initialized;
		}

		/// <summary>
		/// Gets the language description.
		/// </summary>
		/// <param name="enumerationValue">The enumeration value.</param>
		/// <returns>language description</returns>
		private static string GetLanguageKey(Language enumerationValue)
		{
			Type type = enumerationValue.GetType();
			string name = enumerationValue.ToString();

			// Tries to find a DescriptionAttribute for a potential friendly name for the enum
			MemberInfo[] member = type.GetMember(name);
			if (member != null && member.Length > 0)
			{
				object[] attributes = member[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

				if (attributes != null && attributes.Length > 0)
				{
					// Pull out the description value
					return ((DescriptionAttribute)attributes[0]).Description;
				}
			}

			return name;
		}

		/// <summary>
		/// Initializes the resources.
		/// </summary>
		private static void InitializeResources()
		{
			resourceList = new List<ResourceManager>();
		}

		/// <summary>
		/// Localizes a key not found.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>dummy string</returns>
		private static string LocalizeNotFound(string key)
		{
			return string.Format(CultureInfo.CurrentCulture, "{0}", key);
		}

		/// <summary>
		/// Searches for localization.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>null if no string was found, else the first match</returns>
		private static string SearchForLocalization(string key)
		{
			foreach (var manager in resourceList)
			{
				string result = manager.GetString(key, CurrentCulture.Info);
				if (!object.ReferenceEquals(null, result))
				{
					return result;
				}
			}

			return null;
		}

		#endregion Methods
	}
}