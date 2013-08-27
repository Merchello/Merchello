using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Merchello.Core.Configuration.Outline;
using Umbraco.Core.IO;

namespace Merchello.Core.Configuration
{
    /// <summary>
    /// Provides quick access to the Merchello configuration section.
    /// </summary>
    public sealed class MerchelloConfiguration
    {
        private static string _rootDir = "";

        /// <summary>
        /// Name of the application.
        /// </summary>
        public static string ApplicationName = "Merchello";
        public static string ConfigurationName = ApplicationName.ToLower();

        public static MerchelloSection Section
        {
            get { return (MerchelloSection)ConfigurationManager.GetSection(ConfigurationName); }
        }
        

        /// <summary>
        /// Returns the path to the root of the application, by getting the path to where the assembly where this
        /// method is included is present, then traversing until it's past the /bin directory. Ie. this makes it work
        /// even if the assembly is in a /bin/debug or /bin/release folder
        /// </summary>
        /// <returns></returns>
        internal static string GetRootDirectorySafe()
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
        public static string FullpathToRoot
        {
            get { return GetRootDirectorySafe(); }
        }
    }
}
