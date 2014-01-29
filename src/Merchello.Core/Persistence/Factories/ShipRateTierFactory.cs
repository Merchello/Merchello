using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class ShipRateTierFactory : IEntityFactory<IShipRateTier, ShipRateTierDto>
    {
        public IShipRateTier BuildEntity(ShipRateTierDto dto)
        {
            var entity = new ShipRateTier(dto.ShipMethodKey)
                {
                    Key = dto.Key,
                    RangeLow = dto.RangeLow,
                    RangeHigh = dto.RangeHigh,
                    Rate = dto.Rate,
                    UpdateDate = dto.UpdateDate,
                    CreateDate = dto.CreateDate
                };

            entity.ResetDirtyProperties();

            return entity;
        }

        public ShipRateTierDto BuildDto(IShipRateTier entity)
        {
            return new ShipRateTierDto()
                {
                    Key = entity.Key,
                    ShipMethodKey = entity.ShipMethodKey,
                    RangeLow = entity.RangeLow,
                    RangeHigh = entity.RangeHigh,
                    Rate = entity.Rate,
                    UpdateDate = entity.UpdateDate,
                    CreateDate = entity.CreateDate
                };
        }
    }
}