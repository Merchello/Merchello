using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="Shipment"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class ShipmentMapper : MerchelloBaseMapper
    {
        public ShipmentMapper()
        {
            BuildMap();
        }

        #region Overrides of MerchelloBaseMapper

        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<Shipment, ShipmentDto>(src => src.Key, dto => dto.Key);
            CacheMap<Shipment, ShipmentDto>(src => src.ShippedDate, dto => dto.ShippedDate);
            CacheMap<Shipment, ShipmentDto>(src => src.FromOrganization, dto => dto.FromOrganization);
            CacheMap<Shipment, ShipmentDto>(src => src.FromName, dto => dto.FromName);
            CacheMap<Shipment, ShipmentDto>(src => src.FromAddress1, dto => dto.FromAddress1);
            CacheMap<Shipment, ShipmentDto>(src => src.FromAddress2, dto => dto.FromAddress2);
            CacheMap<Shipment, ShipmentDto>(src => src.FromLocality, dto => dto.FromLocality);
            CacheMap<Shipment, ShipmentDto>(src => src.FromRegion, dto => dto.FromRegion);
            CacheMap<Shipment, ShipmentDto>(src => src.FromPostalCode, dto => dto.FromPostalCode);
            CacheMap<Shipment, ShipmentDto>(src => src.FromCountryCode, dto => dto.FromCountryCode);
            CacheMap<Shipment, ShipmentDto>(src => src.FromIsCommercial, dto => dto.FromIsCommercial);
            CacheMap<Shipment, ShipmentDto>(src => src.ToOrganization, dto => dto.ToOrganization);
            CacheMap<Shipment, ShipmentDto>(src => src.ToName, dto => dto.ToName);
            CacheMap<Shipment, ShipmentDto>(src => src.ToAddress1, dto => dto.ToAddress1);
            CacheMap<Shipment, ShipmentDto>(src => src.ToAddress2, dto => dto.ToAddress2);
            CacheMap<Shipment, ShipmentDto>(src => src.ToLocality, dto => dto.ToLocality);
            CacheMap<Shipment, ShipmentDto>(src => src.ToRegion, dto => dto.ToRegion);
            CacheMap<Shipment, ShipmentDto>(src => src.ToPostalCode, dto => dto.ToPostalCode);
            CacheMap<Shipment, ShipmentDto>(src => src.ToCountryCode, dto => dto.ToCountryCode);
            CacheMap<Shipment, ShipmentDto>(src => src.ToIsCommercial, dto => dto.ToIsCommercial);
            CacheMap<Shipment, ShipmentDto>(src => src.ShipMethodKey, dto => dto.ShipMethodKey);
            CacheMap<Shipment, ShipmentDto>(src => src.Phone, dto => dto.Phone);
            CacheMap<Shipment, ShipmentDto>(src => src.Email, dto => dto.Email);
            CacheMap<Shipment, ShipmentDto>(src => src.Email, dto => dto.Email);
            CacheMap<Shipment, ShipmentDto>(src => src.Carrier, dto => dto.Carrier);
            CacheMap<Shipment, ShipmentDto>(src => src.TrackingCode, dto => dto.TrackingCode);
            CacheMap<Shipment, ShipmentDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<Shipment, ShipmentDto>(src => src.CreateDate, dto => dto.CreateDate);
        }

        #endregion
    }
}