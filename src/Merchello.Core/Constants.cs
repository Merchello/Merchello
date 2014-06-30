namespace Merchello.Core
{
    /// <summary>
    /// Constants all the identifiers within the Merchello core.
    /// </summary>
    public static partial class Constants
    {
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
    }
}