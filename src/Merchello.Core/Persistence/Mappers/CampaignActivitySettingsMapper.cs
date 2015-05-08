namespace Merchello.Core.Persistence.Mappers
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The campaign activity settings mapper.
    /// </summary>
    internal sealed class CampaignActivitySettingsMapper : MerchelloBaseMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignActivitySettingsMapper"/> class.
        /// </summary>
        public CampaignActivitySettingsMapper()
        {
            this.BuildMap();
        }

        /// <summary>
        /// Maps the <see cref="ICampaignActivitySettings"/> to the <see cref="CampaignActivitySettingsDto"/>.
        /// </summary>
        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<ICampaignActivitySettings, CampaignActivitySettingsDto>(src => src.Key, dto => dto.Key);
            CacheMap<ICampaignActivitySettings, CampaignActivitySettingsDto>(src => src.CampaignKey, dto => dto.CampaignKey);
            CacheMap<ICampaignActivitySettings, CampaignActivitySettingsDto>(src => src.CampaignActivityTfKey, dto => dto.CampaignActivityTfKey);
            CacheMap<ICampaignActivitySettings, CampaignActivitySettingsDto>(src => src.Name, dto => dto.Name);
            CacheMap<ICampaignActivitySettings, CampaignActivitySettingsDto>(src => src.Alias, dto => dto.Code);
            CacheMap<ICampaignActivitySettings, CampaignActivitySettingsDto>(src => src.Description, dto => dto.Description);
            CacheMap<ICampaignActivitySettings, CampaignActivitySettingsDto>(src => src.Active, dto => dto.Active);
            CacheMap<ICampaignActivitySettings, CampaignActivitySettingsDto>(src => src.StartDate, dto => dto.StartDate);
            CacheMap<ICampaignActivitySettings, CampaignActivitySettingsDto>(src => src.EndDate, dto => dto.EndDate);
            CacheMap<ICampaignActivitySettings, CampaignActivitySettingsDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<ICampaignActivitySettings, CampaignActivitySettingsDto>(src => src.CreateDate, dto => dto.CreateDate);
        }
    }
}