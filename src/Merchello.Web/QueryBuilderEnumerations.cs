namespace Merchello.Web
{
    /// <summary>
    /// A setting for specifying how the query should treat collection clusivity in specified collections and filters
    /// </summary>
    public enum CollectionClusivity
    {
        /// <summary>
        /// Indicates the entity must exist in all collections and filters.
        /// </summary>
        ExistsInAllCollectionsAndFilters,

        /// <summary>
        /// Indicates the entity should exist in any of the collections or filters
        /// </summary>
        ExistsInAnyCollectionOrFilter,

        /// <summary>
        /// Indicates the entity cannot be contained in any of the collections and filters
        /// </summary>
        DoesNotExistInAnyCollectionsAndFilters
    }

    /// <summary>
    /// Valid product collection sort fields.
    /// </summary>
    public enum ProductSortField
    {
        /// <summary>
        /// The product name
        /// </summary>
        Name,

        /// <summary>
        /// The product price.
        /// </summary>
        Price,

        /// <summary>
        /// The product sale price.
        /// </summary>
        SalePrice,

        /// <summary>
        /// The product SKU 
        /// </summary>
        Sku,

        /// <summary>
        /// The product sell price (sale price if present) 
        /// </summary>
        SellPrice
    }
}