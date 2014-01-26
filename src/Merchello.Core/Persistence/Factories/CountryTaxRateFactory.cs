using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Newtonsoft.Json;

namespace Merchello.Core.Persistence.Factories
{
    internal class CountryTaxRateFactory : IEntityFactory<ICountryTaxRate, CountryTaxRateDto>
    {
        public ICountryTaxRate BuildEntity(CountryTaxRateDto dto)
        {
            var deserialized = JsonConvert.DeserializeObject<ProvinceCollection<TaxProvince>>(dto.ProvinceData);
            var provinces = new ProvinceCollection<ITaxProvince>();
            foreach (var p in deserialized)
            {
                provinces.Add(p);
            }

            var countryTaxRate = new CountryTaxRate(dto.ProviderKey, dto.CountryCode)
            {
                Key = dto.Key,
                PercentageTaxRate = dto.PercentageTaxRate,
                Provinces = provinces,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            countryTaxRate.ResetDirtyProperties();

            return countryTaxRate;
        }

        public CountryTaxRateDto BuildDto(ICountryTaxRate entity)
        {
            var provinceData = JsonConvert.SerializeObject(entity.Provinces);
            return new CountryTaxRateDto()
                {
                    Key = entity.Key,
                    CountryCode = entity.CountryCode,
                    ProviderKey = entity.ProviderKey,
                    PercentageTaxRate = entity.PercentageTaxRate,
                    ProvinceData = provinceData,
                    UpdateDate = entity.UpdateDate,
                    CreateDate = entity.CreateDate
                };
        }
    }
}