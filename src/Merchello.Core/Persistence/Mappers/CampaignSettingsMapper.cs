namespace Merchello.Core.Persistence.Mappers
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The campaign settings mapper.
    /// </summary>
    internal sealed class CampaignSettingsMapper : MerchelloBaseMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignSettingsMapper"/> class.
        /// </summary>
        public CampaignSettingsMapper()
        {
            this.BuildMap();
        }

        /// <summary>
        /// Maps the <see cref="ICampaignSettings"/> to the <see cref="CampaignSettingsDto"/>.
        /// </summary>
        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<ICampaignSettings, CampaignSettingsDto>(src => src.Key, dto => dto.Key);
            CacheMap<ICampaignSettings, CampaignSettingsDto>(src => src.Name, dto => dto.Name);
            CacheMap<ICampaignSettings, CampaignSettingsDto>(src => src.Alias, dto => dto.Alias);
            CacheMap<ICampaignSettings, CampaignSettingsDto>(src => src.Description, dto => dto.Description);
            CacheMap<ICampaignSettings, CampaignSettingsDto>(src => src.Active, dto => dto.Active);
            CacheMap<ICampaignSettings, CampaignSettingsDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<ICampaignSettings, CampaignSettingsDto>(src => src.CreateDate, dto => dto.CreateDate);
        }
    }
}