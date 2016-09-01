namespace Merchello.Web
{
    /// <summary>
    /// A setting for specifying how the query should treat collection clusivity in specified collections and filters
    /// </summary>
    public enum FilterQueryClusivity
    {
        /// <summary>
        /// Indicates the entity must exist in all collections and filters.
        /// </summary>
        ExistsInAllCollectionsAndFilters,

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
        /// The product SKU 
        /// </summary>
        Sku
    }
}