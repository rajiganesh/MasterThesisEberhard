namespace Eberhard.Core.Utilities.Localization
{
    using Eberhard.Core.Utilities;
    using System;

    /// <summary>
    /// data context for simple text
    /// </summary>
    public class LocalizedString : NotifingObject
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedString" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public LocalizedString(string key)
        {
            this.Key = key;

            this.Value = Localizer.Localize(key);

            Localizer.CultureChanged += new EventHandler(this.Localizer_CultureChanged);
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

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Handles the CultureChanged event of the Localizer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Localizer_CultureChanged(object sender, EventArgs e)
        {
            this.Value = Localizer.Localize(this.Key);

            this.FirePropertyChanged("Value");
        }

        #endregion Methods
    }
}