namespace Merchello.Core.Configuration.Mvc
{
    /// <summary>
    /// Represents a route prefix to be used in culture based routing.
    /// </summary>
    public interface ICultureRouteBasePath
    {
        /// <summary>
        /// Gets or sets the culture name.
        /// </summary>
        string CultureName { get; set; }

        /// <summary>
        /// Gets or sets the base path.
        /// </summary>
        string BasePath { get; set; }
    }
}