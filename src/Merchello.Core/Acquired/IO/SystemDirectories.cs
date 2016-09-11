namespace Merchello.Core.Acquired.IO
{
    using System.Web;

    /// <summary>
    /// Known s.
    /// </summary>
    /// <remarks>
    /// All paths has a starting but no trailing /
    /// Partial UMBRACO
    /// </remarks>
    internal class SystemDirectories
    {
        /// <summary>
        /// The root.
        /// </summary>
        private static string _root;

        /// <summary>
        /// Gets the root path of the application
        /// </summary>
        public static string Root
        {
            get
            {
                if (_root != null)
                {
                    var appPath = HttpRuntime.AppDomainAppVirtualPath ?? string.Empty;
                    if (appPath == "/")
                        appPath = string.Empty;

                    _root = appPath;
                }

                return _root;
            }

            //Only required for unit tests
            internal set
            {
                _root = value;
            }
        }
    }
}