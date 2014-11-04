namespace Merchello.Core.Persistence.Mappers
{
    using Models;
    using Models.Rdbms;

    /// <summary>
    /// Represents a <see cref="InvoiceStatus"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class ShipmentStatusMapper : MerchelloBaseMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShipmentStatusMapper"/> class.
        /// </summary>
        public ShipmentStatusMapper()
        {
            BuildMap();
        }

        /// <summary>
        /// Builds the cache map
        /// </summary>
        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<ShipmentStatus, ShipmentStatusDto>(src => src.Key, dto => dto.Key);
            CacheMap<ShipmentStatus, ShipmentStatusDto>(src => src.Name, dto => dto.Name);
            CacheMap<ShipmentStatus, ShipmentStatusDto>(src => src.Alias, dto => dto.Alias);
            CacheMap<ShipmentStatus, ShipmentStatusDto>(src => src.Reportable, dto => dto.Reportable);
            CacheMap<ShipmentStatus, ShipmentStatusDto>(src => src.Active, dto => dto.Active);
            CacheMap<ShipmentStatus, ShipmentStatusDto>(src => src.SortOrder, dto => dto.SortOrder);
            CacheMap<ShipmentStatus, ShipmentStatusDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<ShipmentStatus, ShipmentStatusDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}
