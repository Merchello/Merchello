namespace Merchello.Core.Persistence.Mappers
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// Responsible for mapping <see cref="OfferSettings"/> to <see cref="OfferSettingsDto"/>.
    /// </summary>
    internal sealed class OfferSettingsMapper : MerchelloBaseMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferSettingsMapper"/> class.
        /// </summary>
        public OfferSettingsMapper()
        {
            this.BuildMap();
        }

        /// <summary>
        /// Builds the mappings between <see cref="OfferSettings"/> to <see cref="OfferSettingsDto"/>.
        /// </summary>
        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<OfferSettings, OfferSettingsDto>(src => src.Key, dto => dto.Key);
            CacheMap<OfferSettings, OfferSettingsDto>(src => src.Name, dto => dto.Name);
            CacheMap<OfferSettings, OfferSettingsDto>(src => src.OfferCode, dto => dto.OfferCode);            
            CacheMap<OfferSettings, OfferSettingsDto>(src => src.OfferProviderKey, dto => dto.OfferProviderKey);
            CacheMap<OfferSettings, OfferSettingsDto>(src => src.OfferStartsDate, dto => dto.OfferStartsDate);
            CacheMap<OfferSettings, OfferSettingsDto>(src => src.OfferEndsDate, dto => dto.OfferEndsDate);
            CacheMap<OfferSettings, OfferSettingsDto>(src => src.Active, dto => dto.Active);
            CacheMap<OfferSettings, OfferSettingsDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<OfferSettings, OfferSettingsDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}