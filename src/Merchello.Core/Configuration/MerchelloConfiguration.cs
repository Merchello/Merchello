using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Merchello.Core.Configuration.Outline;
using Umbraco.Core.Logging;

namespace Merchello.Core.Configuration
{
    /// <summary>
    /// Provides quick access to the Merchello configuration section.
    /// </summary>
    public sealed class MerchelloConfiguration
    {
        #region SingleTon

        private static readonly Lazy<MerchelloConfiguration> Lazy = new Lazy<MerchelloConfiguration>(() => new MerchelloConfiguration());

        public static MerchelloConfiguration Current
        {
            get { return Lazy.Value; }
        }

        #endregion


        private string _rootDir = "";

        /// <summary>
        /// Name of the application.
        /// </summary>
        public static string ApplicationName = "Merchello";
        public static string ConfigurationName = ApplicationName.ToLower();

        /// <summary>
        /// Returns the <see cref="MerchelloSection"/> Configuration Element
        /// </summary>
        public MerchelloSection Section
        {
            get { return (MerchelloSection)ConfigurationManager.GetSection(ConfigurationName); }
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

        public string DefaultSkuSeparator
        {
            get { return Section.Settings["DefaultSkuSeparator"].Value; }
        }

        public bool AlwaysApproveOrderCreation
        {
            get { return bool.Parse(Section.Settings["AlwaysApproveOrderCreation"].Value); }
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
        /// Returns the path to the root of the application, by getting the path to where the assembly where this
        /// method is included is present, then traversing until it's past the /bin directory. Ie. this makes it work
        /// even if the assembly is in a /bin/debug or /bin/release folder
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the full path to root.
        /// </summary>
        /// <value>The fullpath to root.</value>
        public string FullpathToRoot
        {
            get { return GetRootDirectorySafe(); }
        }
    }
}
