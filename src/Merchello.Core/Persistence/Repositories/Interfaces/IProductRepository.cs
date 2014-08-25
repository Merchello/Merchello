namespace Merchello.Core.Persistence.Repositories
{
    using Models;
    using Models.Rdbms;

    /// <summary>
    /// Marker interface for the address repository
    /// </summary>
    public interface IProductRepository : IPagedRepository<IProduct, ProductDto>
    {
        /// <summary>
        /// Gets or sets a value Indicating whether or not a sku is already exists in the database
        /// </summary>
        /// <param name="sku">The sku to be tested</param>
        /// <returns>A value indicating whether or not a sku is already exists in the database</returns>
        bool SkuExists(string sku);
    }
}
