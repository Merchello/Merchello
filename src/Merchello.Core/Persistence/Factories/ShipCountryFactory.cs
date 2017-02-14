using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Services;

namespace Merchello.Core.Persistence.Factories
{
    internal class ShipCountryFactory : IEntityFactory<IShipCountry, ShipCountryDto>
    {
        private readonly IStoreSettingService _storeSettingService;

        public ShipCountryFactory(IStoreSettingService storeSettingService)
        {
            _storeSettingService = storeSettingService;
        }

        public IShipCountry BuildEntity(ShipCountryDto dto)
        {

            var country = dto.CountryCode.Equals(Constants.CountryCodes.EverywhereElse) ?
                new Country(Constants.CountryCodes.EverywhereElse, "Everywhere Else", new List<IProvince>()) : 
                _storeSettingService.GetCountryByCode(dto.CountryCode);

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