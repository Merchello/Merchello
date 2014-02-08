using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    internal  sealed class ShipRateTierMapper : MerchelloBaseMapper
    {
        public ShipRateTierMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<ShipRateTier, ShipRateTierDto>(src => src.Key, dto => dto.Key);
            CacheMap<ShipRateTier, ShipRateTierDto>(src => src.ShipMethodKey, dto => dto.ShipMethodKey);
            CacheMap<ShipRateTier, ShipRateTierDto>(src => src.RangeLow, dto => dto.RangeLow);
            CacheMap<ShipRateTier, ShipRateTierDto>(src => src.RangeHigh, dto => dto.RangeHigh);
            CacheMap<ShipRateTier, ShipRateTierDto>(src => src.Rate, dto => dto.Rate);
            CacheMap<ShipRateTier, ShipRateTierDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<ShipRateTier, ShipRateTierDto>(src => src.CreateDate, dto => dto.CreateDate);
        }
    }
}