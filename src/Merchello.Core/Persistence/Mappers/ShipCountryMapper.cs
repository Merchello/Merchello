using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    internal sealed class ShipCountryMapper : MerchelloBaseMapper
    {
        public ShipCountryMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<ShipCountry, ShipCountryDto>(src => src.Key, dto => dto.Key);
            CacheMap<ShipCountry, ShipCountryDto>(src => src.CatalogKey, dto => dto.CatalogKey);
            CacheMap<ShipCountry, ShipCountryDto>(src => src.CountryCode, dto => dto.CountryCode);
            CacheMap<ShipCountry, ShipCountryDto>(src => src.Name, dto => dto.Name);
            CacheMap<ShipCountry, ShipCountryDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<ShipCountry, ShipCountryDto>(src => src.CreateDate, dto => dto.CreateDate);
        }
    }
}