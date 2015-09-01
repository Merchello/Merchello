namespace Merchello.Web.Models.DetachedContent
{
    using System;

    using Merchello.Core.Models;

    using Umbraco.Core.Models;

    /// <summary>
    /// Defines a virtual product content.
    /// </summary>
    public interface IVirtualProductContent : IPublishedContent
    {
        Guid Key { get; }

        string CultureName { get; }
    }
}