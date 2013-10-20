using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Mappers;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents a <see cref="WarehouseInventory"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class WarehouseInventoryMapper : MerchelloBaseMapper
    {
        public WarehouseInventoryMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            CacheMap<WarehouseInventory, WarehouseInventoryDto>(src => src.WarehouseId, dto => dto.WarehouseId);
            CacheMap<WarehouseInventory, WarehouseInventoryDto>(src => src.ProductVariantId, dto => dto.ProductVariantId);
            CacheMap<WarehouseInventory, WarehouseInventoryDto>(src => src.Count, dto => dto.Count);
            CacheMap<WarehouseInventory, WarehouseInventoryDto>(src => src.LowCount, dto => dto.LowCount);
            CacheMap<WarehouseInventory, WarehouseInventoryDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<WarehouseInventory, WarehouseInventoryDto>(src => src.CreateDate, dto => dto.CreateDate);
        }
    }
}