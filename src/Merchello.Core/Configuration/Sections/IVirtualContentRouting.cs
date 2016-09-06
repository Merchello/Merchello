namespace Merchello.Core.Configuration.Sections
{
    using System.Collections.Generic;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents a configuration section for configurations related to the custom Merchello MVC routes.
    /// </summary>
    public interface IVirtualContentRouting : IMerchelloConfigurationSection
    {
        /// <inheritdoc/>
        IEnumerable<IContentFinderRouteBasePath> SlugContentFinderRouteBasePath { get; }
    }
}