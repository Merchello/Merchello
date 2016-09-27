namespace Merchello.Core
{
    /// <summary>
    /// Constants Database.
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// Database provider names supported by Merchello.
        /// </summary>
        public static class DbProviderNames
        {

            /// <summary>
            /// Gets the MS Sql Server.
            /// </summary>
            public static string SqlServer
            {
                get
                {
                    return "System.Data.SqlClient";
                }
            }

            /// <summary>
            /// Gets the MS SQLCE.
            /// </summary>
            public static string SqlCe
            {
                get
                {
                    return "System.Data.SqlServerCe.4.0";
                }  
            } 
        }
    }
}