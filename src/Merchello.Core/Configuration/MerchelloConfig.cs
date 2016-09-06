namespace Merchello.Core.Configuration
{
    using System;
    using System.Configuration;

    using Merchello.Core.Configuration.Sections;

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
        private IMerchelloSettingsSection _settings;

        /// <summary>
        /// The extensibility section.
        /// </summary>
        private IMerchelloExtensibilitySection _extensibility;

        /// <summary>
        /// The countries section.
        /// </summary>
        private IMerchelloCountriesSection _countries;

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
            if (_settings == null)
            {
                _settings = ConfigurationManager.GetSection("merchello/merchelloSettings") as IMerchelloSettingsSection;
            }

            if (_extensibility == null)
            {
                _extensibility = ConfigurationManager.GetSection("merchello/merchelloExtensibility") as IMerchelloExtensibilitySection;
            }

            if (_countries == null)
            {
                _countries = ConfigurationManager.GetSection("merchello/merchelloCountries") as IMerchelloCountriesSection;
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
        /// Gets the Merchello <see cref="IMerchelloSettingsSection"/>.
        /// </summary>
        public IMerchelloSettingsSection Settings
        {
            get
            {
                return _settings;
            }
        }

        /// <summary>
        /// Gets the Merchello <see cref="IMerchelloExtensibilitySection"/>.
        /// </summary>
        public IMerchelloExtensibilitySection Extensibility
        {
            get
            {
                return _extensibility;
            }
        }

        /// <summary>
        /// Gets the Merchello <see cref="IMerchelloCountriesSection"/>.
        /// </summary>
        public IMerchelloCountriesSection Countries
        {
            get
            {
                return _countries;
            }
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
            _settings = value;
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
            _extensibility = value;
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
            _countries = value;
        }
    }
}