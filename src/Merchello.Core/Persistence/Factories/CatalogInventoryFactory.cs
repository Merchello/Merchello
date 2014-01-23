using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class CatalogInventoryFactory : IEntityFactory<ICatalogInventory, CatalogInventoryDto>
    {
        public ICatalogInventory BuildEntity(CatalogInventoryDto dto)
        {
           
            return new CatalogInventory(dto.CatalogKey, dto.ProductVariantKey)
                {
                    Count = dto.Count,
                    LowCount = dto.LowCount,
                    CreateDate = dto.CreateDate,
                    UpdateDate = dto.UpdateDate
                };           
        }

        public CatalogInventoryDto BuildDto(ICatalogInventory entity)
        {
            return new CatalogInventoryDto()
                {
                    CatalogKey = entity.CatalogKey,
                    ProductVariantKey = entity.ProductVariantKey,
                    Count = entity.Count,
                    LowCount = entity.LowCount,
                    CreateDate = entity.CreateDate,
                    UpdateDate = entity.UpdateDate
                };
        }
    }
}