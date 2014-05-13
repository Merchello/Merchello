using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    internal sealed class GatewayProviderMapper : MerchelloBaseMapper
    {
        public GatewayProviderMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            if (!PropertyInfoCache.IsEmpty) return;

            CacheMap<GatewayProviderSettings, GatewayProviderSettingsDto>(src => src.Key, dto => dto.Key);
            CacheMap<GatewayProviderSettings, GatewayProviderSettingsDto>(src => src.ProviderTfKey, dto => dto.ProviderTfKey);
            CacheMap<GatewayProviderSettings, GatewayProviderSettingsDto>(src => src.Name, dto => dto.Name);
            CacheMap<GatewayProviderSettings, GatewayProviderSettingsDto>(src => src.Description, dto => dto.Description);
            CacheMap<GatewayProviderSettings, GatewayProviderSettingsDto>(src => src.EncryptExtendedData, dto => dto.EncryptExtendedData);
            CacheMap<GatewayProviderSettings, GatewayProviderSettingsDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<GatewayProviderSettings, GatewayProviderSettingsDto>(src => src.CreateDate, dto => dto.CreateDate);
        }
    }
}