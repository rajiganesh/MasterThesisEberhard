namespace Eberhard.Core.Utilities.Localization
{
	using System.ComponentModel;

	/// <summary>
	/// Contains all languages including localization key = Description
	/// </summary>
	public enum Language
	{
		/// <summary>
		/// English language
		/// </summary>
		[Description("en-US")]
		EN,

		/// <summary>
		/// German language
		/// </summary>
		[Description("de-DE")]
		DE,

        /// <summary>
		/// French language
		/// </summary>
		[Description("fr-FR")]
        FR,

        [Description("es-MX")]
        ES,


    }
}