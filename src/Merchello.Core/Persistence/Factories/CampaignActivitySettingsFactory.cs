namespace Merchello.Core.Persistence.Factories
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The campaign activity settings factory.
    /// </summary>
    internal class CampaignActivitySettingsFactory : IEntityFactory<ICampaignActivitySettings, CampaignActivitySettingsDto>
    {
        /// <summary>
        /// Builds a <see cref="ICampaignActivitySettings"/> entity.
        /// </summary>
        /// <param name="dto">
        /// The <see cref="CampaignActivitySettingsDto"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ICampaignActivitySettings"/>.
        /// </returns>
        public ICampaignActivitySettings BuildEntity(CampaignActivitySettingsDto dto)
        {
            var extendedData = string.IsNullOrEmpty(dto.ExtendedData)
                                   ? new ExtendedDataCollection()
                                   : new ExtendedDataCollection(dto.ExtendedData);
            
            var settings = new CampaignActivitySettings(dto.CampaignKey, dto.CampaignActivityTfKey)
            {
                Key = dto.Key,
                Name = dto.Name,
                Alias = dto.Code,
                Description = dto.Description,
                Active = dto.Active,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ExtendedData = extendedData,
                UpdateDate = dto.UpdateDate,
                CreateDate = dto.CreateDate
            };

            settings.ResetDirtyProperties();

            return settings;
        }

        /// <summary>
        /// The build dto.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="CampaignActivitySettingsDto"/>.
        /// </returns>
        public CampaignActivitySettingsDto BuildDto(ICampaignActivitySettings entity)
        {
            return new CampaignActivitySettingsDto()
                       {
                           Key = entity.Key,
                           Name = entity.Name,
                           Code = entity.Alias,
                           Description = entity.Description,
                           Active = entity.Active,
                           StartDate = entity.StartDate,
                           EndDate = entity.EndDate,
                           CampaignKey = entity.CampaignKey,
                           CampaignActivityTfKey = entity.CampaignActivityTfKey,
                           ExtendedData = entity.ExtendedData.SerializeToXml(),
                           UpdateDate = entity.UpdateDate,
                           CreateDate = entity.CreateDate
                       };
        }
    }
}