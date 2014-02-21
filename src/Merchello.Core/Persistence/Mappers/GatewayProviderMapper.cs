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

            CacheMap<GatewayProvider, GatewayProviderDto>(src => src.Key, dto => dto.Key);
            CacheMap<GatewayProvider, GatewayProviderDto>(src => src.ProviderTfKey, dto => dto.ProviderTfKey);
            CacheMap<GatewayProvider, GatewayProviderDto>(src => src.Name, dto => dto.Name);
            CacheMap<GatewayProvider, GatewayProviderDto>(src => src.Description, dto => dto.Description);
            CacheMap<GatewayProvider, GatewayProviderDto>(src => src.EncryptExtendedData, dto => dto.EncryptExtendedData);
            CacheMap<GatewayProvider, GatewayProviderDto>(src => src.UpdateDate, dto => dto.UpdateDate);
            CacheMap<GatewayProvider, GatewayProviderDto>(src => src.CreateDate, dto => dto.CreateDate);
        }
    }
}