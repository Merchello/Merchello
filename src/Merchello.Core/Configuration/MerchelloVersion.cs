namespace Merchello.Core.Configuration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    /// <summary>
    /// The merchello version.
    /// </summary>
    public class MerchelloVersion
    {
        /// <summary>
        /// The version.
        /// </summary>
        private static readonly Version Version = new Version("1.8.0.10");

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
        public static string CurrentComment
        {
            get { return "0"; }
        }

        /// <summary>
        /// Gets the assembly version.
        /// </summary>
        /// <remarks>
        /// Get the version of the Merchello by looking at a class in that dll
        /// Had to do it like this due to medium trust issues, see: http://haacked.com/archive/2010/11/04/assembly-location-and-medium-trust.aspx
        /// </remarks>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public static string AssemblyVersion
        {
            get { return new AssemblyName(typeof(MerchelloVersion).Assembly.FullName).Version.ToString(); }
        }
    }
}
