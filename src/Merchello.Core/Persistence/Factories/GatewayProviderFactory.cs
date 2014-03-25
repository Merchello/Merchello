using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Umbraco.Core;
namespace Merchello.Core.Persistence.Factories
{
    internal class GatewayProviderFactory : IEntityFactory<IGatewayProvider, GatewayProviderDto>
    {
        public IGatewayProvider BuildEntity(GatewayProviderDto dto)
        {
            var extendedData = string.IsNullOrEmpty(dto.ExtendedData)
                                   ? new ExtendedDataCollection()
                                   : new ExtendedDataCollection(
                                       dto.EncryptExtendedData ? 
                                       dto.ExtendedData.DecryptWithMachineKey() :
                                       dto.ExtendedData
                                       );

            var entity = new GatewayProvider()
            {
                Key = dto.Key,
                ProviderTfKey = dto.ProviderTfKey,
                Name = dto.Name,
                Description = dto.Description,
                TypeFullName = dto.TypeFullName,
                ExtendedData = extendedData,
                EncryptExtendedData = dto.EncryptExtendedData,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            entity.ResetDirtyProperties();

            return entity;
        }

        public GatewayProviderDto BuildDto(IGatewayProvider entity)
        {
            var extendedDataXml = entity.EncryptExtendedData
                                   ? entity.ExtendedData.SerializeToXml().EncryptWithMachineKey()
                                   : entity.ExtendedData.SerializeToXml();

            return new GatewayProviderDto()
            {
                Key = entity.Key,
                ProviderTfKey = entity.ProviderTfKey,
                Name = entity.Name,
                Description = entity.Description,
                TypeFullName = entity.TypeFullName,
                ExtendedData = extendedDataXml,
                EncryptExtendedData = entity.EncryptExtendedData,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

        }
    }
}