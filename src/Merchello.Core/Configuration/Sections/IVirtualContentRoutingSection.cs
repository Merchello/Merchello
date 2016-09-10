namespace Merchello.Core.Configuration.Sections
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a configuration section for configurations related to the custom Merchello MVC routes.
    /// </summary>
    public interface IVirtualContentRoutingSection : IMerchelloSection
    {
        /// <inheritdoc/>
        IEnumerable<IContentFinderRouteBasePath> SlugContentFinderRouteBasePath { get; }
    }
}