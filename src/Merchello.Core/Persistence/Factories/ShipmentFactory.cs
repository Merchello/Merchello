using System.Text;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class ShipmentFactory : IEntityFactory<IShipment, ShipmentDto>
    {
        public IShipment BuildEntity(ShipmentDto dto)
        {
            var shipment = new Shipment()
            {
                Key = dto.Key,
                FromName = dto.FromName,
                FromAddress1 = dto.FromAddress1,
                FromAddress2 = dto.FromAddress2,
                FromLocality = dto.FromLocality,
                FromRegion = dto.FromRegion,
                FromPostalCode = dto.FromPostalCode,
                FromCountryCode = dto.FromCountryCode,
                ToName = dto.ToName,
                ToAddress1 = dto.ToAddress1,
                ToAddress2 = dto.ToAddress2,
                ToLocality = dto.ToLocality,
                ToRegion = dto.ToRegion,
                ToPostalCode = dto.ToPostalCode,
                ToCountryCode = dto.ToCountryCode,
                Phone = dto.Phone,
                ShipMethodKey = dto.ShipMethodKey,       
                InvoiceItemKey = dto.InvoiceItemKey,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            shipment.ResetDirtyProperties();

            return shipment;
        }

        public ShipmentDto BuildDto(IShipment entity)
        {
            var dto = new ShipmentDto()
            {
                Key = entity.Key,
                FromName = entity.FromName,
                FromAddress1 = entity.FromAddress1,
                FromAddress2 = entity.FromAddress2,
                FromLocality = entity.FromLocality,
                FromRegion = entity.FromRegion,
                FromPostalCode = entity.FromPostalCode,
                FromCountryCode = entity.FromCountryCode,
                ToName = entity.ToName,
                ToAddress1 = entity.ToAddress1,
                ToAddress2 = entity.ToAddress2,
                ToLocality = entity.ToLocality,
                ToRegion = entity.ToRegion,
                ToPostalCode = entity.ToPostalCode,
                ToCountryCode = entity.ToCountryCode,                
                ShipMethodKey = entity.ShipMethodKey,
                InvoiceItemKey = entity.InvoiceItemKey,
                Phone = entity.Phone,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate                
            };

            return dto;
        }
    }
}
