namespace Merchello.Core.Persistence.Repositories
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Factories;

    using Umbraco.Core.Persistence;

    /// <inheritdoc/>
    internal partial class ProductRepository : IProductBackOfficeRepository
    {
        /// <inheritdoc/>
        public PagedCollection<IProduct> GetRecentlyUpdatedProducts(long page, long itemsPerPage = 10)
        {
            var sql = GetBaseQuery(false);
            sql.OrderByDescending<ProductDto>(x => x.CreateDate, SqlSyntax);

            var results = Database.Page<ProductDto>(page, itemsPerPage, sql);

            var factory = new ProductFactory(
                _productOptionRepository.GetProductAttributeCollectionForVariant,
                _productVariantRepository.GetCategoryInventoryCollection,
                _productOptionRepository.GetProductOptionCollection,
                _productVariantRepository.GetProductVariantCollection,
                _productVariantRepository.GetDetachedContentCollection);

            return results.AsPagedCollection(factory.BuildEntity, "createDate");
        }
    }
}
