namespace Merchello.Web.Search
{
    using Merchello.Web.Models;
    using Merchello.Web.Models.VirtualContent;

    /// <summary>
    /// Marker interface for a ProductContentQueryBuilder.
    /// </summary>
    public interface IProductContentQueryBuilder : ICmsContentQueryBuilder<IProductCollection, IProductFilter, IProductContent>
    {
    }
}