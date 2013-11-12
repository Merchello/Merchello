using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class ProductAttributeFactory : IEntityFactory<IProductAttribute, ProductAttributeDto>
    {
        public IProductAttribute BuildEntity(ProductAttributeDto dto)
        {
            var attribute = new ProductAttribute(dto.Name, dto.Sku)
                {
                    Id = dto.Id,
                    OptionId = dto.OptionId,
                    SortOrder = dto.SortOrder,
                    UpdateDate = dto.UpdateDate,
                    CreateDate = dto.CreateDate
                };


            return attribute;
        }

        public ProductAttributeDto BuildDto(IProductAttribute entity)
        {
            return new ProductAttributeDto()
                {
                    Id = entity.Id,
                    OptionId = entity.OptionId,
                    Name = entity.Name,
                    Sku = entity.Sku,
                    SortOrder = entity.SortOrder,
                    UpdateDate = entity.UpdateDate,
                    CreateDate = entity.CreateDate
                };
        }
    }
}