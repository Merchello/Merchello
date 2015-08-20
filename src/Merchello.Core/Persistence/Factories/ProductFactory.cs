namespace Merchello.Core.Persistence.Factories
{
    using Merchello.Core.Models;
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
        private readonly ProductOptionCollection _productOptionCollection;

        /// <summary>
        /// The product variant collection.
        /// </summary>
        private readonly ProductVariantCollection _productVariantCollection;


        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFactory"/> class.
        /// </summary>
        public ProductFactory()
            : this(
                new ProductAttributeCollection(),
                new CatalogInventoryCollection(),
                new ProductOptionCollection(),
                new ProductVariantCollection())
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFactory"/> class.
        /// </summary>
        /// <param name="productAttributes">
        /// The product attributes.
        /// </param>
        /// <param name="catalogInventories">
        /// The catalog inventories.
        /// </param>
        /// <param name="productOptions">
        /// The product options.
        /// </param>
        /// <param name="productVariantCollection">
        /// The product variant collection.
        /// </param>
        public ProductFactory(
            ProductAttributeCollection productAttributes,
            CatalogInventoryCollection catalogInventories,
            ProductOptionCollection productOptions,
            ProductVariantCollection productVariantCollection)
        {
            _productVariantFactory = new ProductVariantFactory(productAttributes, catalogInventories);
            _productOptionCollection = productOptions;
            _productVariantCollection = productVariantCollection;
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
                ProductOptions = _productOptionCollection,
                ProductVariants = _productVariantCollection,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

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
