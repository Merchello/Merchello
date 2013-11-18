using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;


namespace Merchello.Core.Persistence.Factories
{
    internal class ProductOptionFactory : IEntityFactory<IProductOption, ProductOptionDto>
    {
        public IProductOption BuildEntity(ProductOptionDto dto)
        {
            var option = new ProductOption(dto.Name, dto.Required)
                {
                    Key = dto.Key,
                    SortOrder = dto.Product2ProductOptionDto.SortOrder,
                    UpdateDate = dto.UpdateDate,
                    CreateDate = dto.CreateDate
                };


            return option;
        }

        public ProductOptionDto BuildDto(IProductOption entity)
        {
            return new ProductOptionDto()
                {
                    Key = entity.Key,
                    Name = entity.Name,
                    Required = entity.Required,
                    CreateDate = entity.CreateDate,
                    UpdateDate = entity.UpdateDate
                };
        }
    }
}