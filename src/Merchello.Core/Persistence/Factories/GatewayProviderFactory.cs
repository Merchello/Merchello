using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;

namespace Merchello.Core.Persistence.Factories
{
    internal class GatewayProviderFactory : IEntityFactory<IGatewayProvider, GatewayProviderDto>
    {
        public IGatewayProvider BuildEntity(GatewayProviderDto dto)
        {
            var entity = new GatewayProvider()
            {
                Key = dto.Key,
                ProviderTfKey = dto.ProviderTfKey,
                Name = dto.Name,
                TypeFullName = dto.TypeFullName,
                ExtendedData = string.IsNullOrEmpty(dto.ExtendedData) ? new ExtendedDataCollection() : new ExtendedDataCollection(dto.ExtendedData),
                EncryptExtendedData = dto.EncryptExtendedData,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            entity.ResetDirtyProperties();

            return entity;
        }

        public GatewayProviderDto BuildDto(IGatewayProvider entity)
        {
            return new GatewayProviderDto()
            {
                Key = entity.Key,
                ProviderTfKey = entity.ProviderTfKey,
                Name = entity.Name,
                TypeFullName = entity.TypeFullName,
                ExtendedData = entity.ExtendedData.SerializeToXml(),
                EncryptExtendedData = entity.EncryptExtendedData,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

        }
    }
}