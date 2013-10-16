using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class ProductFactory : IEntityFactory<IProduct, ProductDto>
    {

        private readonly ProductVariantFactory _productVariantFactory;

        public ProductFactory()
        {
            _productVariantFactory = new ProductVariantFactory();
        }

        public IProduct BuildEntity(ProductDto dto)
        {
            var variant = _productVariantFactory.BuildEntity(dto.ProductVariantDto);
            var product = new Product(variant)
            {
                Key = dto.Key,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            product.ResetDirtyProperties();

            return product;
        }

        public ProductDto BuildDto(IProduct entity)
        {
            
            var dto = new ProductDto()
            {
                Key = entity.Key,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate,
                ProductVariantDto = _productVariantFactory.BuildDto(((Product)entity).DefaultVariant)
            };

            return dto;
        }

    }
}
