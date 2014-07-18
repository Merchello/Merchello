namespace Merchello.Core.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;
    using Outline;
    using Umbraco.Core.IO;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Provides quick access to the Merchello configuration section.
    /// </summary>
    public sealed class MerchelloConfiguration
    {
        /// <summary>
        /// Gets the application name.
        /// </summary>
        public const string ApplicationName = "Merchello";

        /// <summary>
        /// Gets the merchello migration name.
        /// </summary>
        /// <remarks>
        /// Configuration Status - (Upgrades) 
        /// </remarks>
        public const string MerchelloMigrationName = "Merchello";

        /// <summary>
        /// The lazy loaded configuration section
        /// </summary>
        private static readonly Lazy<MerchelloConfiguration> Lazy = new Lazy<MerchelloConfiguration>(() => new MerchelloConfiguration());

        /// <summary>
        /// The root directory.
        /// </summary>
        private string _rootDir = string.Empty;

        /// <summary>
        /// Gets the current instance
        /// </summary>
        public static MerchelloConfiguration Current
        {
            get { return Lazy.Value; }
        }

        /// <summary>
        /// Gets the configuration name.
        /// </summary>
        public static string ConfigurationName
        {
            get { return ApplicationName.ToLower(); }   
        } 
       

        /// <summary>
        /// Gets or sets the configuration status. This will return the version number of the currently installed merchello instance.
        /// </summary>
        /// <value>The configuration status.</value>
        public static string ConfigurationStatus
        {
            get
            {
                return ConfigurationManager.AppSettings.Cast<object>().Any(x => (string) x == "merchelloConfigurationStatus")
                    ? ConfigurationManager.AppSettings["merchelloConfigurationStatus"]
                    : string.Empty;
            }

            set
            {
                SaveAppSetting("merchelloConfigurationStatus", value);
            }
        }
        
        /// <summary>
        /// Gets the <see cref="MerchelloSection"/> Configuration Element
        /// </summary>
        public MerchelloSection Section
        {
            get { return (MerchelloSection)ConfigurationManager.GetSection(ConfigurationName); }
        }

        /// <summary>
        /// Gets the customer member types.
        /// </summary>
        public IEnumerable<string> CustomerMemberTypes
        {
            get
            {
                return Section.Customer.MemberTypes.Split(',').Select(x => x.Trim());
            }
        }

        /// <summary>
        /// Gets the default SKU separator.
        /// </summary>
        public string DefaultSkuSeparator
        {
            get { return Section.Settings["DefaultSkuSeparator"].Value; }
        }

        /// <summary>
        /// Gets a value indicating whether to always approve order creation.
        /// </summary>
        public bool AlwaysApproveOrderCreation
        {
            get { return bool.Parse(Section.Settings["AlwaysApproveOrderCreation"].Value); }
        }

        /// <summary>
        /// Gets a value indicating whether auto update database schema.
        /// </summary>
        /// <remarks>
        /// If true, Merchello will automatically attempt to update the database schema (if required) 
        /// when the boot strapping detects a Merchello version update
        /// </remarks>
        public bool AutoUpdateDbSchema
        {
            get { return bool.Parse(Section.Settings["AutoUpdateDbSchema"].Value); }
        }

        /// <summary>
        /// Gets the days beyond which the anonymous customers will be deleted via a scheduled task.
        /// </summary>
        /// <remarks>
        /// The number of days beyond which the anonymous customers will be deleted via a scheduled task.
        /// </remarks>
        public int AnonymousCustomersMaxDays
        {
            get { return int.Parse(Section.Settings["AnonymousCustomersMaxDays"].Value); }
        }

        /// <summary>
        /// Gets the pattern formatter.
        /// </summary>
        /// <remarks>
        /// Returns the pattern formatter for a given group
        /// </remarks>
        public ReplacementCollection PatternFormatter
        {
            get
            {
                try
                {
                    return Section.PatternFormatter;
                }
                catch (Exception ex)
                {
                    LogHelper.Info<MerchelloConfiguration>(ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the back office.
        /// </summary>
        public TreeCollection BackOffice
        {
            get
            {
                try
                {
                    return Section.BackOffice;
                }
                catch (Exception ex)
                {
                    LogHelper.Info<MerchelloConfiguration>(ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the full path to root.
        /// </summary>
        /// <value>The fullpath to root.</value>
        public string FullpathToRoot
        {
            get { return GetRootDirectorySafe(); }
        }

        /// <summary>
        /// Gets the <see cref="StrategyElement"/> by it's configuration alias
        /// </summary>
        /// <param name="alias">The alias (configuration key) of the <see cref="StrategyElement"/></param>
        /// <returns>
        /// <see cref="StrategyElement"/>
        /// </returns>
        public StrategyElement GetStrategyElement(string alias)
        {
            try
            {
                return Section.Strategies[alias];
            }
            catch (Exception ex)
            {
                LogHelper.Info<MerchelloConfiguration>(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets a <see cref="TaskChainElement"/> by its configuration alias
        /// </summary>
        /// <param name="alias">The alias (configuration key) of the <see cref="TaskChainElement"/></param>
        /// <returns><see cref="TaskChainElement"/></returns>
        public TaskChainElement GetTaskChainElement(string alias)
        {
            try
            {
                return Section.TaskChains[alias];
            }
            catch (Exception ex)
            {
                LogHelper.Info<MerchelloConfiguration>(ex.Message);
                return null;
            }
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
                LogHelper.Info<MerchelloConfiguration>(ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Saves a setting into the web configuration file.
        /// </summary>
        /// <param name="key">Key of the setting to be saved.</param>
        /// <param name="value">Value of the setting to be saved.</param>
        internal static void SaveAppSetting(string key, string value)
        {
            var fileName = IOHelper.MapPath(string.Format("{0}/web.config", SystemDirectories.Root));
            var xml = XDocument.Load(fileName, LoadOptions.PreserveWhitespace);

            var appSettings = xml.Root.DescendantsAndSelf("appSettings").Single();

            // Update appSetting if it exists, or else create a new appSetting for the given key and value
            var setting = appSettings.Descendants("add").FirstOrDefault(s => s.Attribute("key").Value == key);
            if (setting == null)
                appSettings.Add(new XElement("add", new XAttribute("key", key), new XAttribute("value", value)));
            else
                setting.Attribute("value").Value = value;

            xml.Save(fileName, SaveOptions.DisableFormatting);
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// Returns the path to the root of the application, by getting the path to where the assembly where this
        /// method is included is present, then traversing until it's past the /bin directory. This makes it work
        /// even if the assembly is in a /bin/debug or /bin/release folder
        /// </summary>
        /// <returns>
        /// The root directory path
        /// </returns>
        internal string GetRootDirectorySafe()
        {
            if (string.IsNullOrEmpty(_rootDir) == false)
            {
                return _rootDir;
            }

            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new Uri(codeBase);
            var path = uri.LocalPath;
            var baseDirectory = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(baseDirectory))
                throw new Exception("No root directory could be resolved. Please ensure that your Umbraco solution is correctly configured.");

            _rootDir = baseDirectory.Contains("bin")
                           ? baseDirectory.Substring(0, baseDirectory.LastIndexOf("bin", StringComparison.OrdinalIgnoreCase) - 1)
                           : baseDirectory;

            return _rootDir;
        }
    }
}
