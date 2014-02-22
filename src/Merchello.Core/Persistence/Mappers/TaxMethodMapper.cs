using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    internal sealed class TaxMethodMapper : MerchelloBaseMapper
    {
         public TaxMethodMapper()
         {
             BuildMap();
         }

         internal override void BuildMap()
         {
             if (!PropertyInfoCache.IsEmpty) return;

             CacheMap<TaxMethod, TaxMethodDto>(src => src.Key, dto => dto.Key);
             CacheMap<TaxMethod, TaxMethodDto>(src => src.ProviderKey, dto => dto.ProviderKey);
             CacheMap<TaxMethod, TaxMethodDto>(src => src.Name, dto => dto.Name);
             CacheMap<TaxMethod, TaxMethodDto>(src => src.CountryCode, dto => dto.Code);
             CacheMap<TaxMethod, TaxMethodDto>(src => src.PercentageTaxRate, dto => dto.PercentageTaxRate);
             CacheMap<TaxMethod, TaxMethodDto>(src => src.UpdateDate, dto => dto.UpdateDate);
             CacheMap<TaxMethod, TaxMethodDto>(src => src.CreateDate, dto => dto.CreateDate);
         }
    }
}