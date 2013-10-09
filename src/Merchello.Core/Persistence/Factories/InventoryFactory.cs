using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class InventoryFactory : IEntityFactory<IInventory, InventoryDto>
    {
        public IInventory BuildEntity(InventoryDto dto)
        {
            return new Inventory(dto.WarehouseId, dto.ProductVariantKey)
                {
                    Count = dto.Count,
                    LowCount = dto.LowCount,
                    CreateDate = dto.CreateDate,
                    UpdateDate = dto.UpdateDate
                };
        }

        public InventoryDto BuildDto(IInventory entity)
        {
            return new InventoryDto()
                {
                    WarehouseId = entity.WarehouseId,
                    ProductVariantKey = entity.ProductVariantKey,
                    Count = entity.Count,
                    LowCount = entity.LowCount,
                    CreateDate = entity.CreateDate,
                    UpdateDate = entity.UpdateDate
                };
        }
    }
}