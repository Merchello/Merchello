using System;
using Lucene.Net.Search.Function;
using Merchello.Core.Gateways;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Models.TypeFields;
using Umbraco.Core;
using umbraco.editorControls.SettingControls.Pickers;

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

        /// <summary>
        /// Builds an entity based on a resolved type
        /// </summary>
        /// <param name="t">The resolved Type t</param>
        /// <param name="gatewayProviderType">The gateway provider type</param>
        /// <returns></returns>
        internal IGatewayProvider BuildEntity(Type t, GatewayProviderType gatewayProviderType)
        {
            Mandate.ParameterNotNull(t, "Type t cannot be null");
            Mandate.ParameterCondition(Attribute.GetCustomAttribute(t, typeof(GatewayProviderActivationAttribute)) != null, "Type t must have a GatewayProviderActivationAttribute");
            
            var att = (GatewayProviderActivationAttribute) Attribute.GetCustomAttribute(t, typeof(GatewayProviderActivationAttribute));
            
            var provider = new GatewayProvider()
            {
                Key = att.Key,
                ProviderTfKey = EnumTypeFieldConverter.GatewayProvider.GetTypeField(gatewayProviderType).TypeKey,
                Name = att.Name,
                Description = att.Description,
                TypeFullName = t.AssemblyQualifiedName,
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