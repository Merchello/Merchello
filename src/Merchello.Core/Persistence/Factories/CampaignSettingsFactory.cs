namespace Merchello.Core.Persistence.Factories
{
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The campaign settings factory.
    /// </summary>
    internal class CampaignSettingsFactory : IEntityFactory<ICampaignSettings, CampaignSettingsDto>
    {
        /// <summary>
        /// The build entity.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <returns>
        /// The <see cref="ICampaignSettings"/>.
        /// </returns>
        public ICampaignSettings BuildEntity(CampaignSettingsDto dto)
        {
            var settings = new CampaignSettings()
                       {
                           Key = dto.Key,
                           Name = dto.Name,
                           Alias = dto.Alias,
                           Description = dto.Description,
                           Active = dto.Active,
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
        /// The <see cref="CampaignSettingsDto"/>.
        /// </returns>
        public CampaignSettingsDto BuildDto(ICampaignSettings entity)
        {
            return new CampaignSettingsDto()
                       {
                           Key = entity.Key,
                           Name = entity.Name,
                           Alias = entity.Alias,
                           Description = entity.Description,
                           Active = entity.Active,
                           UpdateDate = entity.UpdateDate,
                           CreateDate = entity.CreateDate
                       };
        }
    }
}