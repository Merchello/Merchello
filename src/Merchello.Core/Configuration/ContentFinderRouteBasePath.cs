namespace Merchello.Core.Configuration
{
    /// <inheritdoc/>
    internal class ContentFinderRouteBasePath : IContentFinderRouteBasePath
    {
        /// <inheritdoc/>
        public string CultureName { get; set; }

        /// <inheritdoc/>
        public string BasePath { get; set; }
    }
}