using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    internal sealed class CountryTaxRateMapper : MerchelloBaseMapper
    {
         public CountryTaxRateMapper()
         {
             BuildMap();
         }

         internal override void BuildMap()
         {
             if (!PropertyInfoCache.IsEmpty) return;

             CacheMap<CountryTaxRate, CountryTaxRateDto>(src => src.Key, dto => dto.Key);
             CacheMap<CountryTaxRate, CountryTaxRateDto>(src => src.ProviderKey, dto => dto.ProviderKey);
             CacheMap<CountryTaxRate, CountryTaxRateDto>(src => src.CountryCode, dto => dto.Code);
             CacheMap<CountryTaxRate, CountryTaxRateDto>(src => src.PercentageTaxRate, dto => dto.PercentageTaxRate);
             CacheMap<CountryTaxRate, CountryTaxRateDto>(src => src.UpdateDate, dto => dto.UpdateDate);
             CacheMap<CountryTaxRate, CountryTaxRateDto>(src => src.CreateDate, dto => dto.CreateDate);
         }
    }
}