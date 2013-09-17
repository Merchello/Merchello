using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class ShipmentFactory : IEntityFactory<IShipment, ShipmentDto>
    {
        public IShipment BuildEntity(ShipmentDto dto)
        {
            var shipment = new Shipment(dto.InvoiceId)
            {
                Id = dto.Id,
                Address1 = dto.Address1,
                Address2 = dto.Address2,
                Locality = dto.Locality,
                Region = dto.Region,
                PostalCode = dto.PostalCode,
                CountryCode = dto.CountryCode,
                Phone = dto.Phone,
                ShipMethodId = dto.ShipMethodId,
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
                Id = entity.Id,
                InvoiceId = entity.InvoiceId,
                Address1 = entity.Address1,
                Address2 = entity.Address2,
                Locality = entity.Locality,
                Region = entity.Region,
                PostalCode = entity.PostalCode,
                CountryCode = entity.CountryCode,
                ShipMethodId = entity.ShipMethodId,
                Phone = entity.Phone,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate                
            };

            return dto;
        }
    }
}
