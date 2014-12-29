namespace Merchello.Core.Persistence.Mappers
{
    using Models;
    using Models.Rdbms;

    /// <summary>
    /// The catalog inventory mapper.
    /// </summary>
    internal sealed class CatalogInventoryMapper : MerchelloBaseMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogInventoryMapper"/> class.
        /// </summary>
        public CatalogInventoryMapper()
        {
            BuildMap();
        }

        /// <summary>
        /// Maps <see cref="CatalogInventory"/> properties to <see cref="CatalogInventoryDto"/> fields.
        /// </summary>
        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<CatalogInventory, CatalogInventoryDto>(src => src.CatalogKey, dto => dto.CatalogKey);
            CacheMap<CatalogInventory, CatalogInventoryDto>(src => src.ProductVariantKey, dto => dto.ProductVariantKey);
            CacheMap<CatalogInventory, CatalogInventoryDto>(src => src.Count, dto => dto.Count);
            CacheMap<CatalogInventory, CatalogInventoryDto>(src => src.LowCount, dto => dto.LowCount);
            CacheMap<CatalogInventory, CatalogInventoryDto>(src => src.Location, dto => dto.Location);
            CacheMap<CatalogInventory, CatalogInventoryDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<CatalogInventory, CatalogInventoryDto>(src => src.CreateDate, dto => dto.CreateDate);
        } 
    }
}