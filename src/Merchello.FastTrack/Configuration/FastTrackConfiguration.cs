namespace Merchello.FastTrack.Configuration
{
    using System;
    using System.Configuration;
    using Core.Logging;

    /// <summary>
    /// Provides quick access to the FastTrack configuration section.
    /// </summary>
    public sealed class FastTrackConfiguration
    {
        /// <summary>
        /// Gets the application name.
        /// </summary>
        public const string ApplicationName = "FastTrack";

        /// <summary>
        /// Gets the configuration name.
        /// </summary>
        public static string ConfigurationName
        {
            get { return ApplicationName.ToLower(); }
        }

        /// <summary>
        /// The lazy loaded configuration section
        /// </summary>
        private static readonly Lazy<FastTrackConfiguration> Lazy = new Lazy<FastTrackConfiguration>(() => new FastTrackConfiguration());

        /// <summary>
        /// The root directory.
        /// </summary>
        private string _rootDir = string.Empty;

        /// <summary>
        /// Gets the current instance
        /// </summary>
        public static FastTrackConfiguration Current
        {
            get { return Lazy.Value; }
        }


        /// <summary>
        /// Gets the <see cref="FastTrackSection"/> Configuration Element
        /// </summary>
        public FastTrackSection Section
        {
            get { return (FastTrackSection)ConfigurationManager.GetSection(ConfigurationName); }
        }

        /// <summary>
        /// Get the Store document type alias
        /// </summary>
        public string ContentTypeAliasStore
        {
            get { return Section.Settings["ContentTypeAliasStore"].Value; }
        }

        /// <summary>
        /// Get the Basket document type alias
        /// </summary>
        public string ContentTypeAliasBasket
        {
            get { return Section.Settings["ContentTypeAliasBasket"].Value; }
        }

        /// <summary>
        /// Get the Catalog document type alias
        /// </summary>
        public string ContentTypeAliasCatalog
        {
            get { return Section.Settings["ContentTypeAliasCatalog"].Value; }
        }

        /// <summary>
        /// Get the Checkout document type alias
        /// </summary>
        public string ContentTypeAliasCheckout
        {
            get { return Section.Settings["ContentTypeAliasCheckout"].Value; }
        }

        /// <summary>
        /// Get the Receipt document type alias
        /// </summary>
        public string ContentTypeAliasReceipt
        {
            get { return Section.Settings["ContentTypeAliasReceipt"].Value; }
        }

        /// <summary>
        /// Get the Account document type alias
        /// </summary>
        public string ContentTypeAliasAccount
        {
            get { return Section.Settings["ContentTypeAliasAccount"].Value; }
        }

        /// <summary>
        /// The get setting.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> value of the setting.
        /// </returns>
        public string GetSetting(string alias)
        {
            try
            {
                return Section.Settings[alias].Value;
            }
            catch (Exception ex)
            {
                MultiLogHelper.Info<FastTrackConfiguration>(ex.Message);
                return string.Empty;
            }
        }

    }
}
