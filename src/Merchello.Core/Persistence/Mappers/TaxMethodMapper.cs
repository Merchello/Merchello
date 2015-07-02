namespace Merchello.Core.Persistence.Mappers
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The tax method mapper.
    /// </summary>
    internal sealed class TaxMethodMapper : MerchelloBaseMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaxMethodMapper"/> class.
        /// </summary>
        public TaxMethodMapper()
         {
             BuildMap();
         }

        /// <summary>
        /// The build map.
        /// </summary>
        internal override void BuildMap()
         {
             if (!PropertyInfoCache.IsEmpty) return;

             CacheMap<TaxMethod, TaxMethodDto>(src => src.Key, dto => dto.Key);
             CacheMap<TaxMethod, TaxMethodDto>(src => src.ProviderKey, dto => dto.ProviderKey);
             CacheMap<TaxMethod, TaxMethodDto>(src => src.Name, dto => dto.Name);
             CacheMap<TaxMethod, TaxMethodDto>(src => src.CountryCode, dto => dto.CountryCode);
             CacheMap<TaxMethod, TaxMethodDto>(src => src.PercentageTaxRate, dto => dto.PercentageTaxRate);
             CacheMap<TaxMethod, TaxMethodDto>(src => src.ProductTaxMethod, dto => dto.ProductTaxMethod);
             CacheMap<TaxMethod, TaxMethodDto>(src => src.UpdateDate, dto => dto.UpdateDate);
             CacheMap<TaxMethod, TaxMethodDto>(src => src.CreateDate, dto => dto.CreateDate);
         }
    }
}