namespace Merchello.Core.Persistence.Mappers
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The warehouse catalog mapper.
    /// </summary>
    internal sealed class WarehouseCatalogMapper : MerchelloBaseMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WarehouseCatalogMapper"/> class.
        /// </summary>
        public WarehouseCatalogMapper()
        {
            BuildMap();
        }

        /// <summary>
        /// Builds the field mapping cache between warehouse catalog and the warehouse catalog DTO.
        /// </summary>
        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<WarehouseCatalog, WarehouseCatalogDto>(src => src.Key, dto => dto.Key);
            CacheMap<WarehouseCatalog, WarehouseCatalogDto>(src => src.WarehouseKey, dto => dto.WarehouseKey);
            CacheMap<WarehouseCatalog, WarehouseCatalogDto>(src => src.Name, dto => dto.Name);
            CacheMap<WarehouseCatalog, WarehouseCatalogDto>(src => src.Description, dto => dto.Description);
            CacheMap<WarehouseCatalog, WarehouseCatalogDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<WarehouseCatalog, WarehouseCatalogDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}