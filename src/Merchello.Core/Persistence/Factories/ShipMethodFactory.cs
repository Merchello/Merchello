using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class ShipMethodFactory : IEntityFactory<IShipMethod, ShipMethodDto>
    {
        public IShipMethod BuildEntity(ShipMethodDto dto)
        {
            var shipMethod = new ShipMethod()
            {
                Id = dto.Id,
                Name = dto.Name,
                ProviderKey = dto.ProviderKey,
                ShipMethodTypeFieldKey = dto.ShipMethodTfKey,
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
                Id = entity.Id,
                Name = entity.Name,
                ProviderKey = entity.ProviderKey,
                ShipMethodTfKey = entity.ShipMethodTypeFieldKey,
                Surcharge = entity.Surcharge,
                ServiceCode = entity.ServiceCode,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate                
            };

            return dto;
        }
    }
}
