using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="GatewayProviderBase"/> to DTO mapper used to translate the properties of the public api 
    /// implementation to that of the database's DTO as sql: [tableName].[columnName].
    /// </summary>
    internal sealed class RegisteredGatewayProviderMapper : MerchelloBaseMapper
    {
        public RegisteredGatewayProviderMapper()
        {
            BuildMap();
        }

        internal override void BuildMap()
        {
            CacheMap<GatewayProvider, GatewayProviderDto>(src => src.Key, dto => dto.Key);
            CacheMap<GatewayProvider, GatewayProviderDto>(src => src.GatewayProviderTfKey, dto => dto.GatewayProviderTfKey);
            CacheMap<GatewayProvider, GatewayProviderDto>(src => src.Name, dto => dto.Name);
            CacheMap<GatewayProvider, GatewayProviderDto>(src => src.TypeFullName, dto => dto.TypeFullName);
            CacheMap<GatewayProvider, GatewayProviderDto>(src => src.ConfigurationData, dto => dto.ExtendedData);
            CacheMap<GatewayProvider, GatewayProviderDto>(src => src.EncryptConfigurationData, dto => dto.EncryptExtendedData);
            CacheMap<GatewayProvider, GatewayProviderDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<GatewayProvider, GatewayProviderDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}