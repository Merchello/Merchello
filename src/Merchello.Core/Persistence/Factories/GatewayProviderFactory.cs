using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    public class GatewayProviderFactory
    {
        public IGatewayProvider BuildEntity(GatewayProviderDto dto)
        {
            var provider = new GatewayProvider()
            {
                Key = dto.Key,
                GatewayProviderTfKey = dto.GatewayProviderTfKey,
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

        public GatewayProviderDto BuildDto(IGatewayProvider entity)
        {
            var dto = new GatewayProviderDto()
            {                
                Key = entity.Key,
                GatewayProviderTfKey = entity.GatewayProviderTfKey,
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