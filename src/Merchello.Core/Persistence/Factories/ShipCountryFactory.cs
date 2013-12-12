using System;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Services;

namespace Merchello.Core.Persistence.Factories
{
    internal class ShipCountryFactory : IEntityFactory<IShipCountry, ShipCountryDto>
    {
        private readonly ISettingsService _settingsService;

        public ShipCountryFactory(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public IShipCountry BuildEntity(ShipCountryDto dto)
        {
            var country = _settingsService.GetCountryByCode(dto.CountryCode);

            var shipCountry = new ShipCountry(dto.CatalogKey, country)
            {
                Key = dto.Key,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            shipCountry.ResetDirtyProperties();

            return shipCountry;
        }

        public ShipCountryDto BuildDto(IShipCountry entity)
        {
            return new ShipCountryDto()
            {
                Key = entity.Key,
                CatalogKey = entity.CatalogKey,
                CountryCode = entity.CountryCode,
                Name = entity.Name,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

        }
    }
}