using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Mappers;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Represents a <see cref="Inventory"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class InventoryMapper : MerchelloBaseMapper
    {
        public InventoryMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            CacheMap<Inventory, InventoryDto>(src => src.WarehouseId, dto => dto.WarehouseId);
            CacheMap<Inventory, InventoryDto>(src => src.ProductVariantKey, dto => dto.ProductVariantKey);
            CacheMap<Inventory, InventoryDto>(src => src.Count, dto => dto.Count);
            CacheMap<Inventory, InventoryDto>(src => src.LowCount, dto => dto.LowCount);
            CacheMap<Inventory, InventoryDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<Inventory, InventoryDto>(src => src.CreateDate, dto => dto.CreateDate);
        }
    }
}