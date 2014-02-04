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
            CacheMap<CatalogInventory, CatalogInventoryDto>(src => src.CatalogKey, dto => dto.CatalogKey);
            CacheMap<CatalogInventory, CatalogInventoryDto>(src => src.ProductVariantKey, dto => dto.ProductVariantKey);
            CacheMap<CatalogInventory, CatalogInventoryDto>(src => src.Count, dto => dto.Count);
            CacheMap<CatalogInventory, CatalogInventoryDto>(src => src.LowCount, dto => dto.LowCount);
            CacheMap<CatalogInventory, CatalogInventoryDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<CatalogInventory, CatalogInventoryDto>(src => src.CreateDate, dto => dto.CreateDate);
        } 
    }
}