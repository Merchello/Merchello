using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class InventoryFactory : IEntityFactory<IWarehouseInventory, WarehouseInventoryDto>
    {
        public IWarehouseInventory BuildEntity(WarehouseInventoryDto dto)
        {
            return new WarehouseInventory(dto.WarehouseId, dto.ProductVariantId)
                {
                    Count = dto.Count,
                    LowCount = dto.LowCount,
                    CreateDate = dto.CreateDate,
                    UpdateDate = dto.UpdateDate
                };
        }

        public WarehouseInventoryDto BuildDto(IWarehouseInventory entity)
        {
            return new WarehouseInventoryDto()
                {
                    WarehouseId = entity.WarehouseId,
                    ProductVariantId = entity.ProductVariantId,
                    Count = entity.Count,
                    LowCount = entity.LowCount,
                    CreateDate = entity.CreateDate,
                    UpdateDate = entity.UpdateDate
                };
        }
    }
}