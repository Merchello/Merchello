namespace Merchello.Core.Persistence.Factories
{
    using System;
    using Gateways;
    using Models;
    using Models.Rdbms;
    using Models.TypeFields;
    using Umbraco.Core;

    /// <summary>
    /// The gateway provider settings factory.
    /// </summary>
    internal class GatewayProviderSettingsFactory : IEntityFactory<IGatewayProviderSettings, GatewayProviderSettingsDto>
    {
        /// <summary>
        /// The build entity.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="IGatewayProviderSettings"/>.
        /// </returns>
        public IGatewayProviderSettings BuildEntity(GatewayProviderSettingsDto dto)
        {
            var extendedData = string.IsNullOrEmpty(dto.ExtendedData)
                                   ? new ExtendedDataCollection()
                                   : new ExtendedDataCollection(
                                       dto.EncryptExtendedData ? 
                                       dto.ExtendedData.DecryptWithMachineKey() :
                                       dto.ExtendedData);

            var entity = new GatewayProviderSettings()
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

        /// <summary>
        /// The build dto.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="GatewayProviderSettingsDto"/>.
        /// </returns>
        public GatewayProviderSettingsDto BuildDto(IGatewayProviderSettings entity)
        {
            var extendedDataXml = entity.EncryptExtendedData
                                   ? entity.ExtendedData.SerializeToXml().EncryptWithMachineKey()
                                   : entity.ExtendedData.SerializeToXml();

            return new GatewayProviderSettingsDto()
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
        /// <returns>The <see cref="IGatewayProviderSettings"/></returns>
        internal IGatewayProviderSettings BuildEntity(Type t, GatewayProviderType gatewayProviderType)
        {
            Mandate.ParameterNotNull(t, "Type t cannot be null");
            Mandate.ParameterCondition(t.GetCustomAttribute<GatewayProviderActivationAttribute>(false) != null, "Type t must have a GatewayProviderActivationAttribute");

            var att = t.GetCustomAttribute<GatewayProviderActivationAttribute>(false);
                           
            var provider = new GatewayProviderSettings()
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