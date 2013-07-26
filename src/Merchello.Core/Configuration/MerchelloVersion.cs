using System;
using System.Reflection;

namespace Merchello.Core.Configuration
{
    public class MerchelloVersion
    {
        private static readonly Version Version = new Version("0.9.0.0");

        /// <summary>
        /// Gets the current version of Merchello.
        /// Version class with the specified major, minor, build (Patch), and revision numbers.
        /// </summary>
        /// <remarks>
        /// CURRENT MERCHELLO VERSION ID.
        /// </remarks>
        public static Version Current
        {
            get { return Version; }
        }

        /// <summary>
        /// Gets the version comment (like beta or RC).
        /// </summary>
        /// <value>The version comment.</value>
        public static string CurrentComment { get { return "0"; } }

        // Get the version of the umbraco.dll by looking at a class in that dll
        // Had to do it like this due to medium trust issues, see: http://haacked.com/archive/2010/11/04/assembly-location-and-medium-trust.aspx
        public static string AssemblyVersion { get { return new AssemblyName(typeof(MerchelloVersion).Assembly.FullName).Version.ToString(); } }
    }
}
