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
                GatewayAlias = dto.GatewayAlias,
                ShipMethodTypeFieldKey = dto.ShipMethodTypeFieldKey,
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
                GatewayAlias = entity.GatewayAlias,
                ShipMethodTypeFieldKey = entity.ShipMethodTypeFieldKey,
                Surcharge = entity.Surcharge,
                ServiceCode = entity.ServiceCode,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate                
            };

            return dto;
        }
    }
}
