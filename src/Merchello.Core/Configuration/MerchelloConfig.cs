namespace Merchello.Core.Configuration
{
    using System;
    using System.Configuration;

    using Merchello.Core.Configuration.Sections;
    using Merchello.Core.Logging;

    /// <summary>
    /// Provides access to configurations in the Merchello configuration files.
    /// <para>merchelloSettings.config, merchelloExtensibility.config and merchelloCountries.config</para>
    /// </summary>
    public class MerchelloConfig
    {
        /// <summary>
        /// Configuration singleton.
        /// </summary>
        private static readonly Lazy<MerchelloConfig> config = new Lazy<MerchelloConfig>(() => new MerchelloConfig());

        /// <summary>
        /// The settings section.
        /// </summary>
        private IMerchelloSettingsSection _merchelloSettings;

        /// <summary>
        /// The extensibility section.
        /// </summary>
        private IMerchelloExtensibilitySection _merchelloExtensibility;

        /// <summary>
        /// The countries section.
        /// </summary>
        private IMerchelloCountriesSection _merchelloCountries;

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloConfig"/> class.
        /// </summary>
        /// <param name="settings">
        /// The <see cref="IMerchelloSettingsSection"/>.
        /// </param>
        /// <param name="extensibility">
        /// The <see cref="IMerchelloExtensibilitySection"/>.
        /// </param>
        /// <param name="countries">
        /// The <see cref="IMerchelloCountriesSection"/>.
        /// </param>
        public MerchelloConfig(
            IMerchelloSettingsSection settings,
            IMerchelloExtensibilitySection extensibility,
            IMerchelloCountriesSection countries)
        {
            SetMerchelloSettings(settings);
            SetMerchelloExtensibility(extensibility);
            SetMerchelloCountries(countries);
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="MerchelloConfig"/> class from being created.
        /// </summary>
        private MerchelloConfig()
        {
            if (this._merchelloSettings == null)
            {
                this._merchelloSettings = ConfigurationManager.GetSection("merchello/settings") as IMerchelloSettingsSection;
            }

            if (this._merchelloExtensibility == null)
            {
                this._merchelloExtensibility = ConfigurationManager.GetSection("merchello/extensibility") as IMerchelloExtensibilitySection;
            }

            if (this._merchelloCountries == null)
            {
                this._merchelloCountries = ConfigurationManager.GetSection("merchello/countries") as IMerchelloCountriesSection;
            }
        } 

        /// <summary>
        /// Gets the <see cref="MerchelloConfig"/>.
        /// </summary>
        public static MerchelloConfig For
        {
            get
            {
                return config.Value;
            }
        }

        /// <summary>
        /// Gets the Merchello Settings configuration section.
        /// </summary>
        /// <returns>
        /// The <see cref="IMerchelloSettingsSection"/>.
        /// </returns>
        /// <exception cref="ConfigurationErrorsException">
        /// Throws if the MerchelloSettingsSection is null
        /// </exception>
        public IMerchelloSettingsSection MerchelloSettings()
        {
            if (_merchelloSettings == null)
            {
                var ex = new ConfigurationErrorsException("Could not load the " + typeof(IMerchelloSettingsSection) + " from config file, ensure the web.config and merchelloSettings.config files are formatted correctly");
                MultiLogHelper.Error<MerchelloConfig>("Config error", ex);
                throw ex;
            }

            return _merchelloSettings;
        }

        /// <summary>
        /// Gets the Merchello Extensibility configuration section.
        /// </summary>
        /// <returns>
        /// The <see cref="IMerchelloExtensibilitySection"/>.
        /// </returns>
        /// <exception cref="ConfigurationErrorsException">
        /// Throws if the MerchelloExtensibilitySection is null
        /// </exception>
        public IMerchelloExtensibilitySection MerchelloExtensibility()
        {
            if (_merchelloExtensibility == null)
            {
                var ex = new ConfigurationErrorsException("Could not load the " + typeof(IMerchelloExtensibilitySection) + " from config file, ensure the web.config and merchelloExtensibility.config files are formatted correctly");
                MultiLogHelper.Error<MerchelloConfig>("Config error", ex);
                throw ex;
            }

            return _merchelloExtensibility;
        }

        /// <summary>
        /// Gets the Merchello Countries Configuration Section.
        /// </summary>
        /// <returns>
        /// The <see cref="IMerchelloCountriesSection"/>.
        /// </returns>
        /// <exception cref="ConfigurationErrorsException">
        /// Throws if the MerchelloCountriesSection is null
        /// </exception>
        public IMerchelloCountriesSection MerchelloCountries()
        {
            if (_merchelloCountries == null)
            {
                var ex = new ConfigurationErrorsException("Could not load the " + typeof(IMerchelloCountriesSection) + " from config file, ensure the web.config and merchelloCountries.config files are formatted correctly");
                MultiLogHelper.Error<MerchelloConfig>("Config error", ex);
                throw ex;
            }

            return _merchelloCountries;
        }

        /// <summary>
        /// Sets the <see cref="IMerchelloSettingsSection"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <remarks>
        /// Used for testing.
        /// </remarks>
        public void SetMerchelloSettings(IMerchelloSettingsSection value)
        {
            this._merchelloSettings = value;
        }

        /// <summary>
        /// Sets the <see cref="IMerchelloExtensibilitySection"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <remarks>
        /// Used for testing.
        /// </remarks>
        public void SetMerchelloExtensibility(IMerchelloExtensibilitySection value)
        {
            this._merchelloExtensibility = value;
        }

        /// <summary>
        /// Sets the <see cref="IMerchelloCountriesSection"/>.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <remarks>
        /// Used for testing
        /// </remarks>
        public void SetMerchelloCountries(IMerchelloCountriesSection value)
        {
            this._merchelloCountries = value;
        }
    }
}