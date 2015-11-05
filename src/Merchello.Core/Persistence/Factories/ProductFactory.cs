namespace Merchello.Core.Persistence.Factories
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Core.Models.DetachedContent;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The product factory.
    /// </summary>
    internal class ProductFactory : IEntityFactory<IProduct, ProductDto>
    {
        /// <summary>
        /// The product variant factory.
        /// </summary>
        private readonly ProductVariantFactory _productVariantFactory;

        /// <summary>
        /// The product option collection.
        /// </summary>
        private readonly Func<Guid, ProductOptionCollection> _getProductOptionCollection;

        /// <summary>
        /// The product variant collection.
        /// </summary>
        private readonly Func<Guid, ProductVariantCollection> _getProductVariantCollection;


        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFactory"/> class.
        /// </summary>
        public ProductFactory()
            : this(
                pac => new ProductAttributeCollection(),
                cic => new CatalogInventoryCollection(),
                poc => new ProductOptionCollection(),
                pvc => new ProductVariantCollection(),
                dcc => new DetachedContentCollection<IProductVariantDetachedContent>())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFactory"/> class.
        /// </summary>
        /// <param name="getProductAttributes">
        /// The product attributes.
        /// </param>
        /// <param name="getCatalogInventories">
        /// The catalog inventories.
        /// </param>
        /// <param name="getProductOptions">
        /// The product options.
        /// </param>
        /// <param name="getProductVariantCollection">
        /// The product variant collection.
        /// </param>
        /// <param name="getDetachedContentCollection">
        /// Gets the detached content collection
        /// </param>
        public ProductFactory(
            Func<Guid, ProductAttributeCollection> getProductAttributes,
            Func<Guid, CatalogInventoryCollection> getCatalogInventories,
            Func<Guid, ProductOptionCollection> getProductOptions,
            Func<Guid, ProductVariantCollection> getProductVariantCollection,
            Func<Guid, DetachedContentCollection<IProductVariantDetachedContent>> getDetachedContentCollection)
        {
            _productVariantFactory = new ProductVariantFactory(getProductAttributes, getCatalogInventories, getDetachedContentCollection);
            this._getProductOptionCollection = getProductOptions;
            this._getProductVariantCollection = getProductVariantCollection;
        }

        /// <summary>
        /// The build entity.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="IProduct"/>.
        /// </returns>
        public IProduct BuildEntity(ProductDto dto)
        {
            var variant = _productVariantFactory.BuildEntity(dto.ProductVariantDto);
            var product = new Product(variant)
            {
                Key = dto.Key,
                ProductOptions = this._getProductOptionCollection.Invoke(dto.Key),
                ProductVariants = this._getProductVariantCollection.Invoke(dto.Key),
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            // Fix sort order of attributes in each variant.
            // Since no more than one attribute from a single option can be in a variants Attributes list,
            // it is safe to just take the option's sortOrder and use it as the sortOrder of the "variant-attribute".
            foreach (var pvariant in product.ProductVariants)
            {
                pvariant.Attributes.Select(x =>
                {
                    x.SortOrder = product.ProductOptions.First(o => o.Key == x.OptionKey).SortOrder; // This should not return null! Otherwise there was a problem building the entity.
                    return x;
                }).ToList();
            }

            product.ResetDirtyProperties();

            return product;
        }

        /// <summary>
        /// The build dto.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDto"/>.
        /// </returns>
        public ProductDto BuildDto(IProduct entity)
        {
            
            var dto = new ProductDto()
            {
                Key = entity.Key,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate,
                ProductVariantDto = _productVariantFactory.BuildDto(((Product)entity).MasterVariant)
            };

            return dto;
        }

    }
}
