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
                FromOrganization = dto.FromOrganization,
                FromName = dto.FromName,
                FromAddress1 = dto.FromAddress1,
                FromAddress2 = dto.FromAddress2,
                FromLocality = dto.FromLocality,
                FromRegion = dto.FromRegion,
                FromPostalCode = dto.FromPostalCode,
                FromCountryCode = dto.FromCountryCode,
                FromIsCommercial = dto.FromIsCommercial,
                ToOrganization = dto.ToOrganization,
                ToName = dto.ToName,
                ToAddress1 = dto.ToAddress1,
                ToAddress2 = dto.ToAddress2,
                ToLocality = dto.ToLocality,
                ToRegion = dto.ToRegion,
                ToPostalCode = dto.ToPostalCode,
                ToCountryCode = dto.ToCountryCode,
                ToIsCommercial = dto.ToIsCommercial,
                Phone = dto.Phone,
                Email = dto.Email,
                ShipMethodKey = dto.ShipMethodKey, 
                VersionKey = dto.VersionKey,
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
                FromOrganization = entity.FromOrganization,
                FromName = entity.FromName,
                FromAddress1 = entity.FromAddress1,
                FromAddress2 = entity.FromAddress2,
                FromLocality = entity.FromLocality,
                FromRegion = entity.FromRegion,
                FromPostalCode = entity.FromPostalCode,
                FromCountryCode = entity.FromCountryCode,
                FromIsCommercial = entity.FromIsCommercial,
                ToOrganization = entity.ToOrganization,
                ToName = entity.ToName,
                ToAddress1 = entity.ToAddress1,
                ToAddress2 = entity.ToAddress2,
                ToLocality = entity.ToLocality,
                ToRegion = entity.ToRegion,
                ToPostalCode = entity.ToPostalCode,
                ToCountryCode = entity.ToCountryCode,
                ToIsCommercial = entity.ToIsCommercial,
                ShipMethodKey = entity.ShipMethodKey,
                VersionKey = entity.VersionKey,
                Phone = entity.Phone,
                Email = entity.Email,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate                
            };

            return dto;
        }
    }
}
