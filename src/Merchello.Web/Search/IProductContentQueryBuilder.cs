namespace Merchello.Web.Search
{
    using Merchello.Web.Models;
    using Merchello.Web.Models.VirtualContent;

    /// <summary>
    /// Marker interface for a ProductContentQueryBuilder.
    /// </summary>
    public interface IProductContentQueryBuilder : ICmsContentQueryBuilder<IProductCollection, IProductFilter, IProductContent>
    {
        /// <summary>
        /// Sets the price range constraints.
        /// </summary>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
        /// </param>
        void SetPriceRange(decimal min, decimal max);

        /// <summary>
        /// Clears the price range constraints.
        /// </summary>
        void ClearPriceRange();
    }
}