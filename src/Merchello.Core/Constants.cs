﻿namespace Merchello.Core
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
    }
}