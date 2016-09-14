namespace Merchello.Core
{
    /// <summary>
    /// Constants all the identifiers within the Merchello core.
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// Gets the default culture name.
        /// </summary>
        public static string DefaultCultureName
        {
            get
            {
                return "en-US";
            }
        }

        /// <summary>
        /// Gets the default connection string name.
        /// </summary>
        public static string DefaultConnectionStringName
        {
            get { return "umbracoDbDSN"; }
        }

        /// <summary>
        /// Constant Merchello specific country codes
        /// </summary>
        public static class CountryCodes
        {
            /// <summary>
            /// Gets the everywhere else country code.
            /// </summary>
            public static string EverywhereElse
            {
                get { return "ELSE"; }
            }
        }

        /// <summary>
        /// Database provider names supported by Merchello.
        /// </summary>
        public static class DbProviderNames
        {
            /// <summary>
            /// MS Sql Server.
            /// </summary>
            public const string SqlServer = "System.Data.SqlClient";

            /// <summary>
            /// MS SqlCe.
            /// </summary>
            public const string SqlCe = "System.Data.SqlServerCe.4.0";
        }
    }
}