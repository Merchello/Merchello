namespace Merchello.Core.Persistence.Factories
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    using Newtonsoft.Json;

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

            var countryTaxRate = new TaxMethod(dto.ProviderKey, dto.CountryCode)
            {
                Key = dto.Key,
                Name = dto.Name,
                PercentageTaxRate = dto.PercentageTaxRate,
                Provinces = provinces,
                ProductTaxMethod = dto.ProductTaxMethod,
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
                    Name = entity.Name,
                    CountryCode = entity.CountryCode,
                    ProviderKey = entity.ProviderKey,
                    PercentageTaxRate = entity.PercentageTaxRate,
                    ProvinceData = provinceData,
                    ProductTaxMethod = entity.ProductTaxMethod,
                    UpdateDate = entity.UpdateDate,
                    CreateDate = entity.CreateDate
                };
        }
    }
}