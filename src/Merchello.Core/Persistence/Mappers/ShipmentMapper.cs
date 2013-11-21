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
            CacheMap<Shipment, ShipmentDto>(src => src.Key, dto => dto.Key);
            CacheMap<Shipment, ShipmentDto>(src => src.Address1, dto => dto.Address1);
            CacheMap<Shipment, ShipmentDto>(src => src.Address2, dto => dto.Address2);
            CacheMap<Shipment, ShipmentDto>(src => src.Locality, dto => dto.Locality);
            CacheMap<Shipment, ShipmentDto>(src => src.Region, dto => dto.Region);
            CacheMap<Shipment, ShipmentDto>(src => src.PostalCode, dto => dto.PostalCode);
            CacheMap<Shipment, ShipmentDto>(src => src.CountryCode, dto => dto.CountryCode);
            CacheMap<Shipment, ShipmentDto>(src => src.ShipMethodKey, dto => dto.ShipMethodKey);
            CacheMap<Shipment, ShipmentDto>(src => src.InvoiceItemKey, dto => dto.InvoiceItemKey);
            CacheMap<Shipment, ShipmentDto>(src => src.Phone, dto => dto.Phone);
            CacheMap<Shipment, ShipmentDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<Shipment, ShipmentDto>(src => src.CreateDate, dto => dto.CreateDate);
        }

        #endregion
    }
}