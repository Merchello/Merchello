namespace Merchello.Core.EntityCollections.Providers
{
    using System;

    /// <summary>
    /// Represents the product based filter collection.
    /// </summary>
    /// <remarks>
    /// EntityFilterProviders need to implement <see cref="IEntityFilterGroupProvider"/>
    /// </remarks>
    [EntityCollectionProvider("5316C16C-E967-460B-916B-78985BB7CED2", "9F923716-A022-4089-A110-1E9B4E1F2AD1", "Product Filter Collection", "A collection of product filters that could be used for product filters and custom product groupings", false)]
    public class ProductFilterGroupProvider : ProductFilterGroupProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFilterGroupProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public ProductFilterGroupProvider(IMerchelloContext merchelloContext, Guid collectionKey)
            : base(merchelloContext, collectionKey)
        {
        }
    }
}