namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;

    using Umbraco.Core.Services;

    /// <summary>
    /// Defines a CampaignActivitySettingsService.
    /// </summary>
    public interface ICampaignActivitySettingsService : IService
    {
        /// <summary>
        /// Creates a <see cref="ICampaignActivitySettings"/> without saving it to the database.
        /// </summary>
        /// <param name="campaignKey">
        /// The campaign Key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="campaignActivityType">
        /// The campaign activity type.
        /// </param>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="ICampaignActivitySettings"/>.
        /// </returns>
        /// <remarks>
        /// This cannot be used with Custom types!
        /// </remarks>
        ICampaignActivitySettings CreateCampaignActivitySettings(Guid campaignKey, string name, string alias, CampaignActivityType campaignActivityType, DateTime startDate, DateTime endDate, bool raiseEvents = true);

        /// <summary>
        /// Creates a <see cref="ICampaignActivitySettings"/> without saving it to the database.
        /// </summary>
        /// <param name="campaignKey">
        /// The campaign Key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="campaignActivityTfKey">
        /// The campaign activity type field key.
        /// </param>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="ICampaignActivitySettings"/>.
        /// </returns>
        ICampaignActivitySettings CreateCampaignActivitySettings(Guid campaignKey, string name, string alias, Guid campaignActivityTfKey, DateTime startDate, DateTime endDate, bool raiseEvents = true);

        /// <summary>
        /// Creates a <see cref="ICampaignActivitySettings"/> and saves it to the database.
        /// </summary>
        /// <param name="campaignKey">
        /// The campaign Key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <param name="campaignActivityTfKey">
        /// The campaign activity type field key.
        /// </param>
        /// <param name="startDate">
        /// The start date.
        /// </param>
        /// <param name="endDate">
        /// The end date.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        /// <returns>
        /// The <see cref="ICampaignActivitySettings"/>.
        /// </returns>
        ICampaignActivitySettings CreateCampaignActivitySettingsWithKey(Guid campaignKey, string name, string alias, Guid campaignActivityTfKey, DateTime startDate, DateTime endDate, bool raiseEvents = true);

        /// <summary>
        /// Saves a single <see cref="ICampaignActivitySettings"/>
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        void Save(ICampaignActivitySettings settings, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="ICampaignActivitySettings"/>
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        void Save(IEnumerable<ICampaignActivitySettings> settings, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="ICampaignActivitySettings"/>
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        void Delete(ICampaignActivitySettings settings, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="ICampaignActivitySettings"/>
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        void Delete(IEnumerable<ICampaignActivitySettings> settings, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="ICampaignActivitySettings"/> by it's unique key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ICampaignActivitySettings"/>.
        /// </returns>
        ICampaignActivitySettings GetByKey(Guid key);

        /// <summary>
        /// Gets a collection of <see cref="ICampaignActivitySettings"/> by a collection of keys
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ICampaignActivitySettings}"/>.
        /// </returns>
        IEnumerable<ICampaignActivitySettings> GetByKeys(IEnumerable<Guid> keys);

        /// <summary>
        /// Gets a collection of all CampaignActivities
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{ICampaignActivitySettings}"/>.
        /// </returns>
        IEnumerable<ICampaignActivitySettings> GetAll(); 
            
        /// <summary>
        /// Gets a collection of <see cref="ICampaignActivitySettings"/> for a given campaign.
        /// </summary>
        /// <param name="campaignKey">
        /// The campaign key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ICampaignActivitySettings}"/>.
        /// </returns>
        IEnumerable<ICampaignActivitySettings> GetByCampaignKey(Guid campaignKey);

        /// <summary>
        /// Gets a collection of "active" <see cref="ICampaignActivitySettings"/> for a given campaign
        /// </summary>
        /// <param name="campaignKey">
        /// The campaign key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ICampaignActivitySettings}"/>.
        /// </returns>
        IEnumerable<ICampaignActivitySettings> GetActiveByCampaignKey(Guid campaignKey);
    }
}