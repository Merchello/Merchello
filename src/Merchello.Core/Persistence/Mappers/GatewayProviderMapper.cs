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

            CacheMap<GatewayProviderSetting, GatewayProviderSettingDto>(src => src.Key, dto => dto.Key);
            CacheMap<GatewayProviderSetting, GatewayProviderSettingDto>(src => src.ProviderTfKey, dto => dto.ProviderTfKey);
            CacheMap<GatewayProviderSetting, GatewayProviderSettingDto>(src => src.Name, dto => dto.Name);
            CacheMap<GatewayProviderSetting, GatewayProviderSettingDto>(src => src.Description, dto => dto.Description);
            CacheMap<GatewayProviderSetting, GatewayProviderSettingDto>(src => src.EncryptExtendedData, dto => dto.EncryptExtendedData);
            CacheMap<GatewayProviderSetting, GatewayProviderSettingDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<GatewayProviderSetting, GatewayProviderSettingDto>(src => src.CreateDate, dto => dto.CreateDate);
        }
    }
}