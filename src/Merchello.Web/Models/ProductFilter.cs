namespace Merchello.Web.Models
{
    using Merchello.Core.Models.Interfaces;
    using Merchello.Web.Models.Ui.Rendering;

    /// <summary>
    /// Represents a product filter.
    /// </summary>
    public class ProductFilter : EntityCollectionProxyBase, IProductFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFilter"/> class.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public ProductFilter(IEntityCollection collection)
            : base(collection)
        {
        }
    }
}