using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class ShipMethodFactory : IEntityFactory<IShipMethod, ShipMethodDto>
    {
        public IShipMethod BuildEntity(ShipMethodDto dto)
        {
            var shipMethod = new ShipMethod(dto.ProviderKey, dto.ShipCountryKey)
            {
                Key = dto.Key,
                Name = dto.Name,              
                Surcharge = dto.Surcharge,
                ServiceCode = dto.ServiceCode,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            shipMethod.ResetDirtyProperties();

            return shipMethod;
        }

        public ShipMethodDto BuildDto(IShipMethod entity)
        {
            var dto = new ShipMethodDto()
            {
                Key = entity.Key,
                Name = entity.Name,
                ProviderKey = entity.ProviderKey,
                ShipCountryKey = entity.ShipCountryKey,
                Surcharge = entity.Surcharge,
                ServiceCode = entity.ServiceCode,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate                
            };

            return dto;
        }
    }
}
