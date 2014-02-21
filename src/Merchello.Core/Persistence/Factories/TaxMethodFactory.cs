using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Newtonsoft.Json;

namespace Merchello.Core.Persistence.Factories
{
    internal class TaxMethodFactory : IEntityFactory<ITaxMethod, TaxMethodDto>
    {
        public ITaxMethod BuildEntity(TaxMethodDto dto)
        {
            var deserialized = JsonConvert.DeserializeObject<ProvinceCollection<TaxProvince>>(dto.ProvinceData);
            var provinces = new ProvinceCollection<ITaxProvince>();
            foreach (var p in deserialized)
            {
                provinces.Add(p);
            }

            var countryTaxRate = new TaxMethod(dto.ProviderKey, dto.Code)
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

        public TaxMethodDto BuildDto(ITaxMethod entity)
        {
            var provinceData = JsonConvert.SerializeObject(entity.Provinces);
            return new TaxMethodDto()
                {
                    Key = entity.Key,
                    Code = entity.CountryCode,
                    ProviderKey = entity.ProviderKey,
                    PercentageTaxRate = entity.PercentageTaxRate,
                    ProvinceData = provinceData,
                    UpdateDate = entity.UpdateDate,
                    CreateDate = entity.CreateDate
                };
        }
    }
}