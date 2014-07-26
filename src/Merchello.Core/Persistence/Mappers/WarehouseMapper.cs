namespace Merchello.Core.Persistence.Mappers
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// Represents a <see cref="Warehouse"/> to DTO mapper used to translate the properties of the public API 
    /// implementation to that of the database's DTO as SQL: [tableName].[columnName].
    /// </summary>
    internal sealed class WarehouseMapper : MerchelloBaseMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WarehouseMapper"/> class.
        /// </summary>
        public WarehouseMapper()
        {
            BuildMap();
        }

        /// <summary>
        /// Maps a warehouse to the warehouse DTO.
        /// </summary>
        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<Warehouse, WarehouseDto>(src => src.Key, dto => dto.Key);
            CacheMap<Warehouse, WarehouseDto>(src => src.Name, dto => dto.Name);
            CacheMap<Warehouse, WarehouseDto>(src => src.Address1, dto => dto.Address1);
            CacheMap<Warehouse, WarehouseDto>(src => src.Address2, dto => dto.Address2);
            CacheMap<Warehouse, WarehouseDto>(src => src.Locality, dto => dto.Locality);
            CacheMap<Warehouse, WarehouseDto>(src => src.Region, dto => dto.Region);
            CacheMap<Warehouse, WarehouseDto>(src => src.PostalCode, dto => dto.PostalCode);
            CacheMap<Warehouse, WarehouseDto>(src => src.CountryCode, dto => dto.CountryCode);
            CacheMap<Warehouse, WarehouseDto>(src => src.Phone, dto => dto.Phone);
            CacheMap<Warehouse, WarehouseDto>(src => src.Email, dto => dto.Email);
            CacheMap<Warehouse, WarehouseDto>(src => src.IsDefault, dto => dto.IsDefault);
            CacheMap<Warehouse, WarehouseDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<Warehouse, WarehouseDto>(src => src.CreateDate, dto => dto.CreateDate);
        }
    }
}