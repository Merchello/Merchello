using System;
using Merchello.Core.Gateways;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Models.TypeFields;
using Umbraco.Core;

namespace Merchello.Core.Persistence.Factories
{
    internal class GatewayProviderFactory : IEntityFactory<IGatewayProviderSetting, GatewayProviderSettingDto>
    {
        public IGatewayProviderSetting BuildEntity(GatewayProviderSettingDto dto)
        {
            var extendedData = string.IsNullOrEmpty(dto.ExtendedData)
                                   ? new ExtendedDataCollection()
                                   : new ExtendedDataCollection(
                                       dto.EncryptExtendedData ? 
                                       dto.ExtendedData.DecryptWithMachineKey() :
                                       dto.ExtendedData
                                       );

            var entity = new GatewayProviderSetting()
            {
                Key = dto.Key,
                ProviderTfKey = dto.ProviderTfKey,
                Name = dto.Name,
                Description = dto.Description,
                ExtendedData = extendedData,
                EncryptExtendedData = dto.EncryptExtendedData,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            entity.ResetDirtyProperties();

            return entity;
        }

        public GatewayProviderSettingDto BuildDto(IGatewayProviderSetting entity)
        {
            var extendedDataXml = entity.EncryptExtendedData
                                   ? entity.ExtendedData.SerializeToXml().EncryptWithMachineKey()
                                   : entity.ExtendedData.SerializeToXml();

            return new GatewayProviderSettingDto()
            {
                Key = entity.Key,
                ProviderTfKey = entity.ProviderTfKey,
                Name = entity.Name,
                Description = entity.Description,
                ExtendedData = extendedDataXml,
                EncryptExtendedData = entity.EncryptExtendedData,
                UpdateDate = entity.UpdateDate,
                CreateDate = entity.CreateDate
            };

        }

        /// <summary>
        /// Builds an entity based on a resolved type
        /// </summary>
        /// <param name="t">The resolved Type t</param>
        /// <param name="gatewayProviderType">The gateway provider type</param>
        /// <returns></returns>
        internal IGatewayProviderSetting BuildEntity(Type t, GatewayProviderType gatewayProviderType)
        {
            Mandate.ParameterNotNull(t, "Type t cannot be null");
            Mandate.ParameterCondition(t.GetCustomAttribute<GatewayProviderActivationAttribute>(false) != null, "Type t must have a GatewayProviderActivationAttribute");

            var att = t.GetCustomAttribute<GatewayProviderActivationAttribute>(false);
                           
            var provider = new GatewayProviderSetting()
            {
                Key = att.Key,
                ProviderTfKey = EnumTypeFieldConverter.GatewayProvider.GetTypeField(gatewayProviderType).TypeKey,
                Name = att.Name,
                Description = att.Description,
                ExtendedData = new ExtendedDataCollection(),
                EncryptExtendedData  = false,
                UpdateDate = DateTime.Now,
                CreateDate = DateTime.Now
            };
            
            provider.ResetHasIdentity();

            return provider;
        }
    }
}