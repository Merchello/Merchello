namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;

    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Marker interface for a CampaignActivitySettingsRepository.
    /// </summary>
    internal interface ICampaignActivitySettingsRepository : IRepositoryQueryable<Guid, ICampaignActivitySettings>
    {
        /// <summary>
        /// Returns a collection of <see cref="ICampaignActivitySettings"/> for a given campaign.
        /// </summary>
        /// <param name="campaignKey">
        /// The campaign key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ICampaignActivitySettings}"/>.
        /// </returns>
        IEnumerable<ICampaignActivitySettings> GetByCampaignKey(Guid campaignKey);
    }
}