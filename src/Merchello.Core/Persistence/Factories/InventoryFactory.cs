using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class InventoryFactory : IEntityFactory<IWarehouseInventory, CatalogInventoryDto>
    {
        public IWarehouseInventory BuildEntity(CatalogInventoryDto dto)
        {
            var catalog = new WarehouseCatalogFactory().BuildEntity(dto.WarehouseCatalogDto);

            return new WarehouseInventory(catalog, dto.ProductVariantKey)
                {
                    Count = dto.Count,
                    LowCount = dto.LowCount,
                    CreateDate = dto.CreateDate,
                    UpdateDate = dto.UpdateDate
                };           
        }

        public CatalogInventoryDto BuildDto(IWarehouseInventory entity)
        {
            return new CatalogInventoryDto()
                {
                    CatalogKey = ((WarehouseInventory)entity).CatalogKey,
                    ProductVariantKey = entity.ProductVariantKey,
                    Count = entity.Count,
                    LowCount = entity.LowCount,
                    CreateDate = entity.CreateDate,
                    UpdateDate = entity.UpdateDate
                };
        }
    }
}