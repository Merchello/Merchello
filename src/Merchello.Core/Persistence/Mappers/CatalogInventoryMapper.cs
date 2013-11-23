using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    internal sealed class CatalogInventoryMapper : MerchelloBaseMapper
    {
        public CatalogInventoryMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            CacheMap<WarehouseInventory, CatalogInventoryDto>(src => src.CatalogKey, dto => dto.CatalogKey);
            CacheMap<WarehouseInventory, CatalogInventoryDto>(src => src.ProductVariantKey, dto => dto.ProductVariantKey);
            CacheMap<WarehouseInventory, CatalogInventoryDto>(src => src.Count, dto => dto.Count);
            CacheMap<WarehouseInventory, CatalogInventoryDto>(src => src.LowCount, dto => dto.LowCount);
            CacheMap<WarehouseInventory, CatalogInventoryDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<WarehouseInventory, CatalogInventoryDto>(src => src.CreateDate, dto => dto.CreateDate);
        } 
    }
}