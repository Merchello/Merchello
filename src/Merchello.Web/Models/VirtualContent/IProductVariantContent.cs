namespace Merchello.Web.Models.VirtualContent
{
    using System;

    using Umbraco.Core.Models;

    public interface IProductVariantContent : IPublishedContent
    {
        Guid ProductKey { get; }
    }
}