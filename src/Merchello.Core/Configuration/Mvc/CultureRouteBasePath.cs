namespace Merchello.Core.Configuration.Mvc
{
    /// <inheritdoc/>
    internal class CultureRouteBasePath : ICultureRouteBasePath
    {
        /// <inheritdoc/>
        public string CultureName { get; set; }

        /// <inheritdoc/>
        public string BasePath { get; set; }
    }
}