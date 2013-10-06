using Merchello.Core.Models.GatewayProviders;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    public class RegisteredGatewayProviderFactory
    {
        public IRegisteredGatewayProvider BuildEntity(RegisteredGatewayProviderDto dto)
        {
            var provider = new RegisteredGatewayProvider()
            {
                Key = dto.Key,
                GatewayProviderTypeFieldKey = dto.GatewayProviderTypeFieldKey,
                Name = dto.Name,
                TypeFullName = dto.TypeFullName,
                ConfigurationData = dto.ExtendedData,
                EncryptConfigurationData = dto.EncryptExtendedData,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            provider.ResetDirtyProperties();

            return provider;
        }

        public RegisteredGatewayProviderDto BuildDto(IRegisteredGatewayProvider entity)
        {
            var dto = new RegisteredGatewayProviderDto()
            {                
                Key = entity.Key,
                GatewayProviderTypeFieldKey = entity.GatewayProviderTypeFieldKey,
                Name = entity.Name,
                TypeFullName = entity.TypeFullName,
                ExtendedData = entity.ConfigurationData,
                EncryptExtendedData = entity.EncryptConfigurationData,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

            return dto;
        }
    }
}