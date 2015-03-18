namespace Merchello.Core.Persistence.Factories
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    /// <summary>
    /// The campaign settings factory.
    /// </summary>
    internal class CampaignSettingsFactory : IEntityFactory<ICampaignSettings, CampaignSettingsDto>
    {
        /// <summary>
        /// The collection of <see cref="ICampaignActivitySettings"/>.
        /// </summary>
        private readonly IEnumerable<ICampaignActivitySettings> _activitySetttings;

        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignSettingsFactory"/> class.
        /// </summary>
        public CampaignSettingsFactory()
            : this(Enumerable.Empty<ICampaignActivitySettings>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignSettingsFactory"/> class.
        /// </summary>
        /// <param name="activitySettings">
        /// The activity settings.
        /// </param>
        public CampaignSettingsFactory(IEnumerable<ICampaignActivitySettings> activitySettings)
        {
            _activitySetttings = activitySettings;
        }

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
                           ActivitySettings = _activitySetttings,
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