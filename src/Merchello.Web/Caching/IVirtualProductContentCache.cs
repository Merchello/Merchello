namespace Merchello.Web.Caching
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Web.Models.VirtualContent;

    internal interface IVirtualProductContentCache : IVirtualContentCache<IProductContent, IProduct>
    {
        IProductContent GetBySlug(string slug, Func<string, IProductContent> get);

        IProductContent GetBySku(string sku, Func<string, IProductContent> get);
    }
}