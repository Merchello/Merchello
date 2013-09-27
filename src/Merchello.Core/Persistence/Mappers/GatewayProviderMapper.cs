using Merchello.Core.Models;
using Merchello.Core.Models.GatewayProviders;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Mappers
{
    /// <summary>
    /// Represents a <see cref="RegisteredGatewayProviderBase"/> to DTO mapper used to translate the properties of the public api 
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
            CacheMap<RegisteredGatewayProvider, RegisteredGatewayProviderDto>(src => src.Key, dto => dto.Key);
            CacheMap<RegisteredGatewayProvider, RegisteredGatewayProviderDto>(src => src.GatewayProviderTypeFieldKey, dto => dto.GatewayProviderTypeFieldKey);
            CacheMap<RegisteredGatewayProvider, RegisteredGatewayProviderDto>(src => src.Name, dto => dto.Name);
            CacheMap<RegisteredGatewayProvider, RegisteredGatewayProviderDto>(src => src.TypeFullName, dto => dto.TypeFullName);
            CacheMap<RegisteredGatewayProvider, RegisteredGatewayProviderDto>(src => src.ConfigurationData, dto => dto.ExtendedData);
            CacheMap<RegisteredGatewayProvider, RegisteredGatewayProviderDto>(src => src.EncryptConfigurationData, dto => dto.EncryptExtendedData);
            CacheMap<RegisteredGatewayProvider, RegisteredGatewayProviderDto>(src => src.CreateDate, dto => dto.CreateDate);
            CacheMap<RegisteredGatewayProvider, RegisteredGatewayProviderDto>(src => src.UpdateDate, dto => dto.UpdateDate);
        }
    }
}